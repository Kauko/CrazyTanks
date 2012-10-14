/// By Teemu Kaukoranta, member of the Oulu GamedevClub Stage
/// http://www.gamedevcenter.org
/// 
/// Part of the S.o.l.u.m project
/// Licensed under WTFPL - Do What The Fuck You Want To Public License
/// It would be nice if you don't remove this comment section though


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
using Solum.Menus;
using Solum.SharedTanks;

namespace Solum
{

    public enum GameState { playing, paused, menu, playerSelection, controllerDisconnected };
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MenuManager menuManager;
        MenuManager pauseMenuManager;
        PlayerSelectionMenu playerSelectionMenu;

        int winCounter = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Window.Title = "Shared Tanks - Stage Fall Gamejam 2012";
            this.IsMouseVisible = false;          
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
            //GameServices.AddService<KeyboardDevice>(new KeyboardDevice());
            //GameServices.AddService<MouseDevice>(new MouseDevice());
            GameServices.AddService<Game>(this);
            GameServices.AddService<GamepadDevice>(new GamepadDevice(PlayerIndex.One));
            //if(GamePad.GetState(PlayerIndex.One).IsConnected)
            G.gamePadOne = new GamepadDevice(PlayerIndex.One);
            //if (GamePad.GetState(PlayerIndex.Two).IsConnected)
            G.gamePadTwo = new GamepadDevice(PlayerIndex.Two);
            //if (GamePad.GetState(PlayerIndex.Three).IsConnected)
            G.gamePadThree = new GamepadDevice(PlayerIndex.Three);
            //if (GamePad.GetState(PlayerIndex.Four).IsConnected)
            G.gamePadFour = new GamepadDevice(PlayerIndex.Four);

            GameServices.AddService<Camera2d>(new Camera2d());
            GameServices.AddService<Logger>(new Logger(false));
            GameServices.AddService<GridManager>(new GridManager());
            GameServices.AddService<TankManager>(new TankManager());
            GameServices.AddService<BulletManager>(new BulletManager());

            menuManager = new MenuManager();
            pauseMenuManager = new MenuManager();
            playerSelectionMenu = new PlayerSelectionMenu();

            G.gameState = GameState.menu;

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

            #region Content paths
            TextureRefs.bullet = this.Content.Load<Texture2D>("Placeholder/Images/Bullet");
            TextureRefs.tank = this.Content.Load<Texture2D>("Placeholder/Images/Tank2");
            TextureRefs.turret = this.Content.Load<Texture2D>("Placeholder/Images/Turret2");
            TextureRefs.shield = this.Content.Load<Texture2D>("Placeholder/Images/Shield2");
            TextureRefs.koala = this.Content.Load<Texture2D>("Placeholder/Images/Koala");
            TextureRefs.menuBgImage = this.Content.Load<Texture2D>("Placeholder/Images/MainMenuBG");
            TextureRefs.menuButton = this.Content.Load<Texture2D>("Placeholder/Images/Button");
            TextureRefs.activeMenuButton = this.Content.Load<Texture2D>("Placeholder/Images/ActiveButton");
            TextureRefs.ready = this.Content.Load<Texture2D>("Placeholder/Images/Ready");
            TextureRefs.pressStart = this.Content.Load<Texture2D>("Placeholder/Images/PressStart");
            TextureRefs.waitingReady = this.Content.Load<Texture2D>("Placeholder/Images/WaitingReady");
            TextureRefs.SmartBombPickup = this.Content.Load<Texture2D>("Placeholder/Images/SmartBomb");
            TextureRefs.Wall = this.Content.Load<Texture2D>("Placeholder/Images/Wall");
            TextureRefs.RotatedRectangle = this.Content.Load<Texture2D>("Placeholder/Images/RotatedRectangle");
            TextureRefs.BombIndicator = this.Content.Load<Texture2D>("Placeholder/Images/BombIndicator");
            TextureRefs.ShieldIndicator = this.Content.Load<Texture2D>("Placeholder/Images/ShieldIndicator");
            TextureRefs.Frame= this.Content.Load<Texture2D>("Placeholder/Images/Frame");

