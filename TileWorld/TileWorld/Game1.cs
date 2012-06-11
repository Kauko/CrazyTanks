using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Tileworld.Camera;
using Tileworld.Utility;
using Tileworld.Logging;

namespace Tileworld
{

    public enum GameState { playing, paused };
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState oldKeyboard;
        MouseState oldMouse;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Window.Title = "Tile World";
            this.IsMouseVisible = true;          
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            GameServices.AddService<Camera2d>(new Camera2d());
            GameServices.AddService<Logger>(new Logger());
            GameServices.AddService<GraphicsDevice>(graphics.GraphicsDevice);

            G.gameState = GameState.playing;

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            //graphics.PreferredBackBufferWidth = device.DisplayMode.Width;
            //graphics.PreferredBackBufferHeight = device.DisplayMode.Height;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureRefs.koala = this.Content.Load<Texture2D>("Placeholder/Koala");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            GameServices.GetService<Logger>().gameTime = gameTime;

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();
            switch(G.gameState){
                case GameState.paused:
                    break;
                case GameState.playing:
                    if(this.IsActive)
                        GameServices.GetService<Camera2d>().updateCamera(keyboard, mouse);
                    GameServices.GetService<Logger>().logFPS();
                    break;
            }


            oldMouse = mouse;
            oldKeyboard = keyboard;
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, GameServices.GetService<Camera2d>().get_transformation());
            spriteBatch.Draw(TextureRefs.koala, new Vector2(0.0f, 0.0f), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
