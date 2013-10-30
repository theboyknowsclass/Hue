using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Tests.Models
{
    [TestClass]
    public class LightTests
    {

        //[TestMethod]
        public async Task SetAlertTest()
        {
            Assert.Fail();
        }

        //[TestMethod]
        public async Task CancelAlertTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task GetAttributesTest()
        {
            var bridge = await BridgeTests.GetLoggedInBridge();

            var lights = (HueObjectCollectionBase<Light>)await bridge.GetLightsAsync();

            Light light = lights.Dictionary.Values.FirstOrDefault();

            if (light != null)
            {
                Assert.IsNull(light.State);
                light = await light.GetAttributesAsync() as Light;
                Assert.IsNotNull(light);
                Assert.IsNotNull(light.State);
            }
        }

        //[TestMethod()]
        public async Task RefreshAttributesTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task SetNameTest()
        {
            var bridge = await BridgeTests.GetLoggedInBridge();

            var lights = (HueObjectCollectionBase<Light>)await bridge.GetLightsAsync();

            Light light = lights.FirstOrDefault();

            if (light != null)
            {
                light = await light.GetAttributesAsync() as Light;
                Assert.IsNotNull(light);
                string oldName = light.Name;
                Assert.IsInstanceOfType(await light.SetNameAsync<Light>("New Light Name Test"), typeof(HueObjectCollectionBase<Success>));
                light = await light.GetAttributesAsync() as Light;
                Assert.IsNotNull(light);
                Assert.AreEqual(light.Name, "New Light Name Test");
                Assert.IsInstanceOfType(await light.SetNameAsync<Light>(oldName), typeof(HueObjectCollectionBase<Success>));
                light = await light.GetAttributesAsync() as Light;
                Assert.IsNotNull(light);
                Assert.AreEqual(light.Name, oldName);
            }
        }


        [TestMethod]
        public async Task SetStateTest()
        {
            var bridge = await BridgeTests.GetLoggedInBridge();

            var connection = bridge.Connect();
            connection.Throttle = true;

            var lights = (HueObjectCollectionBase<Light>)await bridge.GetLightsAsync();

            var oldStates = new Dictionary<string, State>();

            foreach (Light rv in lights.Dictionary.Values)
            {
                var light = await rv.GetAttributesAsync() as Light;
                Assert.IsNotNull(light);
                oldStates.Add(rv.ID, light.State);
                var newState = new State { Hue = 0, Brightness = 255, Saturation = 255, On = true };
                Assert.IsInstanceOfType(await rv.SetStateAsync(newState),
                                        typeof(HueObjectCollectionBase<Success>));
                System.Threading.Thread.Sleep(150);
                light = await rv.GetAttributesAsync() as Light;
                Assert.IsNotNull(light);
                Assert.AreEqual(light.State.Hue, newState.Hue);
                Assert.AreEqual(light.State.Brightness, newState.Brightness);
                Assert.AreEqual(light.State.Saturation, newState.Saturation);
                Assert.AreEqual(light.State.ColorMode, "hs");

                System.Threading.Thread.Sleep(100);
            }

            System.Threading.Thread.Sleep(3000);

            foreach (KeyValuePair<string, State> rv in oldStates)
            {
                var light = lights.Dictionary[rv.Key];
                Assert.IsNotNull(light);
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
                        Assert.IsInstanceOfType(await light.SetStateAsync(newState),
                                                typeof(HueObjectCollectionBase<Success>));
                        System.Threading.Thread.Sleep(150);
                        light = await light.GetAttributesAsync() as Light;
                        Assert.IsNotNull(light);
                        Assert.AreEqual(light.State.Hue, newState.Hue);
                        Assert.AreEqual(light.State.Brightness, newState.Brightness);
                        Assert.AreEqual(light.State.Saturation, newState.Saturation);
                        Assert.AreEqual(light.State.ColorMode, "hs");
                        break;
                    case "xy":
                        newState = new State { CIEColor = rv.Value.CIEColor, On = rv.Value.On };
                        Assert.IsInstanceOfType(await light.SetStateAsync(newState),
                                                typeof(HueObjectCollectionBase<Success>));
                        System.Threading.Thread.Sleep(150);
                        light = await light.GetAttributesAsync() as Light;
                        Assert.IsNotNull(light);
                        Assert.AreEqual(light.State.CIEColor[0], newState.CIEColor[0]);
                        Assert.AreEqual(light.State.CIEColor[1], newState.CIEColor[1]);
                        Assert.AreEqual(light.State.ColorMode, "xy");
                        break;
                    case "ct":
                        newState = new State { ColorTemperature = rv.Value.ColorTemperature, On = rv.Value.On };
                        Assert.IsInstanceOfType(await light.SetStateAsync(newState),
                                                typeof(HueObjectCollectionBase<Success>));
                        System.Threading.Thread.Sleep(150);
                        light = await light.GetAttributesAsync() as Light;
                        Assert.IsNotNull(light);
                        Assert.AreEqual(light.State.ColorTemperature, newState.ColorTemperature);
                        Assert.AreEqual(light.State.ColorMode, "ct");
                        break;
                }
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
