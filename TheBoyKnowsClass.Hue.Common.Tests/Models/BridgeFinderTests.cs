using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Tests.Models
{
    [TestClass]
    public class BridgeFinderTests
    {
        [TestMethod]
        public async Task GetBridgesTest()
        {
            var rv = await GetBridges();

            Assert.IsInstanceOfType(rv, typeof(HueObjectCollectionBase<Bridge>));
        }

        #region helper method

        public static async Task<HueObjectCollectionBase<Bridge>> GetBridges()
        {
            var bridgeFinder = new BridgeFinder();
            var connection = new HueConnection {Throttle = true};
            return (HueObjectCollectionBase<Bridge>)await bridgeFinder.GetBridgesAsync(connection);
        }

        public static async Task<Bridge> GetFirstBridge()
        {
            var bridge = (await GetBridges()).FirstOrDefault();
            Assert.IsNotNull(bridge.ID);
            return bridge;
        }

        #endregion
    }
}
