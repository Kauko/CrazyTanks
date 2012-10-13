using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Solum.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Solum.Utility;
using Solum.Logging;

namespace Solum.Menus
{
    class PlayerSelectionWidget
    {
        Vector2 position;
        GamepadDevice device;
        bool started = false, oneReady = false, twoReady = false;

        public PlayerSelectionWidget(Vector2 position, GamepadDevice activeDevice)
        {
            this.position = position;
            this.device= activeDevice;
   
        }

        public PlayerSelectionWidget(Vector2 position, GamepadDevice activeDevice, bool started)
        {
            this.position = position;
            this.device = activeDevice;
            this.started = started;
        }

        public void resetWidget()
        {
            this.started = false;
            this.oneReady = false;
            this.twoReady = false;
        }

        public void setStarted(bool started)
        {
            this.started = started;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.started)
            {
                spriteBatch.Draw(TextureRefs.waitingReady, this.position, Color.White);
                if (this.oneReady)
                    spriteBatch.Draw(TextureRefs.ready, position + new Vector2(100, 100), Color.White);
                if (this.twoReady)
                    spriteBatch.Draw(TextureRefs.ready, position + new Vector2(150, 100), Color.White);
            }
            else
            {
                spriteBatch.Draw(TextureRefs.pressStart, this.position, Color.White);
            }
            
        }

        public void Update()
        {
            if (!device.IsConnected)
            {
                this.started = false;
                this.oneReady = false;
                this.twoReady = false;
            }
            else if (started)
            {
                //GameServices.GetService<Logger>().logMsg("Updating widget");
                if (device.WasButtonPressed(Buttons.DPadRight))
                {
                    if (!oneReady)
                        oneReady = true;
                    else
                        oneReady = false;
                }
                if (device.WasButtonPressed(Buttons.X))
                {
                    if (!twoReady)
                        twoReady = true;
                    else
                        twoReady = false;
                }
                if (device.WasButtonPressed(Buttons.Start) && device.PlayerIndex != PlayerIndex.One)
                {
                    this.started = false;
                    this.oneReady = false;
                    this.twoReady = false;
                }
            }else if(device.WasButtonPressed(Buttons.Start)){
                this.started = true;
            }
        }

        public bool canStart()
        {
            if (!started)
                return true;
            else if (oneReady && twoReady)
                return true;

            return false;
        }

        public bool bothReady()
        {
            if (oneReady && twoReady)
                return true;
            else
                return false;
        }

        public bool isStarted()
        {
            return this.started;
        }

        public GamepadDevice getController()
        {
            return this.device;
        }

    }
}
