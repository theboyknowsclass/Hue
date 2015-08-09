using TheBoyKnowsClass.Common.Attributes;
using TheBoyKnowsClass.Hue.Common.Models.Attributes;

namespace TheBoyKnowsClass.Hue.Common.Enumerations
{
    public enum LightType
    {
        [Description("Extended color light")]
        [SupportsColorMode(ColorMode.ColorTemperature, ColorMode.HueSaturation, ColorMode.XY)]
        ExtendedColorLight,
        [Description("Color light")]
        [SupportsColorMode(ColorMode.HueSaturation, ColorMode.XY)]
        ColorLight,
        [Description("Dimmable plug-in unit")]
        DimmablePlug,
        [Description("Dimmable light")]
        DimmableLight,

    }
}
