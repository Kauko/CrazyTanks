using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Solum.Utility;

namespace Solum.SharedTanks
{
    class Wall : StaticWorldObject
    {
        public Wall()
        {
            this.Type = StaticType.Wall;

        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(TextureRefs.Wall, position, Color.White);
        }
    }
}
