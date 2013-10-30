using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TheBoyKnowsClass.Hue.UI.Common.Enumerations;
using TheBoyKnowsClass.Hue.UI.Common.Models;
using TheBoyKnowsClass.Hue.UI.Common.ViewModels;

namespace TheBoyKnowsClass.Hue.UI.Common.Helpers
{
    public static class ColourHelper
    {
        /// <summary>
        /// Collection of Points to allow for consistency with the UI and colour conversion
        /// </summary>
        public static ColourPointValueMappingsViewModel Hue = new ColourPointValueMappingsViewModel
        {
            new ColourPointValueMapping(0.0, FromARGB(255,255,0,0), 0),
            new ColourPointValueMapping(0.2, FromARGB(255,255,255,0), 12750),
            new ColourPointValueMapping(0.4, FromARGB(255,154,205,50), 25500),
            new ColourPointValueMapping(0.5, FromARGB(255,0,128,0), 36210),
            new ColourPointValueMapping(0.6, FromARGB(255,0,0,255), 46920),
            new ColourPointValueMapping(0.8, FromARGB(255,255,0,255), 56100),
            new ColourPointValueMapping(1.0, FromARGB(255,255,0,0), 65280),
        };

        public static ColourPointValueMappingsViewModel Temperature = new ColourPointValueMappingsViewModel
        {
            new ColourPointValueMapping(0.0, FromHex("#FFFEFA"), 153),
            new ColourPointValueMapping(1.0, FromHex("#FF890E"), 500)
        };

        public static ColourPointValueMappingsViewModel Saturation = new ColourPointValueMappingsViewModel
        {
            new ColourPointValueMapping(1.0, FromHex("#FFFFFFFF"), 0),
            new ColourPointValueMapping(0.5, FromHex("#66FFFFFF"), 102),
            new ColourPointValueMapping(0.0, FromHex("#00FFFFFF"), 255),
        };

        public static string GetColorMode(SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.HSB:
                    return "hs";
                case SceneType.ColourTemperature:
                    return "ct";
                case SceneType.Image:
                    return "hs";
                default:
                    throw new ArgumentOutOfRangeException("sceneType");
            }
        }

        #region Colour Temperature Helpers

        public static double? GetXFromMirek(int? mirek)
        {
            return ConvertToPoint(Temperature, mirek);
        }

        public static int? GetMirekFromX(double? x)
        {
            return ConvertToValue(Temperature, x);
        }

        public static int? GetKelvinFromMirek(int? mirek)
        {
            return (100000 / mirek);
        }

        private static Colour GetRGBFromMirek(int? mirek)
        {
            return !mirek.HasValue ? new Colour() : GetRGBFromTemperature(1000000 / mirek.Value);
        }

        private static Colour GetRGBFromTemperature(long kelvin)
        {
            double r;
            double g;
            double b;

            kelvin = kelvin / 100;


            if (kelvin <= 66)
            {
                r = 255;
            }
            else
            {
                //Note: the R-squared value for this approximation is .988
                r = 329.698727446 * (Math.Pow(kelvin - 60, -0.1332047592));
                if (r < 0)
                {
                    r = 0;
                }
                if (r > 255)
                {
                    r = 255;
                }
            }


            //Second: green
            if (kelvin <= 66)
            {
                //Note: the R-squared value for this approximation is .996
                g = 99.4708025861 * Math.Log(kelvin) - 161.1195681661;
                if (g < 0) g = 0;
                if (g > 255) g = 255;
            }
            else
            {
                //Note: the R-squared value for this approximation is .987
                g = 288.1221695283 * (Math.Pow(kelvin - 60, -0.0755148492));
                if (g < 0)
                {
                    g = 0;
                }
                if (g > 255)
                {
                    g = 255;
                }
            }

            //Third: blue
            if (kelvin >= 66)
            {
                b = 255;
            }
            else
            {
                if (kelvin <= 19)
                {
                    b = 0;
                }
                else
                {
                    //Note: the R-squared value for this approximation is .998
                    b = 138.5177312231 * Math.Log(kelvin - 10) - 305.0447927307;
                    if (b < 0) { b = 0; }
                    if (b > 255) { b = 255; }
                }
            }

            return FromARGB(255, (byte)(int)r, (byte)(int)g, (byte)(int)b);
        }

        public static Colour GetRGBFromMirekAndSaturation(int? mirek, int? saturation)
        {
            int sat = saturation ?? 255;

            Colour hue = GetRGBFromMirek(mirek);
            hue.A = (byte)sat;

            return RemoveAlpha(hue, FromARGB(255, 255, 255, 255));
        }

