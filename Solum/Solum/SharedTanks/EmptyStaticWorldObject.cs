using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solum.SharedTanks
{
    class EmptyStaticWorldObject : StaticWorldObject
    {
        public EmptyStaticWorldObject()
        {
            this.Type = StaticType.Empty;
        }
    }
}
