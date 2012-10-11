using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Solum.Utility;

namespace Solum.Menus
{
    class PauseMenu : Menu
    {
        public PauseMenu(String title) : base(title){

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var rect = new Texture2D(GameServices.GetService<GraphicsDevice>(), 1, 1);
            rect.SetData(new[] { Color.Black });
            spriteBatch.Draw(rect, new Rectangle(0, 0, GameServices.GetService<GraphicsDevice>().Viewport.Width, GameServices.GetService<GraphicsDevice>().Viewport.Height), Color.White*0.5f);

            Vector2 titlePosition = new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width / 2 - (SpriteFontRefs.titleFont.MeasureString(title).X / 2), 50);
            spriteBatch.DrawString(SpriteFontRefs.titleFont, title, titlePosition, Color.Black);

            foreach (Button b in buttons)
            {
                b.Draw(spriteBatch, SpriteFontRefs.textFont);
            }
        }
    }
}
