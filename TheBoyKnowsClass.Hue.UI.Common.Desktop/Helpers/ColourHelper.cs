using System.Windows.Media;
using TheBoyKnowsClass.Hue.UI.Common.Models;

namespace TheBoyKnowsClass.Hue.UI.Common.Desktop.Helpers
{
    public static class ColourHelper
    {
        public static Color ToColor(this Colour colour)
        {
            return Color.FromArgb(colour.A, colour.R, colour.G, colour.B);
        }

        public static Colour ToColour(this Color color)
        {
            return new Colour(color.A, color.R, color.G, color.B);
        }
    }
}
