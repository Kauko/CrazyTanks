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
using Solum.Camera;
using Solum.Utility;
using Solum.Logging;
using Solum.Input;
using Solum.Menu;

namespace Solum
{

    public enum GameState { playing, paused, menu };
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MenuManager menuManager;

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
            GameServices.AddService<GraphicsDevice>(graphics.GraphicsDevice);
            GameServices.AddService<KeyboardDevice>(new KeyboardDevice());
            GameServices.AddService<MouseDevice>(new MouseDevice());

            GameServices.AddService<Camera2d>(new Camera2d());
            GameServices.AddService<Logger>(new Logger());

            menuManager = new MenuManager();

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


            //We create our menus here.
            string BGImage = "Graphics\\Backgrounds\\MainMenuBG";
            string BGM = "Audio\\MainMenuBGM";
            string menuOpenPath = "Audio\\MenuOpen";
            string menuClosePath = "Audio\\MenuClose";

            /*MainMenu mainMenu = new MainMenu("Main Menu");
            mainMenu.Load(Content, BGImage, BGM, menuOpenPath, menuClosePath);
            mainMenu.LoadButtons(Content,
                new int[] { 1, 2, 3 },
                new List<Rectangle>() { new Rectangle(325, 150, 150, 50), new Rectangle(325, 210, 150, 50), new Rectangle(325, 270, 150, 50) },
                new List<string>() { "Continue", "Save", "Quit" }
                );
            menuManager.AddMenu("Main Menu", mainMenu);

            SaveMenu save = new SaveMenu("Save Menu");
            save.Load(Content, BGImage, BGM, menuOpenPath, menuClosePath);
            save.LoadButtons(Content,
                new int[] { 1, 2 },
                new List<Rectangle>() { new Rectangle(325, 150, 150, 50), new Rectangle(325, 210, 150, 50) },
                new List<string>() { "Return", "Go Deeper" });
            menuManager.AddMenu("Save Menu", save);

            DeepMenu deep = new DeepMenu("Deep Menu");
            deep.Load(Content, BGImage, BGM, menuOpenPath, menuClosePath);
            deep.LoadButtons(Content,
                new int[] { 1 },
                new List<Rectangle>() { new Rectangle(325, 150, 150, 50) },
                new List<string>() { "Return" });
            menuManager.AddMenu("Deep Menu", deep);*/
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
            GameServices.GetService<Logger>().gameTime = gameTime;
            GameServices.GetService<KeyboardDevice>().Update();
            GameServices.GetService<MouseDevice>().Update();
            GameServices.GetService<GamepadDevice>().Update();

            if (GameServices.GetService<KeyboardDevice>().State.IsKeyDown(Keys.Escape))
                Exit();

            switch(G.gameState){
                case GameState.menu:
                    break;
                case GameState.paused:
                    break;
                case GameState.playing:
                    if(this.IsActive)
                        GameServices.GetService<Camera2d>().updateCamera();
                    GameServices.GetService<Logger>().logFPS();
                    break;
            }
            
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
