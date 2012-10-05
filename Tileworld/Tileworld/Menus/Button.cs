///=============================================================///
///Author:  Jonathan Deaves ("garfunkle")                       ///
///Date:    13-June-2011                                        ///
///Version: 0.1a                                                ///
///=============================================================///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Solum.Menu
{
    
    /*public class MenuButtonPressedEventArgs : EventArgs{
        public String ButtonName { get; set; }
        public ButtonAction ButtonAction { get; set; }
    }*/


    enum ButtonAction{
        Close,
        Quit,
        Other
    }

    class Button
    {
        ButtonAction action;
        //public event EventHandler<MenuButtonPressedEventArgs> MenuButtonPressed;

        Texture2D texture;
        Rectangle bounds;

        public string text;
        Vector2 textPos;

        public ButtonAction Action
        {
            get { return this.action; }
        }
        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public Button(ButtonAction action, Rectangle bounds, string text)
        {
            this.action = action;
            this.bounds = bounds;
            this.text = text;
        }

        public void Load(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("Graphics\\Buttons\\Button");
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            textPos = new Vector2(bounds.X, bounds.Y);
            textPos += new Vector2((bounds.Width / 2) - (spriteFont.MeasureString(text).X / 2), (bounds.Height / 2) - (spriteFont.MeasureString(text).Y / 2));

            spriteBatch.Draw(texture, bounds, Color.White);
            spriteBatch.DrawString(spriteFont, text, textPos, Color.Black);
        }

        /*public void OnMenuButtonPressed(){
            if(MenuButtonPressed != null){
                MenuButtonPressedEventArgs args = new MenuButtonPressedEventArgs();
                args.ButtonAction = this.Action;
                args.ButtonName = this.text;
                MenuButtonPressed(this, args);
            }
        }*/
    }
}
