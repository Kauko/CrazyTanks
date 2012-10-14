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
        Cannon
        //SmartBomb
    }

    public enum TankState
    {
        Spawning,
        Alive,
        Dead
    }

    public enum TurretState
    {
        Shooting,
        Ready
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
        public float ShieldMeter = 0.0f;
        public float health = 1.0f;
        public float speed;
        public float lastRotation;

        public float shieldDurRemaining;
        public float respawnCounter;
        public float reloadCounter;

        public ShieldState shieldstate;
        public Weapon currentWeapon;
        public List<Weapon> weapons;
        public TankState state;
        public TankState partnerState;
        public ShieldState partnerShieldState;
        public TurretState turretState;

        public bool throttling;
        public bool usedShield;

        public Teams Team { get; private set; }
        SoundEffectInstance shieldSound;



        public Tank(Teams team, Vector2 position, float rotation, GamepadDevice gpad, ControlSide side)
        {
            /* No need to reset these -> not to initialize() */
            controls = new TankControls(side);
            weapons = Enum.GetValues(typeof(Weapon)).Cast<Weapon>().ToList<Weapon>();
            center = new Vector2(TextureRefs.tank.Width / 2, TextureRefs.tank.Height / 2);
            this.Team = team;
            pad = gpad;
            shieldSound = SoundRefs.shieldOn.CreateInstance();
            shieldSound.IsLooped = true;
            partnerState = TankState.Alive;
            partnerShieldState = ShieldState.On;
            /* see descr */

            Initialize(position, rotation);
        }

        public void Initialize(Vector2 position, float rot)
        {
            pos = position;
            dir = Vector2.Zero;

            currentWeapon = Weapon.Cannon;
            state = TankState.Alive;
            turretState = TurretState.Ready;

            shieldDurRemaining = 5;
            SetShieldState(ShieldState.On);

            respawnCounter = C.respawnTime;
            reloadCounter = C.reloadTime;

            rotation = rot;
            turretRotation = rot;
            speed = C.tankThrottleSpeed;
            health = 1.0f;

            throttling = true;
            usedShield = false;
        }

        public void Move(Vector2 stickOffset, Vector2 delta)
        {
            float stickrotation = (float)Math.Atan2(stickOffset.X, stickOffset.Y);

            stickrotation = MathHelper.WrapAngle(stickrotation);

            rotation = MathHelper.WrapAngle(rotation);
            lastRotation = rotation;
            
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
                        if(turretState == TurretState.Ready){
                            turretRotation += cw * C.tankRotationSpeed;
                        }
                    }
                    else
                    {
                        rotation -= cw * C.tankReverseRotationSpeed;
                        if (turretState == TurretState.Ready)
                        {
                            turretRotation -= cw * C.tankReverseRotationSpeed;
                        }
                    }
                }
                if( d > MathHelper.Pi)
                {
                    if (throttling)
                    {
                        rotation -= cw * C.tankRotationSpeed;
                        if (turretState == TurretState.Ready)
                        {
                            turretRotation -= cw * C.tankRotationSpeed;
                        }
                    }
                    else
                    {
                        rotation += cw * C.tankReverseRotationSpeed;
                        if (turretState == TurretState.Ready)
                        {
                            turretRotation += cw * C.tankReverseRotationSpeed;
                        }
                    }
                }

                Vector2 up = new Vector2(0, -1);
                Matrix rotMatrix = Matrix.CreateRotationZ(rotation);

                up = new Vector2(0, -1);
                rotMatrix = Matrix.CreateRotationZ(rotation);
                dir = Vector2.Transform(up, rotMatrix);

                pos.X += dir.X * speed;
                pos.Y += dir.Y * speed;
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
            if (shieldstate == ShieldState.On)
            {
                shieldDurRemaining = C.shieldDuration;
                shieldSound.Play();
            }
            else
            {
                shieldDurRemaining = 0f;
                shieldSound.Stop();
            }
        }


        public void Update(TankState pstate, ShieldState pshieldstate)
        {
            partnerState = pstate;
            partnerShieldState = pshieldstate;

            switch(state){
                case TankState.Spawning:
                    break;
                case TankState.Alive:
                    increaseShieldMeter(C.shieldMeterIncreaseOnUpdate);
                    UpdateAlive();
                    break;
                case TankState.Dead:
                    respawnCounter--;
                    if (respawnCounter < 0)
                    {
                        if(this.Team == Teams.red && G.redDeaths < C.allowedDeaths)
                            state = TankState.Spawning;
                        if (this.Team == Teams.blue && G.blueDeaths < C.allowedDeaths)
                            state = TankState.Spawning;
                        if (this.Team == Teams.green && G.greenDeaths < C.allowedDeaths)
                            state = TankState.Spawning;
                        if (this.Team == Teams.yellow && G.yellowDeaths < C.allowedDeaths)
                            state = TankState.Spawning;
                    }
                    break;
        }
        }

        public void UpdateAlive()
        {
            if (shieldDurRemaining > 0f && shieldstate == ShieldState.On)
            {
                shieldDurRemaining = shieldDurRemaining - 1f;

                if (shieldDurRemaining <= 0f)
                {
                    SetShieldState(ShieldState.Off);
                }
            }

            if (reloadCounter > 0f)
            {
                if (reloadCounter <= C.reloadTime - 1f)
                {
                    turretState = TurretState.Ready;
                }
                reloadCounter--;
            }

            if (pad.WasButtonPressed(controls.reverse))
            {
                if (throttling)
                {
                    throttling = false;
                    rotation -= MathHelper.Pi;
                    speed = C.tankReverseSpeed;
                }
                else
                {
                    throttling = true;
                    rotation += MathHelper.Pi;
                    speed = C.tankThrottleSpeed;
                }
            }

            if (controls.controlside == ControlSide.Left)
            {
                Move(pad.LeftStickPosition, pad.LeftStickDelta);
            }
            if (controls.controlside == ControlSide.Right)
            {
                Move(pad.RightStickPosition, pad.RightStickDelta);
            }

            if (pad.IsButtonDown(controls.turretRotateCW) && (turretState == TurretState.Ready))
            {
                RotateTurret(-C.turretRotationSpeed);
            }
            else if (pad.IsButtonDown(controls.turretRotateCCW) && (turretState == TurretState.Ready))
            {
                RotateTurret(C.turretRotationSpeed);
            }

            if (pad.WasButtonPressed(controls.shoot))
            {
                GameServices.GetService<Logger>().logMsg("AMMUTAAN SAATANA");
                switch (currentWeapon)
                {
                    case Weapon.Cannon:
                        GameServices.GetService<Logger>().logMsg("aseeni on kanuuna");
                        if (reloadCounter <= 0f)
                        {
                            Shoot();
                            turretState = TurretState.Shooting;
                        }
                        break;
                    case Weapon.Shield:
                        if (ShieldMeter == 1.0f && partnerState == TankState.Alive && partnerShieldState == ShieldState.Off)
                        {
                            usedShield = true;
                            ShieldMeter = 0.0f;
                            currentWeapon = Weapon.Cannon;
                        }
                        break;
                    //case Weapon.SmartBomb:
                      //  break;
                }
            }
            if (pad.WasButtonPressed(controls.changeWeapon))
            {
                if(currentWeapon != Weapon.Cannon || currentWeapon == Weapon.Cannon && ShieldMeter == 1.0f) 
                    NextWeapon();
            }
        }

        public void Shoot()
        {
            Vector2 up = new Vector2(0, -1);

            GameServices.GetService<Logger>().logMsg("ja anukseni valmiina");

            for (int i = -1; i < C.bulletsInShot - 1; i++)
            {
                Matrix rotMatrix = Matrix.CreateRotationZ(turretRotation - i * MathHelper.PiOver4 / 10 );
                Bullet bullet = new Bullet(this, Vector2.Transform(up, rotMatrix), pos - new Vector2(TextureRefs.bullet.Width / 2, TextureRefs.bullet.Height / 2));
                bullet.MoveToStartPoint(TextureRefs.turret.Height / 2 - 10);
                GameServices.GetService<BulletManager>().addBullet(bullet);
            }
            Random rand = new Random();
            if (rand.Next(10) != 1)
                SoundRefs.cannon.Play();
            else
                SoundRefs.cannonAlt.Play();
            reloadCounter = C.reloadTime;
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

            if (state == TankState.Alive)
            {
                spriteBatch.Draw(TextureRefs.tank, pos, null, Color.White, drawrotation, center, 1.0f, SpriteEffects.None, 0f);
                if(currentWeapon == Weapon.Cannon)
                    spriteBatch.Draw(TextureRefs.turret, pos, null, Color.White, turretRotation, center, 1.0f, SpriteEffects.None, 0f);
                if(currentWeapon == Weapon.Shield)
                    spriteBatch.Draw(TextureRefs.TurretShield, pos, null, Color.White, turretRotation, center, 1.0f, SpriteEffects.None, 0f);
                if (shieldstate == ShieldState.On)
                {
                    spriteBatch.Draw(TextureRefs.shield, pos, null, Color.White, rotation, center, 1.0f, SpriteEffects.None, 0f);
                }
            }else if(state == TankState.Dead)
                spriteBatch.Draw(TextureRefs.TankDead, pos, null, Color.White, drawrotation, center, 1.0f, SpriteEffects.None, 0f);
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
            pos -= dir * speed;
            rotation = lastRotation;
            turretRotation = lastRotation;
            //throw new NotImplementedException();
        }

        internal void CollectSmartBomb()
        {
            GameServices.GetService<Logger>().logMsg("Tank.collectSmartbomb()");
            //throw new NotImplementedException();
        }

        internal bool takeDamage(Bullet b)
        {
            bool wasHit = false;
            if (this.shieldstate == ShieldState.On)
            {
                SoundRefs.shieldHit.Play();
            }
            else
            {
                GameServices.GetService<Logger>().logMsg("Tank.takeDamage()");
                this.health -= C.bulletDamage;
                wasHit = true;
            }
            if (this.health <= 0.0f)
            {
                Die();
                SoundRefs.tankDead.Play();
                return true;
            }
            if (wasHit)
                SoundRefs.tankHit.Play();
            return false;
            
        }

        private void Die()
        {
            GameServices.GetService<Logger>().logMsg("DIE");
            this.state = TankState.Dead;
            this.respawnCounter = C.respawnTime;
            this.health = 0.0f;
        }

        internal void increaseShieldMeter(float p)
        {
            if (ShieldMeter < 1.0f)
                ShieldMeter += p;
            if (ShieldMeter > 1.0f)
                ShieldMeter = 1.0f;
        }
    }
}
