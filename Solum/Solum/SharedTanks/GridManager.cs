using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solum.Utility;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Solum.SharedTanks
{
    class GridManager
    {
        int pickupsCounter = 0;
        LevelLibrary.Level level;
        StaticWorldObject[,]tiles;
        int pickupTimer = 0;
        Vector2 gridPosition;

        public void loadMap()
        {
            Random rand = new Random();
            this.level = G.levels.ElementAt(rand.Next(G.levels.Count));
            this.tiles = new StaticWorldObject[this.level.Columns, this.level.Rows];

            int screenHeight = GameServices.GetService<GraphicsDevice>().Viewport.Height;
            int screenWidth = GameServices.GetService<GraphicsDevice>().Viewport.Width;
            int screenVerticalCenter = screenHeight / 2;
            int screenHorizontalCenter = screenWidth / 2;

            int gridHeight = this.level.Rows * C.tileHeight;
            int gridWidth = this.level.Columns * C.tileWidth;
            int gridVerticalCenter = gridHeight / 2;
            int gridHorizontalCenter = gridWidth / 2;
            this.gridPosition = new Vector2(screenHorizontalCenter, screenVerticalCenter) - new Vector2(gridHorizontalCenter, gridVerticalCenter);

            for (int c = 0; c < this.level.Columns; c++)
            {
                for (int r = 0; r < this.level.Rows; r++)
                {
                    int symbol = level.GetValue(r, c);
                    switch (symbol)
                    {
                        case 0:
                            this.tiles[c, r] = new EmptyStaticWorldObject();
                            break;
                        case 1:
                            this.tiles[c, r] = new Wall();
                            break;
                        case 2:
                            SmartBombPickup foo = new SmartBombPickup();
                            this.tiles[c, r] = foo;
                            this.pickupsCounter++;
                            break;
                    }
                }
            }
        }

        public List<StaticWorldObject> checkTankCollision(RotatedRectangle tankPosition)
        {
            List<StaticWorldObject> ret = new List<StaticWorldObject>();
            for (int c = 0; c < this.level.Columns; c++)
            {
                for (int r = 0; r < this.level.Rows; r++)
                {
                    RotatedRectangle rect = new RotatedRectangle(new Rectangle((int)this.gridPosition.X + c * C.tileWidth, (int)this.gridPosition.Y + r * C.tileHeight, C.tileWidth, C.tileHeight), 0.0f);
                    if (this.tiles[c,r].Type != StaticType.Empty && tankPosition.Intersects(rect))
                    {
                        ret.Add(this.tiles[c,r]);
                        if (this.tiles[c, r].Type == StaticType.SmartBomb)
                        {
                            this.pickupsCounter--;
                            this.tiles[c, r] = new EmptyStaticWorldObject();
                        }
                    }
                }
            }
            return ret;
        }

        public bool checkBulletCollision(Bullet b)
        {
            Rectangle rect = new Rectangle((int)b.pos.X, (int)b.pos.Y, TextureRefs.bullet.Width, TextureRefs.bullet.Height);
            for (int c = 0; c < this.level.Columns; c++)
            {
                for (int r = 0; r < this.level.Rows; r++)
                {
                    if (this.tiles[c, r].Type == StaticType.Wall && rect.Intersects(new Rectangle((int)this.gridPosition.X + c * C.tileWidth, (int)this.gridPosition.Y + r * C.tileHeight, C.tileWidth, C.tileHeight)) )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Update()
        {
            if (this.pickupsCounter < C.maxPickups && this.pickupTimer >= C.pickupInterval)
            {
                Random rand = new Random();
                if(rand.Next(C.pickupSpawnChance) == 1)
                    spawnPickup(true);
            }
            else if (this.pickupTimer < C.pickupInterval)
                this.pickupTimer++;
            /* Actually there's no need for this..?
            for (int c = 0; c < this.level.Columns; c++)
            {
                for (int r = 0; r < this.level.Rows; r++)
                {
                    this.tiles[c, r].Update();
                }
            }*/
        }

        private void spawnPickup(bool spawnRandomly)
        {
            this.pickupTimer = 0;
            List<Tuple<int, int>> possibleTiles = new List<Tuple<int, int>>();
            if (spawnRandomly)
            {
                for (int c = 0; c < this.level.Columns; c++)
                {
                    for (int r = 0; r < this.level.Rows; r++)
                    {
                        if (this.tiles[c, r].Type == StaticType.Empty)
                        {
                            possibleTiles.Add(Tuple.Create(c, r));
                        }
                    }
                }
            }
            else
            {
                for (int c = 0; c < this.level.Columns; c++)
                {
                    for (int r = 0; r < this.level.Rows; r++)
                    {
                        if (this.tiles[c, r].Type == StaticType.Empty && this.level.GetValue(c,r) == 2)
                        {
                            possibleTiles.Add(Tuple.Create(c, r));
                        }
                    }
                }
            }

            Random rand = new Random();
            Tuple<int,int> t = possibleTiles.ElementAt(rand.Next(possibleTiles.Count));
            SmartBombPickup foo = new SmartBombPickup();
            this.tiles[t.Item1, t.Item2] = foo;
            this.pickupsCounter++;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int c = 0; c < this.level.Columns; c++)
            {
                for (int r = 0; r < this.level.Rows; r++)
                {
                    this.tiles[c, r].Draw(spriteBatch, gridPosition + new Vector2(c*C.tileWidth, r*C.tileHeight));
                }
            }
        }
    }
}