        #endregion

        #region HSB Helpers

        public static int? GetHueFromX(double? x)
        {
            return ConvertToValue(Hue, x);
        }

        public static double? GetXFromHue(int? hue)
        {
            return ConvertToPoint(Hue, hue);
        }

        private static Colour GetRGBFromX(double? x)
        {
            return ConvertToColor(Hue, x);
        }

        public static Colour GetRGBFromXAndSaturation(double? x, int? saturation)
        {
            int sat = saturation ?? 255;

            Colour hue = GetRGBFromX(x);
            hue.A = (byte)sat;

            return RemoveAlpha(hue, FromARGB(255, 255, 255, 255));
        }

        #endregion

        #region Saturation Helpers

        public static double? GetYFromSaturation(int? saturation)
        {
            return ConvertToPoint(Saturation, saturation);
        } 

        public static int? GetSaturationFromY(double? y)
        {
            return ConvertToValue(Saturation, y);
        }

        #endregion

        #region Image Helpers

        //public static int GetHue(this Color color)
        //{
        //    return (int)color.GetDrawingColor().GetHue() * 65280 / 360;
        //}

        //public static int GetSaturation(this Color color)
        //{
        //    return (int)color.GetDrawingColor().GetSaturation() * 255;
        //}

        //public static int GetBrightness(this Color color)
        //{
        //    return (int)color.GetDrawingColor().GetBrightness() * 255;
        //}

        #endregion

        #region Helper Methods

        //public static System.Drawing.Color GetDrawingColor(this Color color)
        //{
        //    return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        //}

        private static double? ConvertToPoint(IEnumerable<ColourPointValueMapping> points, int? x)
        {
            if (x.HasValue)
            {
                var enumerated = points.ToList();
                x = FixValue(enumerated, x);

                ColourPointValueMapping smaller = GetSmallerValue(enumerated, x);
                if (smaller.Value == x) return smaller.Point;
                ColourPointValueMapping larger = GetLargerValue(enumerated, x);
                if (smaller.Value == x) return larger.Point;

                return ((larger.Point - smaller.Point) * (x.Value - smaller.Value) / (larger.Value - smaller.Value)) + smaller.Point;
            }
            return null;
        }

        private static int? ConvertToValue(IEnumerable<ColourPointValueMapping> points, double? x)
        {
            if (x.HasValue)
            {
                var enumerated = points.ToList();
                x = FixPoint(enumerated, x);

                ColourPointValueMapping smaller = GetSmallerPoint(enumerated, x);
                if (smaller.Point == x) return smaller.Value;
                ColourPointValueMapping larger = GetLargerPoint(enumerated, x);
                if (smaller.Point == x) return larger.Value;

                return (int)((larger.Value - smaller.Value) * (x.Value - smaller.Point) / (larger.Point - smaller.Point)) + smaller.Value;
            }
            return null;
        }

        private static Colour ConvertToColor(IEnumerable<ColourPointValueMapping> points, double? x)
        {
            var hue = new Colour();

            if (x.HasValue)
            {
                var enumerated = points.ToList();
                x = FixPoint(enumerated, x);

                ColourPointValueMapping smaller = GetSmallerPoint(enumerated, x);
                if (smaller.Point == x)
                {
                    return smaller.Colour;
                }
                ColourPointValueMapping larger = GetLargerPoint(enumerated, x);
                if (smaller.Point == x)
                {
                    return larger.Colour;
                }

                hue.A = (byte)((int)((larger.Colour.A - smaller.Colour.A) * (x.Value - smaller.Point) / (larger.Point - smaller.Point)) + smaller.Colour.A);
                hue.R = (byte)((int)((larger.Colour.R - smaller.Colour.R) * (x.Value - smaller.Point) / (larger.Point - smaller.Point)) + smaller.Colour.R);
                hue.G = (byte)((int)((larger.Colour.G - smaller.Colour.G) * (x.Value - smaller.Point) / (larger.Point - smaller.Point)) + smaller.Colour.G);
                hue.B = (byte)((int)((larger.Colour.B - smaller.Colour.B) * (x.Value - smaller.Point) / (larger.Point - smaller.Point)) + smaller.Colour.B);
            }

            return hue;
        }

