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
    public enum ShieldState
    {
        On,
        Off
    }

    public enum Weapon
    {
        Shield,
        Cannon,
        SmartBomb
    }

    class Tank : MovingObject
    {
        public Vector2 pos;
        protected Vector2 center;
        protected Vector2 dir;

        protected float rotation;
        protected float turretRotation;
        protected GamepadDevice pad;

        public TankControls controls;

        public ShieldState shieldstate;
        public Weapon currentWeapon;
        public List<Weapon> weapons;

        public bool throttling = true;
        public bool usedShield = false;

        public Bullet bullet;
        public Teams Team { get; private set; }



        public Tank(Teams team, Vector2 position)
        {
            pos = position;
            dir = Vector2.Zero;
            center = new Vector2(TextureRefs.tank.Width / 2, TextureRefs.tank.Height / 2);
            this.Team = team;
            bullet = new Bullet();

            controls = new TankControls(ControlSide.Left);

            weapons = Enum.GetValues(typeof(Weapon)).Cast<Weapon>().ToList<Weapon>();

            foreach (Weapon w in weapons)
            {
                GameServices.GetService<Logger>().logMsg(w.ToString());
            }

            currentWeapon = Weapon.Shield;
            shieldstate = ShieldState.Off;
            pad = GameServices.GetService<GamepadDevice>();
            rotation = 0.0f;
            turretRotation = 0.0f;
        }

        public void Move(Vector2 stickOffset, Vector2 delta)
        {
            float stickrotation = (float)Math.Atan2(stickOffset.X, stickOffset.Y);

            //stickrotation += MathHelper.PiOver2;

            stickrotation = MathHelper.WrapAngle(stickrotation);

            rotation = MathHelper.WrapAngle(rotation);
            
            //GameServices.GetService<Logger>().logMsg(stickrotation.ToString());
            //GameServices.GetService<Logger>().logMsg(rotation.ToString());

            //float d = 0f;

           /* d = stickrotation - rotation;

            if(stickrotation > rotation)
            {
                d = stickrotation - rotation;
            }
            else
            {
                d = rotation - stickrotation;  
            }*/

            //GameServices.GetService<Logger>().logMsg(d.ToString());

            if (stickOffset != Vector2.Zero)
            {
                rotation = stickrotation;

                Vector2 up = new Vector2(0, -1);
                Matrix rotMatrix = Matrix.CreateRotationZ(rotation);
                dir = Vector2.Transform(up, rotMatrix);

                pos += dir * C.tankThrottleSpeed;
            }

            //bullet.SetDirection(Vector2.Transform(up, rotMatrix));


            /*if (d < MathHelper.Pi)
            {
                if (d >= MathHelper.PiOver2 && d < MathHelper.Pi)
                {
                    rotation += C.tankRotationSpeed;
                    pos.X += stickOffset.X * C.tankThrottleSpeed;
                    pos.Y -= stickOffset.Y * C.tankThrottleSpeed;

                }
                else if (d < MathHelper.PiOver2 && d >= 0)
                {
                    rotation -= C.tankRotationSpeed;
                    pos.X += stickOffset.X * C.tankThrottleSpeed;
                    pos.Y -= stickOffset.Y * C.tankThrottleSpeed;

                }
            }
            /*else{
                if(throttling == true)
                    throttling = false;
                else
                    throttling = true;

                if (d >= MathHelper.Pi && d <= MathHelper.Pi + MathHelper.PiOver2)
                {
                    rotation -= C.tankRotationSpeed;
                    pos.X -= stickOffset.X * C.tankThrottleSpeed;
                    pos.Y += stickOffset.Y * C.tankThrottleSpeed;

                }
                else if (d > MathHelper.Pi + MathHelper.PiOver2 && d <= MathHelper.TwoPi)
                {
                    rotation += C.tankRotationSpeed;
                    pos.X -= stickOffset.X * C.tankThrottleSpeed;
                    pos.Y += stickOffset.Y * C.tankThrottleSpeed;
                }
            }
             * */

        }

        public void Rotate(Vector2 stickOffset)
        {
            //rotation += stickOffset.X * 
        }

        public void RotateTurret(float trot)
        {
            turretRotation += trot;
            turretRotation = MathHelper.WrapAngle(turretRotation);
        }

        public void SetShieldState(ShieldState state)
        {
            shieldstate = state;
        }


        public override void Update()
        {
            bullet.Update();

            Move(pad.LeftStickPosition, pad.LeftStickDelta);

            if (pad.IsButtonDown(controls.turretRotateCW))
            {
                RotateTurret(-C.turretRotationSpeed);
            }
            else if (pad.IsButtonDown(controls.turretRotateCCW))
            {
                RotateTurret(C.turretRotationSpeed);
            }

            if (pad.WasButtonPressed(controls.shoot))
            {
                switch (currentWeapon)
                {
                    case Weapon.Cannon:
                        Shoot();
                        break;
                    case Weapon.Shield:
                        usedShield = true;
                        break;
                    case Weapon.SmartBomb:
                        break;
                }
            }
            if (pad.WasButtonPressed(controls.changeWeapon))
            {
                NextWeapon();
            }
        }

        public void Shoot()
        {
            bullet.SetPosition(pos - center);

            Vector2 up = new Vector2(0, -1);
            Matrix rotMatrix = Matrix.CreateRotationZ(turretRotation);

            bullet.SetDirection(Vector2.Transform(up, rotMatrix));
        }

        public void NextWeapon()
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i] == currentWeapon)
                {
                    if (i + 1 == weapons.Count)
                    {
                        i = -1;
                    }
                    currentWeapon = weapons[i + 1];
                    break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureRefs.tank, pos, null, Color.White, rotation, center, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureRefs.turret, pos + center - new Vector2(TextureRefs.turret.Width / 2, TextureRefs.turret.Height / 2), null, Color.White, turretRotation, center, 1.0f, SpriteEffects.None, 0f);
            if (shieldstate == ShieldState.On)
            {
                spriteBatch.Draw(TextureRefs.shield, pos + center - new Vector2(TextureRefs.shield.Width / 2, TextureRefs.shield.Height / 2), null, Color.White, rotation, new Vector2(TextureRefs.shield.Width / 2, TextureRefs.shield.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(TextureRefs.bullet, bullet.pos, Color.White);
        }

        public RotatedRectangle getRectangle()
        {
            return new RotatedRectangle(new Rectangle((int)pos.X, (int)pos.Y, TextureRefs.tank.Width, TextureRefs.tank.Height), this.rotation);
        }
    }
}