namespace TheBoyKnowsClass.Hue.UI.Common.Models
{
    public class ColourPointValueMapping : TypePointValueMapping<Colour>
    {
        public ColourPointValueMapping(double point, Colour type, int value)
            : base(point, type, value)
        {
        }

        public Colour Colour { get { return Type; } set { Type = value; } }
    }
}
