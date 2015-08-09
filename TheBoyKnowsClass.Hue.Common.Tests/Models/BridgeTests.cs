using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Tests.Models
{
    [TestClass]
    public class BridgeTests
    {

        [TestMethod]
        public async Task ConnectTest()
        {
            var rv = await GetConnectedBridge();
            Assert.IsTrue(rv.IsConnected);
            Assert.IsInstanceOfType(rv, typeof(Bridge));
        }


        [TestMethod]
        public async Task LoginFailTest()
        {
            string invalidID = new Guid().ToString();

            var rv = await GetConnectedBridge();
            Assert.IsInstanceOfType(await rv.LoginAsync(invalidID), typeof(HueObjectCollectionBase<Error>));
            Assert.IsFalse(rv.IsLoggedIn);
        }

        [TestMethod]
        public async Task LoginSuccessTest()
        {
            const string validID = "newdeveloper1";

            var rv = await GetConnectedBridge();
            Assert.IsInstanceOfType(await rv.LoginAsync(validID), typeof(HueObjectCollectionBase<Success>));
            Assert.IsTrue(rv.IsLoggedIn);
        }


        [TestMethod]
        public async Task GetBridgeConfigurationTest()
        {
            Bridge bridge = await GetLoggedInBridge();
            Assert.IsInstanceOfType(await bridge.GetBridgeConfigurationAsync(),typeof(BridgeConfig));

        }

        #region Client API

        [TestMethod]
        public async Task CreateNewClientLinkFailTest()
        {
            const string invalidID = "NewAutomatedTestDeveloper";
            const string deviceType = "AutomatedTestDevice";

            Bridge bridge = await GetLoggedInBridge();

            var bridgeConfig = new BridgeConfig { LinkButton = false };

            Assert.IsInstanceOfType(await bridge.SetBridgeConfigurationAsync(bridgeConfig),
                                    typeof(HueObjectCollectionBase<Success>));
            Assert.IsFalse(((BridgeConfig)await bridge.GetBridgeConfigurationAsync()).LinkButton == true);
            Assert.IsInstanceOfType(await bridge.CreateNewClientAsync(deviceType, invalidID),
                                    typeof(HueObjectCollectionBase<Error>));
        }

        [TestMethod]
        public async Task CreateAndRemoveClientTest()
        {
            const string invalidID = "NewAutomatedTestDeveloper";
            const string deviceType = "AutomatedTestDevice";

            Bridge bridge = await GetLoggedInBridge();

            var bridgeConfig = new BridgeConfig { LinkButton = true };

            Assert.IsInstanceOfType(await bridge.SetBridgeConfigurationAsync(bridgeConfig),
                                    typeof(HueObjectCollectionBase<Success>));

            Assert.IsTrue(((BridgeConfig)await bridge.GetBridgeConfigurationAsync()).LinkButton == true);

            Assert.IsInstanceOfType(await bridge.CreateNewClientAsync(deviceType, invalidID),
                                    typeof(HueObjectCollectionBase<Success>));

            Assert.IsInstanceOfType(await bridge.RemoveClientAsync(invalidID), typeof(HueObjectCollectionBase<Success>));
        }

        #endregion

        #region Lights API

        [TestMethod]
        public async Task GetLightsTest()
        {
            var bridge = await GetLoggedInBridge();
            var rv = await bridge.GetLightsAsync();

            Assert.IsInstanceOfType(rv, typeof(HueObjectCollectionBase<Light>));

            Assert.IsInstanceOfType(bridge.Lights, typeof(HueObjectCollectionBase<Light>));
            Assert.IsInstanceOfType(bridge.Lights, typeof(HueObjectCollectionBase<Light>));
        }

        [TestMethod]
        public async Task GetLightsFullTest()
        {
            var bridge = await GetLoggedInBridge();

            var lights = (HueObjectCollectionBase<Light>)await bridge.GetLightsFullAsync();

            Light light = lights.Dictionary.Values.FirstOrDefault();

            if (light != null)
            {
                Assert.IsNotNull(light);
                Assert.IsNotNull(light.State);
            }
        }

        [TestMethod]
        public async Task StartNewLightScanTest()
        {
            Assert.IsInstanceOfType(await (await GetLoggedInBridge()).StartNewLightScanAsync(), typeof(HueObjectCollectionBase<Success>));
        }

        [TestMethod]
        public async Task GetLastAddedLightsTest()
        {
            Assert.IsInstanceOfType(await (await GetLoggedInBridge()).GetLastAddedLightsAsync(), typeof(LastAddedLights));
        }

        #endregion

        #region Groups API

        [TestMethod]
        public async Task CreateAndRemoveGroupTest()
        {
            var rv = await (await GetLoggedInBridge()).CreateGroupAsync("myNewGroup", new List<string> { "1", "2", "3" });

            Assert.IsInstanceOfType(rv, typeof(HueObjectCollectionBase<Success>));

            string id = (from r in ((HueObjectCollectionBase<Success>)rv).Dictionary
                         select r.Value.PathValue.Replace("/groups/", "")).FirstOrDefault();

            Assert.IsNotNull(id);

            Assert.IsInstanceOfType(await (await GetLoggedInBridge()).DeleteGroupAsync(id), typeof(HueObjectCollectionBase<Success>));
        }

        [TestMethod]
        public async Task GetGroupsTest()
        {
            var bridge = await GetLoggedInBridge();

            Assert.IsInstanceOfType(await bridge.GetGroupsAsync(), typeof(HueObjectCollectionBase<Group>));

            Assert.IsInstanceOfType(bridge.Groups, typeof(HueObjectCollectionBase<Group>));
            Assert.IsInstanceOfType(bridge.Groups, typeof(HueObjectCollectionBase<Group>));
        }

        [TestMethod]
        public async Task GetGroupsFullTest()
        {
            var bridge = await GetLoggedInBridge();

            var groups = (HueObjectCollectionBase<Group>)await bridge.GetGroupsFullAsync();

            Group group = groups.Dictionary.Values.FirstOrDefault();

            if (group != null)
            {
                Assert.IsNotNull(group);
                Assert.IsNotNull(group.State);
            }
        }

        #endregion

        #region Schedules API

        [TestMethod]
        public async Task CreateScheduleAndDeleteTest()
        {
            var bridge = await GetLoggedInBridge();

            var command = new Command { Address = "/api/0/groups/1/action", Method = "PUT" };
            command.Body.Add("on", true);
            command.Body.Add("Hue", 25500);
            command.Body.Add("Brightness", 255);
            command.Body.Add("Saturation", 255);

            var schedule = new Schedule(command, DateTime.Now.AddMinutes(1)) { Description = "test schedule", Name = "test schedule" };

            var rv = await bridge.CreateScheduleAsync(schedule) as HueObjectCollectionBase<Success>;

            Assert.IsNotNull(rv);

            string id = (from kvp in rv.Dictionary select kvp.Value.PathValue).FirstOrDefault();

            Assert.IsNotNull(id);

            Assert.IsInstanceOfType(await bridge.DeleteScheduleAsync(id), typeof(HueObjectCollectionBase<Success>));
        }

        [TestMethod]
        public async Task GetSchedulesTest()
        {
            Assert.IsInstanceOfType(await (await GetLoggedInBridge()).GetSchedulesAsync(), typeof(HueObjectCollectionBase<Schedule>));
        }

        //[TestMethod]
        //public void GetScheduleAttributesTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod]
        //public void SetScheduleAttributesTest()
        //{
        //    Assert.Fail();
        //}

        #endregion

        #region Helper Methods

        public static async Task<Bridge> GetConnectedBridge()
        {
            Bridge bridge = await BridgeFinderTests.GetFirstBridge();
            Assert.IsFalse(bridge.IsConnected);
            var connection =  bridge.Connect();
            Assert.IsTrue(bridge.IsConnected);
            return bridge;
        }

        public static async Task<Bridge> GetLoggedInBridge()
        {
            const string validID = "newdeveloper1";
            Bridge rv = await GetConnectedBridge();
            await rv.LoginAsync(validID);
            return rv;
        }

        #endregion
    }
}
