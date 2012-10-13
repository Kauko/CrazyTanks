using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Solum.Utility;
using Microsoft.Xna.Framework;

namespace Solum.SharedTanks
{
    class BulletManager
    {
        List<Bullet> bullets = new List<Bullet>();

        public void addBullet(Bullet b)
        {
            bullets.Add(b);
        }

        public void Update()
        {
            Rectangle screen = new Rectangle(0, 0, GameServices.GetService<GraphicsDevice>().Viewport.Width, GameServices.GetService<GraphicsDevice>().Viewport.Height);
            screen.Inflate(100, 100);
            foreach (Bullet b in bullets)
            {
                b.Update();
                //Check that bullet is on screen
                if (!screen.Contains(new Point((int)b.pos.X, (int)b.pos.Y)))
                {
                    b.removable = true;
                    continue;
                }
                //check collisions with walls
                if (GameServices.GetService<GridManager>().checkBulletCollision(b))
                {
                    b.removable = true;
                    continue;
                }
            }

            removeBullet();
        }

        public void checkTankCollisions(Tank t)
        {
            foreach (Bullet b in bullets)
            {
                if (t.getRotatedRectangle().Intersects(new RotatedRectangle(new Rectangle((int)b.pos.X, (int)b.pos.Y, TextureRefs.bullet.Width, TextureRefs.bullet.Height),0.0f)))
                {
                    if (b.shooter != t)
                    {
                        if (t.state == TankState.Alive)
                        {
                            t.TakeDamage(b);
                            if(t.Team != b.shooter.Team)
                                b.shooter.increaseShieldMeter(C.shieldMeterIncreaseOnDamage);
                        }
                        b.removable = true;
                    }
                }
            }

            removeBullet();
        }

        private void removeBullet()
        {
            List<Bullet> bulletsCopy = new List<Bullet>(bullets);
            foreach (Bullet b in bulletsCopy)
            {
                if (b.removable)
                    bullets.Remove(b);
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Bullet b in bullets)
            {
                b.Draw(spriteBatch);
            }
        }
    }
}
