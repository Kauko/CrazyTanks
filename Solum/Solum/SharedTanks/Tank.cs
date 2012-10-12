using System;
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

    enum ControlSide
    {
        Right,
        Left
    }

    class Tank : MovingObject
    {
        public Vector2 pos;
        protected Vector2 center;
        protected float rotation;
        protected GamepadDevice pad;
        public ControlSide side;

        public Tank()
        {
            pos = Vector2.Zero;
            center = Vector2.Zero;
            side = ControlSide.Left;
            pad = GameServices.GetService<GamepadDevice>();
            rotation = 0.0f;
        }

        public void Move(Vector2 stickOffset, Vector2 delta)
        {
            //pos += stickOffset * C.tankThrottleSpeed;
            float stickrotation = (float)Math.Atan2(stickOffset.X, stickOffset.Y);
            stickrotation += MathHelper.Pi;

            float d = 0f;

            if(stickrotation > rotation)
            {
                d = stickrotation - rotation;
            }
            else
            {
                d = rotation - stickrotation;  
            }

            if (d > MathHelper.Pi && d < (2 * MathHelper.Pi) - 0.2f)
            {
                rotation += C.tankRotationSpeed;
            }
            else if (d < MathHelper.Pi && d > 0.2f) 
            {
                rotation -= C.tankRotationSpeed;
            }

            //Vector2 direction = Vector2.Zero - stickOffset;
            //direction.Normalize();
            //float angle = (float)(Math.Atan2(direction.X, direction.Y)) - MathHelper.Pi;

            //if(angle 

            //rotation = angle;

        }

        public void Rotate(Vector2 stickOffset)
        {
            //rotation += stickOffset.X * 
        }

        public override void Update()
        {
            center.X = TextureRefs.tank.Width / 2;
            center.Y = TextureRefs.tank.Height / 2;

            if (side == ControlSide.Left)
            {
                Move(pad.LeftStickPosition, pad.LeftStickDelta);
            }
            else
            {
                Move(pad.RightStickPosition, pad.RightStickDelta);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureRefs.tank, pos, null, Color.White, rotation, center, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
