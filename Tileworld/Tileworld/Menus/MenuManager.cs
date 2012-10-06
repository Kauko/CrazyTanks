///
/// Based on the code by Jonathan Deaves ("garfunkle") 
/// http://www.gmaker.org/tutorials/downloads.asp?id=173
/// 
/// Modified by Teemu Kaukoranta, member of the Oulu GamedevClub Stage
/// http://www.gamedevcenter.org
/// 
/// Part of the Solum project

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Solum.Utility;
using Solum.Logging;

namespace Solum.Menus
{
    class MenuManager
    {
        public enum MenuStates
        {
            None,
            Exit
        }public MenuStates MenuState { get; set; }  //Events to feed back to Game1.cs
        
        Dictionary<string, Menu> Menus;     //A container for all our menus
        Menu activeMenu;                    //Which menu is currently active
        Stack<Menu> previousMenus;          //An ordered queue of previous menus.

        public Menu ActiveMenu
        {
            get { return activeMenu; }
        }

        public MenuManager()
        {
            Menus = new Dictionary<string, Menu>();
            previousMenus = new Stack<Menu>();
        }

        public void Update()
        {
            if (activeMenu != null)
                activeMenu.Update();

            GetButtonEvent();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (activeMenu != null)
            {
                activeMenu.Draw(spriteBatch);
            }
            else
                GameServices.GetService<Logger>().logMsg("activeMenu is null");
        }

        /// <summary>
        /// This method adds a new menu to storage
        /// </summary>
        public void AddMenu(string name, Menu menu)
        {
            Menus.Add(name, menu);
        }

        /// <summary>
        /// Puts any active menu item storage
        /// and then opens the new menu.
        /// </summary>
        /// <param name="name">Name of menu</param>
        public void Show(string name)
        {
            if (Menus.ContainsKey(name))
            {
                if (activeMenu == null)
                {
                    //First menu open, so we play bgm.
                    Menus[name].PlayBGM(true);

                    //Sets the active menu based on name
                    //then performs menus open method.
                    activeMenu = Menus[name];
                    activeMenu.Open(true);
                }
                else
                {
                    //Perform closing actions of menu then
                    //puts current menu in stack of hidden.
                    activeMenu.Close(false);
                    previousMenus.Push(activeMenu);

                    //Sets the active menu based on name
                    //then performs menus open method.
                    activeMenu = Menus[name];
                    activeMenu.Open(true);
                }

            }
            else
                return;
        }

        /// <summary>
        /// Closes the current menu then re-opens
        /// last menu, if any.
        /// </summary>
        public void Close()
        {
            activeMenu.Close(true);
            if (previousMenus.Count() > 0)
            {
                activeMenu = previousMenus.Pop();
            }
            else
            {
                activeMenu.StopBGM();
                activeMenu = null;
                G.gameState = GameState.playing;
            }
        }

        /// <summary>
        /// Closes all menus, clears the storage
        /// then will be returned to game.
        /// </summary>
        public void Exit()
        {
            activeMenu.StopBGM();
            previousMenus.Clear();
            activeMenu.Close(true);
            activeMenu = null;
        }

        public void GetButtonEvent()
        {
            if (activeMenu != null)
            {
                switch (activeMenu.ButtonState)
                {
                    case Menu.ButtonStates.None:
                        break;
                    case Menu.ButtonStates.Close:
                        activeMenu.ButtonState = Menu.ButtonStates.None;
                        Close();
                        break;
                    case Menu.ButtonStates.Quit:
                        activeMenu.ButtonState = Menu.ButtonStates.None;
                        MenuState = MenuStates.Exit;
                        break;
                    case Menu.ButtonStates.Pressed:
                        activeMenu.ButtonState = Menu.ButtonStates.None;
                        Show(activeMenu.pressedButtonName);
                        break;
                }
            }
        }
    }
}
