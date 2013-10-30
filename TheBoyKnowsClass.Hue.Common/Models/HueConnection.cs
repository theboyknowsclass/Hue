using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TheBoyKnowsClass.Common.Models.Queue;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;
using TheBoyKnowsClass.Hue.Common.Operations;
using TheBoyKnowsClass.Hue.Common.Properties;
using TheBoyKnowsClass.Schedules.Common.Models;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public delegate Task<HueObjectBase> HueActionDelegate();

    public class HueConnection
    {
        // This is set so as not to overload the Hue Bridge
        private const int ThrottleRate = 100;

        private readonly SyncItemQueue<QueueItem> _queue;
        private readonly ScheduleTicker _scheduleTicker;
        private readonly WaitHandle[] _queueEvents;
        private readonly WaitHandle[] _tickerEvents;
        private readonly EventWaitHandle _exitThreadEvent;

        private readonly Dictionary<string, HueObjectBase> _syncItemQueueResponses; 

        private Bridge _bridge;
        private string _loggedInID;
        private readonly HttpClient _httpClient;
        private bool _throttle;
        private Task _task;
        private readonly object _consumerSyncRoot;


        public delegate void ResponseReadyEventHandler(object sender, HueResponseEventArgs e);
        public event ResponseReadyEventHandler ResponseReady;


        protected virtual void OnResponseReady(HueResponseEventArgs e)
        {
            if (ResponseReady != null) ResponseReady(this, e);
        }

        public HueConnection()
        {
            // Limit the max buffer size for the response so we don't get overwhelmed
            _httpClient = new HttpClient {MaxResponseContentBufferSize = 256000};
            _httpClient.DefaultRequestHeaders.Add("user-agent","Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

            _queue = new SyncItemQueue<QueueItem>(true);
            _scheduleTicker = new ScheduleTicker(new Schedules.Common.Models.Schedule(), new TimeSpan(0, 0, 0, 0, ThrottleRate));

            _exitThreadEvent = new ManualResetEvent(false);
            _queueEvents = new WaitHandle[] { _exitThreadEvent, _queue.ItemsQueuedEvent };
            _tickerEvents = new WaitHandle[] { _exitThreadEvent, _scheduleTicker.TickerEvent };
            _syncItemQueueResponses = new Dictionary<string, HueObjectBase>();

            _consumerSyncRoot = new object();
        }

        ~HueConnection()  // destructor
        {
            _exitThreadEvent.Set();
        }

        #region Public Properties

        public Bridge Bridge
        {
            get { return _bridge; }
            internal set { _bridge = value; }
        }

        public bool Connected
        {
            get { return _bridge != null; }
        }

        public string LoggedInID
        {
            get { return _loggedInID; }
            internal set { _loggedInID = value; }
        }

        public bool IsLoggedIn
        {
            get { return !String.IsNullOrEmpty(_loggedInID); }
        }

        public bool Throttle
        {
            get { return _throttle; }
            set
            {
                lock (_consumerSyncRoot)
                {
                    _throttle = value;

                    if (value)
                    {
                        StartConsumer();
                    }
                    else
                    {
                        _exitThreadEvent.Set();
                        _scheduleTicker.TimerStop();
                    }
                }

            }
        }

        #endregion

        #region URI Methods


        internal string LightURI(string lightID)
        {
            return String.Format(Resources.LightURI, _bridge.InternalIPAddress, _loggedInID, lightID);
        }

        internal string LightStateURI(string lightID)
        {
            return String.Format(Resources.LightStateURI, _bridge.InternalIPAddress, _loggedInID, lightID);
        }

        internal string GroupsAllURI()
        {
            return String.Format(Resources.GroupsAllURI, _bridge.InternalIPAddress, _loggedInID);
        }

        internal string GroupURI(string groupID)
        {
            return String.Format(Resources.GroupURI, _bridge.InternalIPAddress, _loggedInID, groupID);
        }

        internal string GroupStateURI(string groupID)
        {
            return String.Format(Resources.GroupStateURI, _bridge.InternalIPAddress, _loggedInID, groupID);
        }

        internal string ScheduleURI(string scheduleID)
        {
            return ScheduleURI(_bridge.InternalIPAddress, _loggedInID, scheduleID);
        }

        internal static string ScheduleURI(string bridgeID, string userID, string scheduleID)
        {
            return String.Format(Resources.ScheduleURI, bridgeID, userID, scheduleID);
        }

        #endregion

        #region HTTP Helper Methods

        internal async Task<string> GetAsync(string uri)
        {
            return await HTTPOperations.GetAsync(_httpClient, uri);
        }

        internal async Task<string> PostAsync(string uri, string postContent)
        {
            return await HTTPOperations.PostAsync(_httpClient, uri, postContent);
        }

        internal async Task<string> PutAsync(string uri, string postContent)
        {
            return await HTTPOperations.PutAsync(_httpClient, uri, postContent);
        }

        internal async Task<string> DeleteAsync(string uri)
        {
            return await HTTPOperations.DeleteAsync(_httpClient, uri);
        }

        #endregion

        #region Helper Methods

        private void StartConsumer()
        {
            if (_task == null)
            {
                _scheduleTicker.TimerStart();
                _task = Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        Debug.WriteLine("starting consumer thread");
                        Debug.WriteLine("waiting for queue or kill task");
                        while (WaitHandle.WaitAny(_queueEvents) != 0)
                        {
                            Debug.WriteLine("queued item detected, waiting for ticker");
                            while (WaitHandle.WaitAny(_tickerEvents) != 0)
                            {
                                Debug.WriteLine("ticker detected");
                                QueueItem dequeueItem = _queue.DequeueSync();

                                if (dequeueItem != null)
                                {
                                    HueObjectBase rv = await LoginWrapperAsync(dequeueItem.Action);

                                    if (_syncItemQueueResponses.ContainsKey(dequeueItem.Path))
                                    {
                                        _syncItemQueueResponses[dequeueItem.Path] = rv;
                                    }
                                    else
                                    {
                                        _syncItemQueueResponses.Add(dequeueItem.Path, rv);
                                    }

                                    Debug.WriteLine(rv);
                                    var eventArgs = new HueResponseEventArgs { Response = rv, Path = dequeueItem.Path };
                                    OnResponseReady(eventArgs);
                                }
                                break;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception.Message);
                        _task = null;
                    }

                    Debug.WriteLine("stopping consumer thread");
                });
            }
        }

        internal async Task<HueObjectBase> ThrottleWrapperAsync(QueueItem item)
        {
            if (Throttle)
            {
                _queue.EnqueueSync(item);

                var success = new HueObjectCollectionBase<Success>();
                success.AutoAdd(new Success(item.Path, "Item Queued Successfully"));
                return success;
            }

            return await LoginWrapperAsync(item.Action);
        }

        internal async Task<HueObjectBase> LoginWrapperAsync(HueActionDelegate hueAction)
        {
            HueActionDelegate method = async delegate
            {
                return !IsLoggedIn ? (HueObjectBase)new Error(-1, "", "No User Logged In") : await hueAction();
            };
            return await ConnectionWrapperAsync(method);
        }

        internal async Task<HueObjectBase> ConnectionWrapperAsync(HueActionDelegate hueAction)
        {
            return !Connected ? (HueObjectBase)new Error(-1, "", "Not connected to a Hue Bridge") : await hueAction(); 
        }

        #endregion

        public class QueueItem : IEquatable<QueueItem>
        {
            public DateTime Time { get; set; }
            public string Path { get; set; }
            public HueActionDelegate Action { get; set; }

            public bool Equals(QueueItem other)
            {
                return other.Path == Path && Math.Abs((Time - other.Time).Milliseconds) < ThrottleRate;
            }
        }
    }
}