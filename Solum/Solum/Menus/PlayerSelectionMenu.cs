using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Solum.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Solum.Menus
{
    class PlayerSelectionMenu
    {
        PlayerSelectionWidget[] widgets = new PlayerSelectionWidget[4];

        public PlayerSelectionMenu()
        {
            widgets[0] = new PlayerSelectionWidget(new Vector2(C.playerSelectionWidgetPositionX,C.playerSelectionWidgetPositionY), G.gamePadOne, true);
            widgets[1] = new PlayerSelectionWidget(new Vector2(C.playerSelectionWidgetPositionX, C.playerSelectionWidgetPositionY) + new Vector2(C.playerSelectionWidgetHorizontalOffsetX,0), G.gamePadTwo);
            widgets[2] = new PlayerSelectionWidget(new Vector2(C.playerSelectionWidgetPositionX, C.playerSelectionWidgetPositionY) + new Vector2(0,C.playerSelectionWidgetVerticalOffsetY), G.gamePadThree);
            widgets[3] = new PlayerSelectionWidget(new Vector2(C.playerSelectionWidgetPositionX, C.playerSelectionWidgetPositionY) + new Vector2(0,C.playerSelectionWidgetVerticalOffsetY) 
                + new Vector2(C.playerSelectionWidgetHorizontalOffsetX,0), G.gamePadFour);
        }

        private void resetWidgets()
        {
            foreach (PlayerSelectionWidget w in widgets)
            {
                w.resetWidget();
            }

            widgets[0].setStarted(true);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureRefs.menuBgImage, new Rectangle(0, 0, GameServices.GetService<GraphicsDevice>().Viewport.Width, GameServices.GetService<GraphicsDevice>().Viewport.Height), Color.White);
            foreach(PlayerSelectionWidget p in widgets){
                p.Draw(spriteBatch);
            }
        }

        public void Update()
        {
            bool startGame = true;
            foreach (PlayerSelectionWidget p in widgets)
            {
                p.Update();
                if (!p.canStart())
                    startGame = false;
            }
            if (!widgets[0].bothReady())
                startGame = false;
            if (startGame && widgets[0].getController().WasButtonPressed(Buttons.Start))
            {
                foreach (PlayerSelectionWidget p in widgets)
                {
                    if(p.isStarted())
                        G.activeGamepads.Add(p.getController());
                }
                G.gameState = GameState.playing;
                resetWidgets();
            }

            if(G.gamePadOne.WasButtonPressed(Buttons.Back)){
                G.gameState = GameState.menu;
                resetWidgets();
            }

        }
    }
}
