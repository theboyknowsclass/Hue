using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;
using TheBoyKnowsClass.Hue.Common.Models.Factories;
using TheBoyKnowsClass.Hue.Common.Operations;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    [DataContract]
    public class Bridge : HueConnectedObjectBase
    {
        private HueObjectCollectionBase<Group> _groups;
        private HueObjectCollectionBase<Light> _lights;

        public Bridge(JObject jObject, HueConnection hueConnection) : base(hueConnection, (string)jObject["id"])
        {
            InternalIPAddress = (string)jObject["internalipaddress"];
            MACAddress = (string)jObject["macaddress"];

            _groups = new HueObjectCollectionBase<Group>();
            _lights = new HueObjectCollectionBase<Light>();
        }

        [DataMember(Name = "id")]
        new public string ID
        { get { return base.ID; }}
        [DataMember(Name = "internalipaddress")]
        public string InternalIPAddress { get; private set; }
        [DataMember(Name = "macaddress")]
        public string MACAddress { get; private set; }

        public BridgeConfig Config { get; set; }

        public bool IsConnected
        {
            get { return Context.Connected && InternalIPAddress == Context.Bridge.InternalIPAddress; }
        }

        public bool IsLoggedIn
        {
            get { return IsConnected && Context.IsLoggedIn; }
        }

        public new HueConnection Context
        {
            get
            {
                return base.Context;
            }
        }

        public HueObjectCollectionBase<Light> Lights
        {
            get
            {
                if (_lights != null)
                {
                    return _lights;
                }
                return _lights = GetLightsAsync().Result as HueObjectCollectionBase<Light>;
            }
        }

        public HueObjectCollectionBase<Group> Groups
        {
            get
            {
                if (_groups != null)
                {
                    return _groups;
                }
                _groups = GetGroupsAsync().Result as HueObjectCollectionBase<Group>;
                return _groups;
            }
        }


        #region Connection Methods

        public HueConnection Connect()
        {
            Context.Bridge = this;
            return Context;
        }

        public async Task<HueObjectBase> LoginAsync(string userID)
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.GetAsync(String.Format(Resources.ConfigURI, InternalIPAddress, userID));

                HueObjectBase returnObject = HueObjectFactory.CreateHueObject(returnString, HueObjectType.BridgeConfig);

                var bridgeConfig = returnObject as BridgeConfig;
                if (bridgeConfig != null)
                {
                    if (bridgeConfig.WhiteList.Count > 0)
                    {
                        Context.LoggedInID = userID;
                        var success = new HueObjectCollectionBase<Success>();
                        success.Dictionary.Add("1", new Success("", String.Format("Logged in with id {0}", userID)));
                        return success;
                    }

                    var error = new HueObjectCollectionBase<Error>();
                    error.Dictionary.Add("1", new Error(-1, returnString, String.Format("unable to login with id {0}", userID)));
                    return error;
                }

                return returnObject;
            };
            return await Context.ConnectionWrapperAsync(method);
        }

        #endregion

        #region Configuration API

        public async Task<HueObjectBase> GetBridgeConfigurationAsync()
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.GetAsync(String.Format(Resources.ConfigURI, InternalIPAddress, Context.LoggedInID));
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.BridgeConfig);
            };
            return await Context.LoginWrapperAsync(method);
        }

        public async Task<HueObjectBase> SetBridgeConfigurationAsync(BridgeConfig bridgeConfig)
        {
            HueActionDelegate method = async delegate
            {
                string newConfigString = bridgeConfig.ToJSON();
                string returnString = await Context.PutAsync(String.Format(Resources.ConfigURI, InternalIPAddress, Context.LoggedInID), newConfigString);
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.Success);
            };

            return await Context.LoginWrapperAsync(method);
        }

        #endregion

        #region Client API

        public async Task<HueObjectBase> CreateNewClientAsync(string clientType, string id)
        {
            HueActionDelegate method = async delegate
            {
                var newClient = new Client(clientType, id);
                string newClientString = newClient.ToJSON();
                string returnString = await Context.PostAsync(String.Format(Resources.DefaultURI, InternalIPAddress), newClientString);
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.Success);
            };
            return await Context.ConnectionWrapperAsync(method);
        }

        public async Task<HueObjectBase> RemoveClientAsync(string idToDelete)
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.DeleteAsync(String.Format(Resources.UserDeleteURI, InternalIPAddress, Context.LoggedInID, idToDelete));
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.Success);
            };
            return await Context.LoginWrapperAsync(method);
        }

        #endregion

        #region Lights API

        public async Task<HueObjectBase> GetLightsAsync()
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.GetAsync(string.Format(Resources.LightsAllURI, InternalIPAddress, Context.LoggedInID));
                return HueObjectFactory.CreateHueObject(returnString, Context, HueObjectType.Light);
            };

            var rv = await Context.LoginWrapperAsync(method);
            var lights = rv as HueObjectCollectionBase<Light>;
            if (lights != null)
            {
                _lights = lights;
            }
            return rv;
        }

        public async Task<HueObjectBase> GetLightsFullAsync()
        {
            HueObjectBase lights = await GetLightsAsync();

            var lightsList = lights as HueObjectCollectionBase<Light>;
            if (lightsList != null)
            {
                var lightList2 = (from l in lightsList.Dictionary.Values select l).ToList();

                foreach (Light light in lightList2)
                {
                    HueObjectBase lightDetail = await light.GetAttributesAsync();

                    var detail = lightDetail as Light;
                    if (detail != null)
                    {
                        lightsList.Dictionary[light.ID] = detail;
                    }
                }

                _lights = lightsList;
            }
            return lights;
        }

        public async Task<HueObjectBase> StartNewLightScanAsync()
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.PostAsync(string.Format(Resources.LightsAllURI, InternalIPAddress, Context.LoggedInID), "");
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.Success);
            }
                ;
            return await Context.LoginWrapperAsync(method);
        }

        public async Task<HueObjectBase> GetLastAddedLightsAsync()
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.GetAsync(string.Format(Resources.LightsNewURI, InternalIPAddress, Context.LoggedInID));
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.LastAddedLights);
            }
                ;
            return await Context.LoginWrapperAsync(method);
        }

        #endregion

        #region Groups API

        public async Task<HueObjectBase> CreateGroupAsync(string groupName, IEnumerable<string> lights)
        {
            HueActionDelegate method = async delegate
            {
                var newGroup = new Group { Name = groupName, LightsIDs = lights.ToList() };
                string newGroupString = newGroup.ToJSON();
                string returnString = await Context.PostAsync(Context.GroupsAllURI(), newGroupString);
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.Success);
            };
            return await Context.LoginWrapperAsync(method);
        }

        public async Task<HueObjectBase> DeleteGroupAsync(string groupID)
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.DeleteAsync(Context.GroupURI(groupID));
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.Success);
            };
            return await Context.LoginWrapperAsync(method);
        }

        public async Task<HueObjectBase> GetGroupsAsync()
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.GetAsync(Context.GroupsAllURI());
                return HueObjectFactory.CreateHueObject(returnString, Context, HueObjectType.Group);
            };

            var rv = await Context.LoginWrapperAsync(method);
            var groups = rv as HueObjectCollectionBase<Group>;
            if (groups != null)
            {
                _groups = groups;
            }
            return rv;
        }

        public async Task<HueObjectBase> GetGroupsFullAsync()
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.GetAsync(Context.GroupsAllURI());
                var groups = HueObjectFactory.CreateHueObject(returnString, Context, HueObjectType.Group) as HueObjectCollectionBase<Group>;

                if (groups != null)
                {
                    var groups2 = (from l in groups.Dictionary.Values select l).ToList();

                    foreach (Group group in groups2)
                    {
                        HueObjectBase groupDetail = await group.GetAttributesAsync();

                        var detail = groupDetail as Group;
                        if (detail != null)
                        {
                            groups.Dictionary[group.ID] = detail;
                        }
                    }

                    _groups = groups;
                }
                return groups;
            };
            return await Context.LoginWrapperAsync(method);
        }

        #endregion

        #region Schedules API

        public async Task<HueObjectBase> CreateScheduleAsync(Schedule schedule)
        {
            HueActionDelegate method = async delegate
           {
                string newScheduleString = schedule.ToJSON();
                string returnString = await Context.PostAsync(String.Format(Resources.SchedulesAllURI, InternalIPAddress, Context.LoggedInID), newScheduleString);
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.Success);
            };
            return await Context.LoginWrapperAsync(method);
        }

        public async Task<HueObjectBase> DeleteScheduleAsync(string scheduleID)
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.DeleteAsync(String.Format(Resources.ScheduleURI, InternalIPAddress, Context.LoggedInID, scheduleID));
                return HueObjectFactory.CreateHueObject(returnString, HueObjectType.Success);
            };
            return await Context.LoginWrapperAsync(method);
        }

        public async Task<HueObjectBase> GetSchedulesAsync()
        {
            HueActionDelegate method = async delegate
            {
                string returnString = await Context.GetAsync(String.Format(Resources.SchedulesAllURI, InternalIPAddress, Context.LoggedInID));
                return HueObjectFactory.CreateHueObject(returnString, Context, HueObjectType.Schedule);
            };
            return await Context.LoginWrapperAsync(method);
        }

        #endregion
    }
}
