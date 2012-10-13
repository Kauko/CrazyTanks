using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Solum.Utility;

namespace Solum.SharedTanks
{
    class SmartBombPickup : SpawnObject
    {
        public SmartBombPickup()
        {
            this.Type = StaticType.Smartbomb;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(TextureRefs.SmartBombPickup, position, Color.White);
        }
    }
}
