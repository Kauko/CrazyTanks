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
        public float ShieldMeter {
            get { return ShieldMeter; } 
            set { 
                if (ShieldMeter < 1.0f) 
                    ShieldMeter += value;
                if (ShieldMeter > 1.0f)
                    ShieldMeter = 1.0f;
                } 
        }
        public float health = 1.0f;

        public ShieldState shieldstate;
        public Weapon currentWeapon;
        public List<Weapon> weapons;

        public bool throttling = true;
        public bool usedShield = false;

        public Teams Team { get; private set; }



        public Tank(Teams team, Vector2 position, ControlSide side)
        {
            pos = position;
            GameServices.GetService<Logger>().logMsg("" + pos);
            dir = Vector2.Zero;
            center = new Vector2(TextureRefs.tank.Width / 2, TextureRefs.tank.Height / 2);
            this.Team = team;

            controls = new TankControls(side);

            weapons = Enum.GetValues(typeof(Weapon)).Cast<Weapon>().ToList<Weapon>();

            currentWeapon = Weapon.Shield;
            shieldstate = ShieldState.Off;
            pad = GameServices.GetService<GamepadDevice>();
            rotation = 0.0f;
            turretRotation = 0.0f;
        }

        public void Move(Vector2 stickOffset, Vector2 delta)
        {
            float stickrotation = (float)Math.Atan2(stickOffset.X, stickOffset.Y);

            stickrotation = MathHelper.WrapAngle(stickrotation);

            rotation = MathHelper.WrapAngle(rotation);
            
            float d = 0f;
            int cw = 0;

            if(stickrotation > rotation)
            {
                d = stickrotation - rotation;
                cw = 1;
            }
            else
            {
                d = rotation - stickrotation;
                cw = -1;
            }

            if (stickOffset != Vector2.Zero)
            {
                if (d < MathHelper.Pi)
                {
                    if (throttling)
                    {
                        rotation += cw * C.tankRotationSpeed;
                        turretRotation += cw * C.tankRotationSpeed;
                    }
                    else
                    {
                        rotation -= cw * C.tankRotationSpeed;
                        turretRotation -= cw * C.tankRotationSpeed;
                    }
                }
                if( d > MathHelper.Pi)
                {
                    if (throttling)
                    {
                        rotation -= cw * C.tankRotationSpeed;
                        turretRotation -= cw * C.tankRotationSpeed;
                    }
                    else
                    {
                        rotation += cw * C.tankRotationSpeed;
                        turretRotation += cw * C.tankRotationSpeed;
                    }
                }

                Vector2 up = new Vector2(0, -1);
                Matrix rotMatrix = Matrix.CreateRotationZ(rotation);

                up = new Vector2(0, -1);
                rotMatrix = Matrix.CreateRotationZ(rotation);
                dir = Vector2.Transform(up, rotMatrix);

                pos.X += dir.X * C.tankThrottleSpeed;
                pos.Y += dir.Y * C.tankThrottleSpeed;
            }
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
            if (controls.darkside == ControlSide.Left)
            {
                if(pad.WasButtonPressed(controls.reverse)){
                    if (throttling)
                    {
                        throttling = false;
                        rotation -= MathHelper.Pi;
                    }
                    else
                    {
                        throttling = true;
                        rotation += MathHelper.Pi;
                    }
                }
                Move(pad.LeftStickPosition, pad.LeftStickDelta);
            }
            if (controls.darkside == ControlSide.Right)
            {
                Move(pad.RightStickPosition, pad.RightStickDelta);
            }

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
            Vector2 up = new Vector2(0, -1);
            Matrix rotMatrix = Matrix.CreateRotationZ(turretRotation);
            
            GameServices.GetService<BulletManager>().addBullet(new Bullet(this, Vector2.Transform(up, rotMatrix), pos - center));
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

            float drawrotation = rotation;
            if (!throttling)
            {
                drawrotation = rotation + MathHelper.Pi;
            }

            spriteBatch.Draw(TextureRefs.tank, pos, null, Color.White, drawrotation, center, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureRefs.turret, pos, null, Color.White, turretRotation, center, 1.0f, SpriteEffects.None, 0f);
            if (shieldstate == ShieldState.On)
            {
                spriteBatch.Draw(TextureRefs.shield, pos, null, Color.White, rotation, center, 1.0f, SpriteEffects.None, 0f);
            }
            //Draw healthbar
            var rect = new Texture2D(GameServices.GetService<GraphicsDevice>(), 1, 1);
            rect.SetData(new[] { Color.DeepPink });
            float width = C.healthbarWidth * this.health;
            spriteBatch.Draw(rect, new Rectangle((int)pos.X, (int)pos.Y + C.healthbarHorizontalOffset, (int) width, C.healthbarHeight), Color.White * 0.5f);

        }

        public RotatedRectangle getRotatedRectangle()
        {
            return new RotatedRectangle(getRectangle(), this.rotation);
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)pos.X - TextureRefs.tank.Width / 2, (int)pos.Y - TextureRefs.tank.Height / 2, TextureRefs.tank.Width, TextureRefs.tank.Height);
        }

        internal void Collide()
        {
            GameServices.GetService<Logger>().logMsg("collide()");
            //throw new NotImplementedException();
        }

        internal void CollectSmartBomb()
        {
            GameServices.GetService<Logger>().logMsg("Tank.collectSmartbomb()");
            //throw new NotImplementedException();
        }

        internal void takeDamage(Bullet b)
        {
            if (this.shieldstate == ShieldState.On)
            {

            }
            else
            {
                GameServices.GetService<Logger>().logMsg("Tank.takeDamage()");
                this.health -= C.bulletDamage;
            }
            if (this.health <= 0.0f)
                Die();
            
        }

        private void Die()
        {
            GameServices.GetService<Logger>().logMsg("DIE");
            this.health = 1.0f;
        }
    }
}
