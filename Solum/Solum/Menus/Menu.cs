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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Solum.Utility;
using Solum.Input;
using Solum.Logging;

namespace Solum.Menus
{
    abstract class Menu
    {
        public enum ButtonStates
        {
            None,
            Close,
            Pressed,
            Quit,
            Play,
            PlayerSelection
        }public ButtonStates ButtonState { get; set; }   //Feedback event to MenuManager

        public String pressedButtonName;            //Handle inputs

        protected string title; //Title at top of menu

        protected List<Button> buttons = new List<Button>();
        private int selectedIndex = 0;
        /// <summary>
        /// Constructs a base menu item.
        /// </summary>
        /// <param name="title">The 'name' of the menu, shows in title at top</param>
        public Menu(string title)
        {
            this.title = title;
            this.ButtonState = ButtonStates.None;
        }

        /// <summary>
        /// Adds buttons to the menu page. First Item must be a return button, 
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="id"></param>
        /// <param name="bounds"></param>
        /// <param name="text"></param>
        public void LoadButtons(ButtonAction[] actions, List<Rectangle> bounds, List<string> text)
        {
            for (int i = 0; i < actions.Count(); i++)
            {
                this.buttons.Add(new Button(actions[i], bounds[i], text[i]));
                //buttons[i].MenuButtonPressed += HandleMenuButtonPressed;
            }
            buttons.ElementAt(this.selectedIndex).isActive = true;
        }

        public virtual void Update()
        {
            if (G.gamePadOne.WasButtonPressed(Buttons.DPadDown))
            {
                if (this.selectedIndex < buttons.Count - 1)
                    this.selectedIndex++;
                else
                    this.selectedIndex = 0;

                foreach (Button b in buttons)
                    b.isActive = false;
                buttons.ElementAt(this.selectedIndex).isActive = true;
            }
            else if (G.gamePadOne.WasButtonPressed(Buttons.DPadUp))
            {
                if (this.selectedIndex > 0)
                    this.selectedIndex--;
                else
                    this.selectedIndex = buttons.Count - 1;

                foreach (Button b in buttons)
                    b.isActive = false;
                buttons.ElementAt(this.selectedIndex).isActive = true;
            }

            

            if (G.gamePadOne.WasButtonPressed(Buttons.A))
            {
                this.ButtonPush(buttons.ElementAt(selectedIndex));
            }
            
            /*if (GameServices.GetService<MouseDevice>().WasButtonPressed(MouseButtons.Left))
            {
                Point mousePos = new Point((int)GameServices.GetService<MouseDevice>().State.X, (int)GameServices.GetService<MouseDevice>().State.Y);
                foreach (Button b in buttons)
                {
                    if (b.Bounds.Contains(mousePos))
                        this.ButtonPush(b);
                }
            }*/
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(TextureRefs.menuBgImage, new Rectangle(0, 0, GameServices.GetService<GraphicsDevice>().Viewport.Width, GameServices.GetService<GraphicsDevice>().Viewport.Height), Color.Black);

            Vector2 titlePosition = new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width / 2 - (SpriteFontRefs.titleFont.MeasureString(title).X / 2), 50);
            spriteBatch.DrawString(SpriteFontRefs.titleFont, title, titlePosition, Color.Black);

            foreach (Button b in buttons)
            {
                b.Draw(spriteBatch, SpriteFontRefs.textFont);
            }
        }

        
        public void ButtonPush(Button b)
        {
            switch (b.Action)
            {
                case ButtonAction.Quit:
                    this.ButtonState = ButtonStates.Quit;
                    break;
                case ButtonAction.Other:
                    this.ButtonState = ButtonStates.Pressed;
                    this.pressedButtonName = b.text;
                    break;
                case ButtonAction.Close:
                    this.ButtonState = ButtonStates.Close;
                    break;
                case ButtonAction.Play:
                    this.ButtonState = ButtonStates.Play;
                    break;
                case ButtonAction.PlayerSelect:
                    this.ButtonState = ButtonStates.PlayerSelection;
                    break;
            }
        }

        /// <summary>
        /// This occurs when the menu is opened.
        /// </summary>
        /// <param name="playSoundEffect">Should sound effect be played during this call</param>
        public virtual void Open(bool playSoundEffect)
        {
            if (playSoundEffect)
                SoundRefs.menuOpen.Play();
        }

        /// <summary>
        /// This occurs when menu is closed
        /// </summary>
        /// <param name="playSoundEffect">Should sound effect be played during this call</param>
        public virtual void Close(bool playSoundEffect)
        {
            if (playSoundEffect)
                SoundRefs.menuClose.Play();
        }

        public void PlayBGM(bool loop)
        {
            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Play(SoundRefs.bgm);
        }

        public void StopBGM()
        {
            MediaPlayer.Stop();
        }
    }
}
