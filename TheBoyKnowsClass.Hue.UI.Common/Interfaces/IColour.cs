namespace TheBoyKnowsClass.Hue.UI.Common.Interfaces
{
    public interface IColour<T>
    {
        byte R { get; set; }
        byte G { get; set; }
        byte B { get; set; }
        byte A { get; set; }

        T GetColour { get; }
    }
}