            SpriteFontRefs.textFont = Content.Load<SpriteFont>("Placeholder/Fonts/textFont");
            SpriteFontRefs.titleFont = Content.Load<SpriteFont>("Placeholder/Fonts/titleFont");

            SoundRefs.menuOpen = Content.Load<SoundEffect>("Placeholder/Audio/MenuOpen");
            SoundRefs.menuClose = Content.Load<SoundEffect>("Placeholder/Audio/MenuClose");

            SoundRefs.bgm = Content.Load<Song>("Placeholder/Audio/MainMenuBGM");

            LevelRefs.levelOne = Content.Load<LevelLibrary.Level>("Levels/level1");
            G.levels.Add(LevelRefs.levelOne);
            #endregion

            GameServices.GetService<GridManager>().loadMap();

            #region screen Center location..
            int screenHeight = graphics.GraphicsDevice.Viewport.Height;
            int screenWidth = graphics.GraphicsDevice.Viewport.Width;

            int screenVerticalCenter = screenHeight / 2;
            int screenHorizontalCenter = screenWidth / 2;

            int firstY = screenVerticalCenter - C.menuButtonVerticalOffset - (C.menuButtonHeight / 2);
            int firstX = screenHorizontalCenter - (C.menuButtonWidth / 2);
            #endregion

            #region building main menu
            MainMenu mainMenu = new MainMenu("Main Menu");
            mainMenu.LoadButtons(
                new ButtonAction[] { ButtonAction.PlayerSelect, ButtonAction.Other, ButtonAction.Quit },
                new List<Rectangle>() { new Rectangle(firstX, firstY, C.menuButtonWidth, C.menuButtonHeight), new Rectangle(firstX, firstY + C.menuButtonVerticalAddition, C.menuButtonWidth, C.menuButtonHeight), new Rectangle(firstX, firstY + C.menuButtonVerticalAddition * 2, C.menuButtonWidth, C.menuButtonHeight) },
                new List<string>() { "Start", "Save", "Quit" }
                );
            menuManager.AddMenu("Main Menu", mainMenu);

            MainMenu save = new MainMenu("Save Menu");
            save.LoadButtons(
                new ButtonAction[] { ButtonAction.Close, ButtonAction.Other },
                new List<Rectangle>() { new Rectangle(firstX, firstY, C.menuButtonWidth, C.menuButtonHeight), new Rectangle(firstX, firstY + C.menuButtonVerticalAddition, C.menuButtonWidth, C.menuButtonHeight) },
                new List<string>() { "Return", "Go Deeper" });
            menuManager.AddMenu("Save", save);

            MainMenu deep = new MainMenu("Deep Menu");
            deep.LoadButtons(
                new ButtonAction[] { ButtonAction.Close},
                new List<Rectangle>() { new Rectangle(firstX, firstY, C.menuButtonWidth, C.menuButtonHeight) },
                new List<string>() { "Return" });
            menuManager.AddMenu("Go Deeper", deep);
            #endregion

            #region building pause menu
            PauseMenu mainPauseMenu = new PauseMenu("Pause menu");
            mainPauseMenu.LoadButtons(
                new ButtonAction[] { ButtonAction.Play, ButtonAction.Quit },
                new List<Rectangle>() { new Rectangle(firstX, firstY, C.menuButtonWidth, C.menuButtonHeight), new Rectangle(firstX, firstY + C.menuButtonVerticalAddition, C.menuButtonWidth, C.menuButtonHeight) },
                new List<string>() { "Continue", "Main Menu" });
            pauseMenuManager.AddMenu("Pause Menu", mainPauseMenu);
            #endregion

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
            //GameServices.GetService<KeyboardDevice>().Update();
            //GameServices.GetService<MouseDevice>().Update();
            GameServices.GetService<GamepadDevice>().Update();
            updateGamepads();