        private static Colour ConvertToColor(IEnumerable<ColourPointValueMapping> points, int? x)
        {
            var hue = new Colour();

            if (x.HasValue)
            {
                var enumerated = points.ToList();
                x = FixValue(enumerated, x);

                ColourPointValueMapping smaller = GetSmallerValue(enumerated, x);
                if (smaller.Value == x)
                {
                    return smaller.Colour;
                }
                ColourPointValueMapping larger = GetLargerValue(enumerated, x);
                if (larger.Value == x)
                {
                    return larger.Colour;
                }

                hue.A = (byte)((larger.Colour.A - smaller.Colour.A) * (x.Value - smaller.Value) / (larger.Value - smaller.Value) + smaller.Colour.A);
                hue.R = (byte)((larger.Colour.R - smaller.Colour.R) * (x.Value - smaller.Value) / (larger.Value - smaller.Value) + smaller.Colour.R);
                hue.G = (byte)((larger.Colour.G - smaller.Colour.G) * (x.Value - smaller.Value) / (larger.Value - smaller.Value) + smaller.Colour.G);
                hue.B = (byte)((larger.Colour.B - smaller.Colour.B) * (x.Value - smaller.Value) / (larger.Value - smaller.Value) + smaller.Colour.B);
            }

            return hue;
        }

        private static int FixValue(IEnumerable<ColourPointValueMapping> points, int? x)
        {
            if (x.HasValue)
            {
                var colourPointValueMappings = points as IList<ColourPointValueMapping> ?? points.ToList();
                var max = (from p in colourPointValueMappings select p.Value).Max();
                var min = (from p in colourPointValueMappings select p.Value).Min();

                return Math.Min(Math.Max(min, x.Value), max);
            }
            return 0;
        }

        private static double FixPoint(IEnumerable<ColourPointValueMapping> points, double? x)
        {
            if (x.HasValue)
            {
                var colourPointValueMappings = points as IList<ColourPointValueMapping> ?? points.ToList();
                var max = (from p in colourPointValueMappings select p.Point).Max();
                var min = (from p in colourPointValueMappings select p.Point).Min();

                return Math.Min(Math.Max(min, x.Value), max);
            }
            return 0;
        }

        private static ColourPointValueMapping GetSmallerPoint(IEnumerable<ColourPointValueMapping> points, double? point)
        {
            return (from p in points
                    where p.Point <= point
                    orderby p.Point descending
                    select p).FirstOrDefault();
        }

        private static ColourPointValueMapping GetSmallerValue(IEnumerable<ColourPointValueMapping> points, int? value)
        {
            return (from p in points
                    where p.Value <= value
                    orderby p.Value descending
                    select p).FirstOrDefault();
        }

        private static ColourPointValueMapping GetLargerPoint(IEnumerable<ColourPointValueMapping> points, double? value)
        {
            return (from p in points
                    where p.Point >= value
                    orderby p.Point ascending 
                    select p).FirstOrDefault();
        }

        private static ColourPointValueMapping GetLargerValue(IEnumerable<ColourPointValueMapping> points, int? value)
        {
            return (from p in points
                    where p.Value >= value
                    orderby p.Value ascending
                    select p).FirstOrDefault();
        }

        private static Colour RemoveAlpha(Colour foreground, Colour background)
        {
            if (foreground.A == 255)
                return foreground;

            var alpha = foreground.A / 255.0;
            var diff = 1.0 - alpha;
            return FromARGB(255,
                (byte)(foreground.R * alpha + background.R * diff),
                (byte)(foreground.G * alpha + background.G * diff),
                (byte)(foreground.B * alpha + background.B * diff));
        }

        public static Colour FromARGB(byte a, byte r, byte g, byte b)
        {
            return new Colour(a,r,g,b);
        }

        public static Colour FromHex(string hexValue)
        {
            byte a, r, g, b;

            if (hexValue.Length == 7)
            {
                a = 255;
                r = byte.Parse(hexValue.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                g = byte.Parse(hexValue.Substring(3, 2), NumberStyles.AllowHexSpecifier);
                b = byte.Parse(hexValue.Substring(5, 2), NumberStyles.AllowHexSpecifier);
            }
            else if (hexValue.Length == 9)
            {
                a = byte.Parse(hexValue.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                r = byte.Parse(hexValue.Substring(3, 2), NumberStyles.AllowHexSpecifier);
                g = byte.Parse(hexValue.Substring(5, 2), NumberStyles.AllowHexSpecifier);
                b = byte.Parse(hexValue.Substring(7, 2), NumberStyles.AllowHexSpecifier);
            }
            else
            {
                throw new FormatException("Not a valid hex format"); 
            }

            return FromARGB(a, r, g, b);
        }

        //public static string ToHex(this IColour colour)
        //{
        //    return String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", colour.A, colour.R, colour.G, colour.B);
        //}

        #endregion
    }
}
