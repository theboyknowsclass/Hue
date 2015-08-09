using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Tests.Models
{
    [TestClass]
    public class GroupTests
    {

        [TestMethod]
        public async Task GetGroupAttributesTest()
        {
            var bridge = await BridgeTests.GetLoggedInBridge();

            var groups = (HueObjectCollectionBase<Group>)await bridge.GetGroupsAsync();

            Group group = groups.Dictionary.Values.FirstOrDefault();

            if (group != null)
            {
                //Assert.IsNull(group.State);
                group = await group.GetAttributesAsync() as Group;
                Assert.IsNotNull(group);
                Assert.IsNotNull(group.State);
            }
        }

        [TestMethod]
        public async Task SetStateTest()
        {
            var bridge = await BridgeTests.GetLoggedInBridge();

            var groups = (HueObjectCollectionBase<Group>)await bridge.GetGroupsAsync();

            var oldStates = new Dictionary<string, State>();

            foreach (Group rv in groups.Dictionary.Values)
            {
                var group = await rv.GetAttributesAsync() as Group;
                Assert.IsNotNull(group);
                oldStates.Add(rv.ID, group.State);
                var newState = new State { Hue = 46920, Brightness = 254, Saturation = 254, On = true };
                Assert.IsInstanceOfType(await rv.SetStateAsync(newState), typeof(HueObjectCollectionBase<Success>));
                System.Threading.Thread.Sleep(3000);
                group = await rv.GetAttributesAsync() as Group;
                Assert.IsNotNull(group);
                Assert.AreEqual(group.State.Hue, newState.Hue);
                Assert.AreEqual(group.State.Brightness, newState.Brightness);
                Assert.AreEqual(group.State.Saturation, newState.Saturation);
                Assert.AreEqual(group.State.ColorMode, "hs");
            }

            foreach (KeyValuePair<string, State> rv in oldStates)
            {
                var group = groups.Dictionary[rv.Key];
                Assert.IsNotNull(group);
                State newState;

                switch (rv.Value.ColorMode)
                {
                    case "hs":
                        newState = new State
                        {
                            Hue = rv.Value.Hue,
                            Brightness = rv.Value.Brightness,
                            Saturation = rv.Value.Saturation,
                            On = rv.Value.On
                        };
                        Assert.IsInstanceOfType(await group.SetStateAsync(newState),
                                                typeof(HueObjectCollectionBase<Success>));
                        group = await group.GetAttributesAsync() as Group;
                        Assert.IsNotNull(group);
                        Assert.AreEqual(group.State.Hue, newState.Hue);
                        Assert.AreEqual(group.State.Brightness, newState.Brightness);
                        Assert.AreEqual(group.State.Saturation, newState.Saturation);
                        //Assert.AreEqual(group.State.ColorMode, "hs");
                        break;
                    case "xy":
                        newState = new State { CIEColor = rv.Value.CIEColor, On = rv.Value.On };
                        Assert.IsInstanceOfType(await group.SetStateAsync(newState),
                                                typeof(HueObjectCollectionBase<Success>));
                        group = await group.GetAttributesAsync() as Group;
                        Assert.IsNotNull(group);
                        Assert.AreEqual(group.State.CIEColor[0], newState.CIEColor[0]);
                        Assert.AreEqual(group.State.CIEColor[1], newState.CIEColor[1]);
                        //Assert.AreEqual(group.State.ColorMode, "xy");
                        break;
                    case "ct":
                        newState = new State { ColorTemperature = rv.Value.ColorTemperature, On = rv.Value.On };
                        Assert.IsInstanceOfType(await group.SetStateAsync(newState),
                                                typeof(HueObjectCollectionBase<Success>));
                        group = await group.GetAttributesAsync() as Group;
                        Assert.IsNotNull(group);
                        Assert.AreEqual(group.State.ColorTemperature, newState.ColorTemperature);
                        //Assert.AreEqual(group.State.ColorMode, "ct");
                        break;
                }
            }
        }
    }
}
