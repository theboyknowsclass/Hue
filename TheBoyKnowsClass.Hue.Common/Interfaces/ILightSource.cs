using System.Threading.Tasks;
using TheBoyKnowsClass.Hue.Common.Models;
using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Interfaces
{
    public interface ILightSource
    {
        string ID { get; }
        State State { get; set; }
        string Name { get; set; }

        Task<HueObjectBase> SetNameAsync<T>(string newName)
            where T : ILightSource, new();
    }
}