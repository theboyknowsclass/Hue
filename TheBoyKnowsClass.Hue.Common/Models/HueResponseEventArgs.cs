using TheBoyKnowsClass.Hue.Common.Models.BaseClasses;

namespace TheBoyKnowsClass.Hue.Common.Models
{
    public class HueResponseEventArgs : System.EventArgs
    {
        public string Path { get; set; }
        public HueObjectBase Response { get; set; }
    }
}
