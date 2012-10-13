using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Solum.Utility;
using Solum.Input;
using Microsoft.Xna.Framework;

namespace Solum.SharedTanks
{
    public enum Teams { red,blue,green,yellow}

    class TankManager
    {
        public List<Tuple<Tank,Tank>> tanks = new List<Tuple<Tank,Tank>>();

        public void initTanks()
        {
            int screenWidth = GameServices.GetService<GraphicsDevice>().Viewport.Width;
            int screenHeight = GameServices.GetService<GraphicsDevice>().Viewport.Height;
            foreach (GamepadDevice d in G.activeGamepads)
            {
                if (d.PlayerIndex == PlayerIndex.One)
                {
                    tanks.Add(new Tuple<Tank,Tank>(
                        new Tank(Teams.red, 
                            new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY)), 
                        new Tank(Teams.red, 
                            new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY)
                            +new Vector2(C.tankPartnerOffsetX, C.tankPartnerOffsetY))));
                }
                else if (d.PlayerIndex == PlayerIndex.Two)
                {
                    tanks.Add(new Tuple<Tank, Tank>(
                        new Tank(Teams.blue,
                            new Vector2(screenWidth, 0) 
                            - new Vector2(C.tankSpawnOffsetX, 0)
                            + new Vector2(0,C.tankSpawnOffsetY)), 
                        new Tank(Teams.blue, 
                            new Vector2(screenWidth, 0) 
                            - new Vector2(C.tankSpawnOffsetX, 0)
                            + new Vector2(0,C.tankSpawnOffsetY)
                            - new Vector2(C.tankPartnerOffsetX, 0)
                            + new Vector2(0, C.tankPartnerOffsetY))));
                }
                else if (d.PlayerIndex == PlayerIndex.Three)
                {
                    tanks.Add(new Tuple<Tank, Tank>(
                        new Tank(Teams.green, 
                            new Vector2(0, screenHeight) 
                            - new Vector2(0, C.tankSpawnOffsetY)
                            + new Vector2(C.tankSpawnOffsetX,0)), 
                        new Tank(Teams.green,
                            new Vector2(0, screenHeight)
                            - new Vector2(0, C.tankSpawnOffsetY)
                            + new Vector2(C.tankSpawnOffsetX, 0) 
                            + new Vector2(C.tankPartnerOffsetX, 0) 
                            - new Vector2(0,C.tankPartnerOffsetY))));
                }
                else if (d.PlayerIndex == PlayerIndex.Four)
                {
                    tanks.Add(new Tuple<Tank, Tank>(
                        new Tank(Teams.yellow,
                            new Vector2(screenWidth, screenHeight)
                            -new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY)), 
                        new Tank(Teams.yellow,
                            new Vector2(screenWidth, screenHeight)
                            - new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY)
                            - new Vector2(C.tankPartnerOffsetX, C.tankPartnerOffsetY))));
                }
            }
        }

        public void Update()
        {
            foreach (Tuple<Tank,Tank> t in tanks)
            {
                t.Item1.Update();
                t.Item2.Update();

                if (t.Item1.usedShield)
                    t.Item2.SetShieldState(ShieldState.On);
                if (t.Item2.usedShield)
                    t.Item1.SetShieldState(ShieldState.Off);

                //Check collisions with other tanks
                if (checkTankCollisions(t.Item1))
                    t.Item1.Collide();

                if (checkTankCollisions(t.Item2))
                    t.Item2.Collide();

                //check collisions with staticworldobjects (walls, powerups)
                foreach (StaticWorldObject o in GameServices.GetService<GridManager>().checkTankCollision(t.Item1.getRotatedRectangle()))
                {
                    if (o.Type == StaticType.Wall)
                        t.Item1.Collide();
                    else if (o.Type == StaticType.SmartBomb)
                        t.Item1.CollectSmartBomb();
                }
                foreach (StaticWorldObject o in GameServices.GetService<GridManager>().checkTankCollision(t.Item2.getRotatedRectangle()))
                {
                    if (o.Type == StaticType.Wall)
                        t.Item2.Collide();
                    else if (o.Type == StaticType.SmartBomb)
                        t.Item2.CollectSmartBomb();
                }
                //check collisions with bullets
                GameServices.GetService<BulletManager>().checkTankCollisions(t.Item1);
                GameServices.GetService<BulletManager>().checkTankCollisions(t.Item2);
            }
        }

        private bool checkTankCollisions(Tank self)
        {
            foreach (Tuple<Tank, Tank> t in tanks)
            {
                if (self.pos != t.Item1.pos)
                {
                    if (self.getRotatedRectangle().Intersects(t.Item1.getRotatedRectangle()))
                    {
                        return true;
                    }
                }

                if (self.pos != t.Item2.pos)
                {
                    if (self.getRotatedRectangle().Intersects(t.Item2.getRotatedRectangle()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tuple<Tank, Tank> t in tanks)
            {
                t.Item1.Draw(spriteBatch);
                t.Item2.Draw(spriteBatch);
            }

        }
    }
}
