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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Solum.Utility;
using Solum.Input;

namespace Solum.Menu
{
    class Menu
    {
        public enum ButtonStates
        {
            None,
            Close,
            Pressed,
            Quit,
        }public ButtonStates ButtonState { get; set; }   //Feedback event to MenuManager

        public String pressedButtonName;            //Handle inputs
        protected SpriteFont titleFont;             //larger font to draw titles
        protected SpriteFont textFont;              //Font to draw menu text

        protected Texture2D backgroundImage;        //The image to show on background of menu
        protected Song bgm;                         //An audio track to player while open
        protected SoundEffect menuOpen;             //Plays when menu first opens.
        protected SoundEffect menuClose;            //Plays when menu closes.
        protected string title;                     //Title at top of menu

        protected List<Button> buttons;
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
        public void LoadButtons(ContentManager Content, ButtonAction[] actions, List<Rectangle> bounds, List<string> text)
        {
            for (int i = 0; i < actions.Count(); i++)
            {
                this.buttons.Add(new Button(actions[i], bounds[i], text[i]));
                buttons[i].Load(Content);
                //buttons[i].MenuButtonPressed += HandleMenuButtonPressed;
            }
        }

        /// <summary>
        /// Load the menus content
        /// </summary>
        /// <param name="bgTexturePath">Path to texture in relation to Content Manager</param>
        /// <param name="bgmPath">Path to bgm file in relation to Content Manager</param>
        public virtual void Load(ContentManager Content, string bgTexturePath, string bgmPath, string menuOpenPath, string menuClosePath)
        {
            titleFont = Content.Load<SpriteFont>("Fonts\\titleFont");
            textFont = Content.Load<SpriteFont>("Fonts\\textFont");
            //Load in our image and audio files
            backgroundImage = Content.Load<Texture2D>(bgTexturePath);
            bgm = Content.Load<Song>(bgmPath);
            menuOpen = Content.Load<SoundEffect>(menuOpenPath);
            menuClose = Content.Load<SoundEffect>(menuClosePath);
        }

        public virtual void Update()
        {
            if (GameServices.GetService<MouseDevice>().WasButtonHeld(MouseButtons.Left))
            {
                Point mousePos = new Point((int)GameServices.GetService<MouseDevice>().Position.X, (int)GameServices.GetService<MouseDevice>().Position.Y);
                foreach (Button b in buttons)
                {
                    if (b.Bounds.Contains(mousePos))
                        this.ButtonPush(b);
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundImage, Vector2.Zero, Color.White);

            Vector2 titlePosition = new Vector2(400 - (titleFont.MeasureString(title).X / 2), 50);
            spriteBatch.DrawString(titleFont, title, titlePosition, Color.Black);

            foreach (Button b in buttons)
            {
                b.Draw(spriteBatch, textFont);
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
            }
        }

        /// <summary>
        /// This occurs when the menu is opened.
        /// </summary>
        /// <param name="playSoundEffect">Should sound effect be played during this call</param>
        public virtual void Open(bool playSoundEffect)
        {
            if (playSoundEffect)
                menuOpen.Play();
        }

        /// <summary>
        /// This occurs when menu is closed
        /// </summary>
        /// <param name="playSoundEffect">Should sound effect be played during this call</param>
        public virtual void Close(bool playSoundEffect)
        {
            if (playSoundEffect)
                menuClose.Play();
        }

        /*public void HandleMenuButtonPressed(object sender, MenuButtonPressedEventArgs args)
        {

        }*/

        public void PlayBGM(bool loop)
        {
            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Play(bgm);
        }

        public void StopBGM()
        {
            MediaPlayer.Stop();
        }
    }
}