            if (menuManager.MenuState == MenuManager.MenuStates.Exit)
                Exit();

            switch(G.gameState){
                case GameState.menu:
                    //If we dont reset pausemenumanagers state, the pause menu will keep on exiting when we access it
                    pauseMenuManager.MenuState = MenuManager.MenuStates.None;
                    if (menuManager.ActiveMenu == null)
                        menuManager.Show("Main Menu");
                    //if(G.gamePadOne.IsConnected)
                    menuManager.Update();
                    break;
                case GameState.playerSelection:
                    playerSelectionMenu.Update();
                    break;
                case GameState.paused:
                    if(pauseMenuManager.ActiveMenu == null)
                        pauseMenuManager.Show("Pause Menu");
                    pauseMenuManager.Update();
                    if (pauseMenuManager.MenuState == MenuManager.MenuStates.Exit)
                    {
                        G.gameState = GameState.menu;
                        G.activeGamepads = new List<GamepadDevice>();
                    }
                    break;
                case GameState.controllerDisconnected:
                    bool allConnected = true;
                    foreach(GamepadDevice d in G.activeGamepads){
                        if (!d.IsConnected)
                            allConnected = false;
                    }
                    if (allConnected)
                        G.gameState = GameState.playing;
                    break;
                case GameState.playing:
                    pauseMenuManager.MenuState = MenuManager.MenuStates.None;
                    if (GameServices.GetService<TankManager>().tanks.Count == 0)
                        GameServices.GetService<TankManager>().initTanks();
                   //GameServices.GetService<Logger>().logMsg("Playing");
                    if (this.IsActive)
                    {
                        //GameServices.GetService<Camera2d>().updateCamera();
                        

                    }/*else
                        G.gameState = GameState.paused;*/
                    GameServices.GetService<TankManager>().Update();
                    GameServices.GetService<Logger>().logFPS();
                    GameServices.GetService<GridManager>().Update();
                    GameServices.GetService<BulletManager>().Update();
                    foreach (GamepadDevice d in G.activeGamepads)
                    {
                        if (d.WasButtonPressed(Buttons.Start))
                        {
                            G.gameState = GameState.paused;
                        }
                    }
                    if (someoneWins())
                    {
                        winCounter++;
                    }
                    if (winCounter >= C.winWaitTime)
                        G.gameState = GameState.menu;
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
            GraphicsDevice.Clear(Color.DimGray);
            switch(G.gameState){
                case GameState.menu:
                    spriteBatch.Begin();
                    menuManager.Draw(spriteBatch);
                    break;
                case GameState.playerSelection:
                    spriteBatch.Begin();
                    playerSelectionMenu.Draw(spriteBatch);
                    break;
                case GameState.paused:
                    //All of playing draws here too;
                    spriteBatch.Begin();
                    GameServices.GetService<TankManager>().Draw(spriteBatch);
                    GameServices.GetService<GridManager>().Draw(spriteBatch);
                    GameServices.GetService<BulletManager>().Draw(spriteBatch);
                    spriteBatch.Draw(TextureRefs.Frame, new Vector2(0, 0), Color.White);
                    DrawFrame(spriteBatch);
                    //Drawing the pause menu on top
                    pauseMenuManager.Draw(spriteBatch);                 
                    break;
                case GameState.controllerDisconnected:
                    var rect = new Texture2D(GameServices.GetService<GraphicsDevice>(), 1, 1);
                    rect.SetData(new[] { Color.Black });
                    spriteBatch.Draw(rect, new Rectangle(0, 0, GameServices.GetService<GraphicsDevice>().Viewport.Width, GameServices.GetService<GraphicsDevice>().Viewport.Height), Color.White*0.5f);
                    Vector2 titlePosition = new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width / 2 - (SpriteFontRefs.titleFont.MeasureString("Controller disconnected").X / 2), 50);
                    spriteBatch.DrawString(SpriteFontRefs.titleFont, "Controller disconnected", titlePosition, Color.Black);
                    break;
                case GameState.playing:
                    spriteBatch.Begin();
                    GameServices.GetService<GridManager>().Draw(spriteBatch);
                    spriteBatch.Draw(TextureRefs.Frame, new Vector2(0, 0), Color.White);
                    DrawFrame(spriteBatch);
                    GameServices.GetService<TankManager>().Draw(spriteBatch);
                    GameServices.GetService<BulletManager>().Draw(spriteBatch);
                    break;
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawFrame(SpriteBatch spriteBatch)
        {
            float height, alpha = C.indicatorTransparency;
            foreach (GamepadDevice d in G.activeGamepads)
            {
                if (d.PlayerIndex == PlayerIndex.One)
                {
                    //Red
                    height = TextureRefs.ShieldIndicator.Height * G.redOneShield;
                    alpha = C.indicatorTransparency;
                    if (G.redOneShield == 1.0)
                        alpha = 1.0f;
                    spriteBatch.Draw(TextureRefs.ShieldIndicator, new Vector2(0, 0), new Rectangle(0, 0, TextureRefs.ShieldIndicator.Width, (int)height), Color.White * alpha);

                    height = TextureRefs.ShieldIndicator.Height * G.redTwoShield;
                    alpha = C.indicatorTransparency;
                    if (G.redTwoShield == 1.0)
                        alpha = 1.0f;
                    spriteBatch.Draw(TextureRefs.ShieldIndicator, new Vector2(TextureRefs.ShieldIndicator.Width + TextureRefs.BombIndicator.Width, 0), new Rectangle(0, 0, TextureRefs.ShieldIndicator.Width, (int)height), Color.White * alpha);

                    spriteBatch.DrawString(SpriteFontRefs.titleFont, "K" + G.redKills + "D" + G.redDeaths, new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width / 2 - C.KDCounterOffsetLeft, 0), Color.Black);
                    continue;
                }

                if (d.PlayerIndex == PlayerIndex.Two)
                {
                    //Blue
                    height = TextureRefs.ShieldIndicator.Height * G.blueOneShield;
                    alpha = C.indicatorTransparency;
                    if (G.blueOneShield == 1.0)
                        alpha = 1.0f;
                    spriteBatch.Draw(TextureRefs.ShieldIndicator,
                        new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width - TextureRefs.ShieldIndicator.Width, 0),
                        new Rectangle(0, 0, TextureRefs.ShieldIndicator.Width, (int)height), Color.White * alpha);

                    height = TextureRefs.ShieldIndicator.Height * G.blueTwoShield;
                    alpha = C.indicatorTransparency;
                    if (G.blueTwoShield == 1.0)
                        alpha = 1.0f;
                    spriteBatch.Draw(TextureRefs.ShieldIndicator,
                        new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width - TextureRefs.ShieldIndicator.Width
                            - TextureRefs.ShieldIndicator.Width - TextureRefs.BombIndicator.Width, 0),
                        new Rectangle(0, 0, TextureRefs.ShieldIndicator.Width, (int)height), Color.White * alpha);

                    spriteBatch.DrawString(SpriteFontRefs.titleFont, "K" + G.blueKills + "D" + G.blueDeaths, new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width / 2 + C.KDCounterOffsetRight, 0), Color.Black);
                    continue;
                }

                if (d.PlayerIndex == PlayerIndex.Three)
                {
                    //green
                    height = TextureRefs.ShieldIndicator.Height * G.greenOneShield;
                    alpha = C.indicatorTransparency;
                    if (G.greenOneShield == 1.0)
                        alpha = 1.0f;
                    spriteBatch.Draw(TextureRefs.ShieldIndicator,
                        new Vector2(0, GameServices.GetService<GraphicsDevice>().Viewport.Height - TextureRefs.ShieldIndicator.Height),
                        new Rectangle(0, 0, TextureRefs.ShieldIndicator.Width, (int)height), Color.White * alpha);

                    height = TextureRefs.ShieldIndicator.Height * G.greenTwoShield;
                    alpha = C.indicatorTransparency;
                    if (G.greenTwoShield == 1.0)
                        alpha = 1.0f;
                    spriteBatch.Draw(TextureRefs.ShieldIndicator,
                        new Vector2(TextureRefs.ShieldIndicator.Width + TextureRefs.BombIndicator.Width, GameServices.GetService<GraphicsDevice>().Viewport.Height - TextureRefs.ShieldIndicator.Height),
                        new Rectangle(0, 0, TextureRefs.ShieldIndicator.Width, (int)height), Color.White * alpha);

                    spriteBatch.DrawString(SpriteFontRefs.titleFont, "K" + G.greenKills + "D" + G.greenDeaths, new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width / 2 - C.KDCounterOffsetLeft, GameServices.GetService<GraphicsDevice>().Viewport.Height-C.frameHeight), Color.Black);
                    continue;
                }

                if (d.PlayerIndex == PlayerIndex.Four)
                {
                    //yellow
                    height = TextureRefs.ShieldIndicator.Height * G.yellowOneShield;
                    alpha = C.indicatorTransparency;
                    if (G.yellowOneShield == 1.0)
                        alpha = 1.0f;
                    spriteBatch.Draw(TextureRefs.ShieldIndicator,
                        new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width - TextureRefs.ShieldIndicator.Width,
                            GameServices.GetService<GraphicsDevice>().Viewport.Height - TextureRefs.ShieldIndicator.Height),
                        new Rectangle(0, 0, TextureRefs.ShieldIndicator.Width, (int)height), Color.White * alpha);

                    height = TextureRefs.ShieldIndicator.Height * G.yellowTwoShield;
                    alpha = C.indicatorTransparency;
                    if (G.yellowTwoShield == 1.0)
                        alpha = 1.0f;
                    spriteBatch.Draw(TextureRefs.ShieldIndicator,
                        new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width - TextureRefs.ShieldIndicator.Width - TextureRefs.ShieldIndicator.Width - TextureRefs.BombIndicator.Width,
                            GameServices.GetService<GraphicsDevice>().Viewport.Height - TextureRefs.ShieldIndicator.Height - TextureRefs.ShieldIndicator.Height - TextureRefs.BombIndicator.Height),
                        new Rectangle(0, 0, TextureRefs.ShieldIndicator.Width, (int)height), Color.White * alpha);

                    spriteBatch.DrawString(SpriteFontRefs.titleFont, "K" + G.yellowKills + "D" + G.yellowDeaths, new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width / 2 + C.KDCounterOffsetRight, GameServices.GetService<GraphicsDevice>().Viewport.Height - C.frameHeight), Color.Black);
                    continue;
                }
            }
           
        }

        private void updateGamepads()
        {
            switch (G.gameState)
            {
                case GameState.menu:
                case GameState.playerSelection:
                    G.gamePadOne.Update();
                    G.gamePadTwo.Update();
                    G.gamePadThree.Update();
                    G.gamePadFour.Update();
                    break;
                case GameState.playing:
                case GameState.paused:
                case GameState.controllerDisconnected:
                    foreach (GamepadDevice d in G.activeGamepads)
                        d.Update();
                    break;
            }
        }

        private bool someoneWins()
        {
            int foo = 0;
            if (G.redDeaths >= C.allowedDeaths + 1)
                foo++;
            if (G.blueDeaths >= C.allowedDeaths + 1)
                foo++;
            if (G.greenDeaths >= C.allowedDeaths + 1)
                foo++;
            if (G.yellowDeaths >= C.allowedDeaths + 1)
                foo++;
            if (G.activeGamepads.Count == 1)
                return false;
            if (foo >= G.activeGamepads.Count - 1)
                return true;

            return false;
            
        }
    }
}
