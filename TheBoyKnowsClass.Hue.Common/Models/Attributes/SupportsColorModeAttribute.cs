using System;
using System.Collections.Generic;
using TheBoyKnowsClass.Hue.Common.Enumerations;

namespace TheBoyKnowsClass.Hue.Common.Models.Attributes
{
    public class SupportsColorModeAttribute : Attribute
    {
        public SupportsColorModeAttribute(params ColorMode[] modes)
        {
            Modes = modes;
        }

        public IEnumerable<ColorMode> Modes { get; private set; }
    }
}
