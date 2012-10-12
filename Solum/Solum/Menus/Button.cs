///
/// Based on the code by Jonathan Deaves ("garfunkle") 
/// http://www.gmaker.org/tutorials/downloads.asp?id=173
/// 
/// Modified by Teemu Kaukoranta, member of the Oulu GamedevClub Stage
/// http://www.gamedevcenter.org
/// 
/// Part of the S.o.l.u.m project
/// Licensed under WTFPL - Do What The Fuck You Want To Public License
/// It would be nice if you don't remove this comment section though

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Solum.Utility;

namespace Solum.Menus
{
    
    /*public class MenuButtonPressedEventArgs : EventArgs{
        public String ButtonName { get; set; }
        public ButtonAction ButtonAction { get; set; }
    }*/


    enum ButtonAction
    {
        Close,
        Quit,
        Other,
        Play,
        PlayerSelect
    }

    class Button
    {
        ButtonAction action;
        //public event EventHandler<MenuButtonPressedEventArgs> MenuButtonPressed;

        Rectangle bounds;

        public string text;
        Vector2 textPos;
        public bool isActive;

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
            this.isActive = false;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            textPos = new Vector2(bounds.X, bounds.Y);
            textPos += new Vector2((bounds.Width / 2) - (spriteFont.MeasureString(text).X / 2), (bounds.Height / 2) - (spriteFont.MeasureString(text).Y / 2));

            if(!this.isActive)
                spriteBatch.Draw(TextureRefs.menuButton, bounds, Color.White);
            else
                spriteBatch.Draw(TextureRefs.activeMenuButton, bounds, Color.White);
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
