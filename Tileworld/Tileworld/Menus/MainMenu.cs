using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Solum.Utility;
using Microsoft.Xna.Framework;

namespace Solum.Menus
{
    class MainMenu : Menu
    {
        public MainMenu(String title) : base(title){

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureRefs.menuBgImage, new Rectangle(0, 0, GameServices.GetService<GraphicsDevice>().Viewport.Width, GameServices.GetService<GraphicsDevice>().Viewport.Height), Color.White);

            Vector2 titlePosition = new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width / 2 - (SpriteFontRefs.titleFont.MeasureString(title).X / 2), 50);
            spriteBatch.DrawString(SpriteFontRefs.titleFont, title, titlePosition, Color.Black);

            foreach (Button b in buttons)
            {
                b.Draw(spriteBatch, SpriteFontRefs.textFont);
            }
        }
    }
}
