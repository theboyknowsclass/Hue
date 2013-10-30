namespace TheBoyKnowsClass.Hue.UI.Common.Models
{
    public class TypePointValueMapping<T> 
    {
        public TypePointValueMapping(double point, T type, int value)
        {
            Point = point;
            Type = type;
            Value = value;
        }

        public double Point { get; set; }
        public T Type { get; set; }
        public int Value { get; set; }
    }
}
