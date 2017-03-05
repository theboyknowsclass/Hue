using System.Threading.Tasks;
using TheBoyKnowsClass.Hue.Common.Enumerations;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;
using TheBoyKnowsClass.Hue.Common.Models.Factories;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public class BridgeFinder
    {
        public async Task<HueObjectBase> GetBridgesAsync(HueConnection connection)
        {
            // ToDo Implement UPnP code here

            return await GetBridgesFromPortalAPIAsync(connection);
        }

        private async Task<HueObjectBase> GetBridgesFromPortalAPIAsync(HueConnection connection)
        {
            string returnString = await connection.GetAsync(Resources.BridgeUPNPURI);
            return HueObjectFactory.CreateHueObject(returnString, connection, HueObjectType.Bridge) as HueObjectCollectionBase<Bridge> ?? (HueObjectBase)new Error(-1, "", "No Hue Bridge Found");
        }
    }
}
