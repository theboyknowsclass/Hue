using System;

namespace TheBoyKnowsClass.Hue.Common.Models.BaseClasses
{
    public abstract class HueConnectedObjectBase : HueObjectBase
    {
        protected readonly HueConnection Context;


        protected HueConnectedObjectBase(HueConnection context, string id) : base(id)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context",@" cannot be null for Connected Object");
            }

            Context = context;
        }

        protected HueConnectedObjectBase(string id) : base(id)
        {

        }

        protected HueConnectedObjectBase() : base()
        {
            
        }
    }
}
