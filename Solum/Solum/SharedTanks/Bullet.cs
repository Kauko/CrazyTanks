﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Solum.Utility;
using Solum.Input;
using Solum.Logging;

namespace Solum.SharedTanks
{
    class Bullet : MovingObject
    {
        public Vector2 pos;
        public Vector2 dir;

        public Bullet()
        {
            pos = Vector2.Zero;
            dir = Vector2.Zero;
        }

        public void SetDirection(Vector2 direction)
        {
            dir = direction;
        }

        public void SetPosition(Vector2 position)
        {
            pos = position;
        }

        public override void Update()
        {
            pos.X = pos.X + dir.X * C.bulletSpeed;
            pos.Y = pos.Y + dir.Y * C.bulletSpeed;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureRefs.bullet, pos, Color.White);
        }
        
    }
}
