#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileWorld.Utility;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        float pauseAlpha;

        KeyboardState oldKeyboard;
        MouseState oldMouse;
        float oldFramerate;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {

               

                updateCamera(keyboard, mouse);



                oldMouse = mouse;
                oldKeyboard = keyboard;

            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 2;
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, G.cam.get_transformation(ScreenManager.GraphicsDevice));
            spriteBatch.Draw(TextureRefs.koala, new Vector2(0.0f, 0.0f), Color.White);

            //Writes framerate to console
            float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (frameRate - oldFramerate > 3.0f || frameRate - oldFramerate < -3.0f)
                Console.WriteLine("frame rate: " + frameRate);
            oldFramerate = frameRate;

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion

        private void updateCamera(KeyboardState keyboard, MouseState mouse)
        {

            //Keyboard camera movement
            if (keyboard.IsKeyDown(Keys.A))
                G.cam.MoveLeft(C.camKeyboardScrollSpeed * 2 / G.cam.Zoom);

            if (keyboard.IsKeyDown(Keys.D))
                G.cam.MoveRight(C.camKeyboardScrollSpeed * 2 / G.cam.Zoom);

            if (keyboard.IsKeyDown(Keys.W))
                G.cam.MoveUp(C.camKeyboardScrollSpeed * 2 / G.cam.Zoom);

            if (keyboard.IsKeyDown(Keys.S))
                G.cam.MoveDown(C.camKeyboardScrollSpeed * 2 / G.cam.Zoom);

            //Keyboard zoom
            if (keyboard.IsKeyDown(Keys.R))
                if (G.cam.Zoom > C.camZoomSpeedThreshold)
                {
                    G.cam.Zoom += C.camKeyboardCloseZoomSpeed;
                }
                //zoom for far away
                else
                {
                    G.cam.Zoom += C.camKeyboardFarZoomSpeed;
                }

            if (keyboard.IsKeyDown(Keys.F))
                if (G.cam.Zoom > C.camZoomSpeedThreshold)
                {
                    G.cam.Zoom -= C.camKeyboardCloseZoomSpeed;
                }
                //zoom for far away
                else
                {
                    G.cam.Zoom -= C.camKeyboardFarZoomSpeed;
                }

            //Keyboard camera rotation
            if (keyboard.IsKeyDown(Keys.Q))
                G.cam.Rotation -= C.camKeyboardRotationSpeed;

            if (keyboard.IsKeyDown(Keys.E))
                G.cam.Rotation += C.camKeyboardRotationSpeed;

            //Mouse camera movement
            if (mouse.Y < C.camMouseScrollBorderWidth && mouse.Y > 0)
                G.cam.MoveUp(C.camKeyboardScrollSpeed);

            if (mouse.Y > ScreenManager.GraphicsDevice.Viewport.Height - C.camMouseScrollBorderWidth && mouse.Y < ScreenManager.GraphicsDevice.Viewport.Height)
                G.cam.MoveDown(C.camKeyboardScrollSpeed);

            if (mouse.X < C.camMouseScrollBorderWidth && mouse.X > 0)
                G.cam.MoveLeft(C.camKeyboardScrollSpeed);

            if (mouse.X > ScreenManager.GraphicsDevice.Viewport.Width - C.camMouseScrollBorderWidth && mouse.X < ScreenManager.GraphicsDevice.Viewport.Width)
                G.cam.MoveRight(C.camKeyboardScrollSpeed);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (Mouse.GetState().X - oldMouse.X > 0)
                    G.cam.MoveLeft((Mouse.GetState().X - oldMouse.X) * (C.camMouseDragSpeed * 2 / G.cam.Zoom));
                else if (Mouse.GetState().X - oldMouse.X < 0)
                    G.cam.MoveRight(-((Mouse.GetState().X - oldMouse.X) * (C.camMouseDragSpeed * 2 / G.cam.Zoom))); //NOTE: If you want to change how mousedrag works,
                //It's probably better to change C.camDragSpeed than touch these method calls

                if (Mouse.GetState().Y - oldMouse.Y > 0)
                    G.cam.MoveUp((Mouse.GetState().Y - oldMouse.Y) * (C.camMouseDragSpeed * 2 / G.cam.Zoom));
                else if (Mouse.GetState().Y - oldMouse.Y < 0)
                    G.cam.MoveDown(-((Mouse.GetState().Y - oldMouse.Y) * (C.camMouseDragSpeed * 2 / G.cam.Zoom))); //Drag is slower if we are zoomed in

            }

            if (Mouse.GetState().ScrollWheelValue != oldMouse.ScrollWheelValue)
            {
                //Zoom for close distance
                if (G.cam.Zoom > C.camZoomSpeedThreshold)
                {
                    G.cam.Zoom += (Mouse.GetState().ScrollWheelValue - oldMouse.ScrollWheelValue) / 120.0f / C.camMouseCloseZoomSpeed;
                }
                //zoom for far away
                else
                {
                    G.cam.Zoom *= 1 + (Mouse.GetState().ScrollWheelValue - oldMouse.ScrollWheelValue) / 120.0f / C.camMouseFarZoomSpeed; //120.0f is some weird magic number..
                }
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                G.cam.Rotation += ((Mouse.GetState().X - oldMouse.X) / C.camMouseRotateSpeed);
                G.cam.Rotation += ((Mouse.GetState().Y - oldMouse.Y) / C.camMouseRotateSpeed);
            }

        }
    }
}
