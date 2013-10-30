namespace TheBoyKnowsClass.Hue.Common.Models.BaseClasses
{
    public abstract class HueObjectBase
    {
        protected readonly string ID;

        protected HueObjectBase()
        { 
        }

        protected HueObjectBase(string id) : this()
        {
            ID = id;
        }
    }
}
