using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Solum.SharedTanks
{
    public enum StaticType
    {
        Smartbomb,
        Wall,
        Empty,
    }
    abstract class StaticWorldObject : WorldObject
    {
        public StaticType Type { get; protected set; }
    
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position){
        }
    }

}
