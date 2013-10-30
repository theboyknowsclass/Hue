namespace TheBoyKnowsClass.Hue.UI.Common.Models
{
    public class Colour
    {
        public Colour()
        {
        }

        public Colour(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public byte R { get; set; }

        public byte G { get; set; }

        public byte B { get; set; }

        public byte A { get; set; }
    }
}
