using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Solum.Utility;
using Solum.Input;
using Microsoft.Xna.Framework;
using Solum.Logging;

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
                            new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY), MathHelper.PiOver2 + MathHelper.PiOver4, d, ControlSide.Left), 
                        new Tank(Teams.red, 
                            new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY)
                            +new Vector2(C.tankPartnerOffsetX, C.tankPartnerOffsetY), MathHelper.PiOver2 + MathHelper.PiOver4, d, ControlSide.Right)));
                }
                else if (d.PlayerIndex == PlayerIndex.Two)
                {
                    tanks.Add(new Tuple<Tank, Tank>(
                        new Tank(Teams.blue,
                            new Vector2(screenWidth, 0) 
                            - new Vector2(C.tankSpawnOffsetX, 0)
                            + new Vector2(0,C.tankSpawnOffsetY), MathHelper.Pi + MathHelper.PiOver4, d, ControlSide.Left), 
                        new Tank(Teams.blue, 
                            new Vector2(screenWidth, 0) 
                            - new Vector2(C.tankSpawnOffsetX, 0)
                            + new Vector2(0,C.tankSpawnOffsetY)
                            - new Vector2(C.tankPartnerOffsetX, 0)
                            + new Vector2(0, C.tankPartnerOffsetY), MathHelper.Pi + MathHelper.PiOver4, d, ControlSide.Right)));
                }
                else if (d.PlayerIndex == PlayerIndex.Three)
                {
                    tanks.Add(new Tuple<Tank, Tank>(
                        new Tank(Teams.green, 
                            new Vector2(0, screenHeight) 
                            - new Vector2(0, C.tankSpawnOffsetY)
                            + new Vector2(C.tankSpawnOffsetX,0), MathHelper.PiOver4, d, ControlSide.Left), 
                        new Tank(Teams.green,
                            new Vector2(0, screenHeight)
                            - new Vector2(0, C.tankSpawnOffsetY)
                            + new Vector2(C.tankSpawnOffsetX, 0) 
                            + new Vector2(C.tankPartnerOffsetX, 0)
                            - new Vector2(0, C.tankPartnerOffsetY), MathHelper.PiOver4, d, ControlSide.Right)));
                }
                else if (d.PlayerIndex == PlayerIndex.Four)
                {
                    tanks.Add(new Tuple<Tank, Tank>(
                        new Tank(Teams.yellow,
                            new Vector2(screenWidth, screenHeight)
                            -new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY), MathHelper.TwoPi - MathHelper.PiOver4, d, ControlSide.Left), 
                        new Tank(Teams.yellow,
                            new Vector2(screenWidth, screenHeight)
                            - new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY)
                            - new Vector2(C.tankPartnerOffsetX, C.tankPartnerOffsetY), MathHelper.TwoPi - MathHelper.PiOver4, d, ControlSide.Right)));
                }
            }
        }

        public void ReSpawn(Tank tank){
            int screenWidth = GameServices.GetService<GraphicsDevice>().Viewport.Width;
            int screenHeight = GameServices.GetService<GraphicsDevice>().Viewport.Height;

            switch(tank.Team){
                case Teams.red:
                    if (tank.controls.controlside == ControlSide.Left)
                        tank.Initialize(new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY), MathHelper.Pi + MathHelper.PiOver4);
                    else
                        tank.Initialize(new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY) + new Vector2(C.tankPartnerOffsetX, C.tankPartnerOffsetY), MathHelper.Pi + MathHelper.PiOver4);
                    break;
                case Teams.blue:
                    if (tank.controls.controlside == ControlSide.Left)
                        tank.Initialize(new Vector2(screenWidth, 0) - new Vector2(C.tankSpawnOffsetX, 0) + new Vector2(0, C.tankSpawnOffsetY), MathHelper.Pi + MathHelper.PiOver4);
                    else
                        tank.Initialize(new Vector2(screenWidth, 0) - new Vector2(C.tankSpawnOffsetX, 0) + new Vector2(0, C.tankSpawnOffsetY) - new Vector2(C.tankPartnerOffsetX, 0) + new Vector2(0, C.tankPartnerOffsetY), MathHelper.Pi + MathHelper.PiOver4);
                    break;
                case Teams.green:
                    if (tank.controls.controlside == ControlSide.Left)
                        tank.Initialize(new Vector2(0, screenHeight) - new Vector2(0, C.tankSpawnOffsetY) + new Vector2(C.tankSpawnOffsetX, 0), MathHelper.PiOver4);
                    else
                        tank.Initialize(new Vector2(0, screenHeight) - new Vector2(C.tankSpawnOffsetX, 0) + new Vector2(0, C.tankSpawnOffsetY) - new Vector2(C.tankPartnerOffsetX, 0) + new Vector2(0, C.tankPartnerOffsetY), MathHelper.PiOver4);
                    break;
                case Teams.yellow:
                    if (tank.controls.controlside == ControlSide.Left)
                        tank.Initialize(new Vector2(screenWidth, screenHeight) - new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY), MathHelper.TwoPi - MathHelper.PiOver4);
                    else
                        tank.Initialize(new Vector2(screenWidth, screenHeight) - new Vector2(C.tankSpawnOffsetX, C.tankSpawnOffsetY) - new Vector2(C.tankPartnerOffsetX, C.tankPartnerOffsetY), MathHelper.TwoPi - MathHelper.PiOver4);
                    break;
            }
        }

        public void Update()
        {
            foreach (Tuple<Tank,Tank> t in tanks)
            {
                t.Item1.Update();
                t.Item2.Update();

                if (t.Item1.state == TankState.Spawning)
                {
                    ReSpawn(t.Item1);
                }
                if (t.Item2.state == TankState.Spawning)
                {
                    ReSpawn(t.Item2);
                }

                if (t.Item1.usedShield)
                {
                    t.Item2.SetShieldState(ShieldState.On);
                    t.Item1.usedShield = false;
                }
                if (t.Item2.usedShield)
                {
                    t.Item1.SetShieldState(ShieldState.On);
                    t.Item2.usedShield = false;
                }

                //Check collisions with other tanks
                if (checkTankCollisions(t.Item1))
                    t.Item1.Collide();

                if (checkTankCollisions(t.Item2))
                    t.Item2.Collide();

                //Check collision with play area border
                if(checkScreenCollisions(t.Item1))
                    t.Item1.Collide();
                if (checkScreenCollisions(t.Item2))
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

        private bool checkScreenCollisions(Tank self)
        {
            Rectangle screenLeft = new Rectangle(0, 0, C.frameWidth, GameServices.GetService<GraphicsDevice>().Viewport.Height);
            Rectangle screenRight = new Rectangle(GameServices.GetService<GraphicsDevice>().Viewport.Width-C.frameWidth, 0, C.frameWidth, GameServices.GetService<GraphicsDevice>().Viewport.Height);
            Rectangle screenTop = new Rectangle(0, 0,GameServices.GetService<GraphicsDevice>().Viewport.Width, C.frameHeight);
            Rectangle screenBottom = new Rectangle(0, GameServices.GetService<GraphicsDevice>().Viewport.Height- C.frameHeight, GameServices.GetService<GraphicsDevice>().Viewport.Width, C.frameHeight);
            if (self.getRotatedRectangle().Intersects(screenLeft))
            {
                GameServices.GetService<Logger>().logMsg("screenLeft");
                return true;
            }
                
            if (self.getRotatedRectangle().Intersects(screenRight))
            {
                GameServices.GetService<Logger>().logMsg("screenRight");
                return true;
            }
            if (self.getRotatedRectangle().Intersects(screenTop))
            {
                GameServices.GetService<Logger>().logMsg("screenTop");
                return true;
            }
            if (self.getRotatedRectangle().Intersects(screenBottom))
            {
                GameServices.GetService<Logger>().logMsg("screenBottom");
                return true;
            }
            return false;
        }

        private bool checkTankCollisions(Tank self)
        {
            foreach (Tuple<Tank, Tank> t in tanks)
            {
                if (self.pos != t.Item1.pos)
                {
                    if (self.getRotatedRectangle().Intersects(t.Item1.getRotatedRectangle()))
                    {
                        GameServices.GetService<Logger>().logMsg("Tanks collide!");
                        return true;
                    }
                }

                if (self.pos != t.Item2.pos)
                {
                    if (self.getRotatedRectangle().Intersects(t.Item2.getRotatedRectangle()))
                    {
                        GameServices.GetService<Logger>().logMsg("Tanks collide!");
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

                if (t.Item1.Team == Teams.red)
                {
                    G.redOneShield = t.Item1.ShieldMeter;
                    G.redTwoShield = t.Item2.ShieldMeter;
                }
                else if (t.Item1.Team == Teams.blue)
                {
                    G.blueOneShield = t.Item1.ShieldMeter;
                    G.blueTwoShield = t.Item2.ShieldMeter;
                }
                else if (t.Item1.Team == Teams.green)
                {
                    G.greenOneShield = t.Item1.ShieldMeter;
                    G.greenTwoShield = t.Item2.ShieldMeter;
                }
                else if (t.Item1.Team == Teams.yellow)
                {
                    G.yellowOneShield = t.Item1.ShieldMeter;
                    G.yellowTwoShield = t.Item2.ShieldMeter;
                }
            }
        }
    }
}
