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
using TouchAndPlay.screens;
using TouchAndPlay.input;

namespace TouchAndPlay
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        const int APP_WIDTH = GameConfig.APP_WIDTH;
        const int APP_HEIGHT = GameConfig.APP_HEIGHT;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KinectManager kinector;

        //define your custom screens here
        private SplashScreen splashScreen;
        private StageScreen stageScreen;
        private MenuScreen menuScreen;
        private TestScreen aboutScreen;
        private CartesianTestScreen cartesianTestScreen;
        private CreateProfileScreen createProfileScreen;


        private ScreenState currentScreen;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = APP_WIDTH;
            graphics.PreferredBackBufferHeight = APP_HEIGHT;

            kinector = new KinectManager(APP_WIDTH, APP_HEIGHT, this.graphics);

            //instantiate your custom screens here
            stageScreen = new StageScreen(kinector);
            splashScreen = new SplashScreen();
            menuScreen = new MenuScreen(Color.Beige);
            aboutScreen = new TestScreen();
            cartesianTestScreen = new CartesianTestScreen();
            createProfileScreen = new CreateProfileScreen();

            menuScreen.setScreenColor(Color.Olive);

            graphics.ToggleFullScreen();

            IsMouseVisible = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            kinector.Initialize();

            //change tracking mode here
            kinector.setTrackingMode(Microsoft.Kinect.SkeletonTrackingMode.Default);

            currentScreen = ScreenState.SPLASH_SCREEN;

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

            //load the screen's contents here
            splashScreen.LoadContent(this.Content);
            kinector.LoadContent(this.Content);
            stageScreen.LoadContent(this.Content);

            createProfileScreen.LoadContent(this.Content);
            createProfileScreen.createScreen();
            
            cartesianTestScreen.LoadContent(this.Content);
            cartesianTestScreen.createScreen();

            aboutScreen.LoadContent(this.Content);
            aboutScreen.createSreen();

            menuScreen.LoadContent(this.Content);
            menuScreen.createScreen();
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
                kinector.Stop();
            }

            UpdateInputDevices();
            UpdateScreenState();

            base.Update(gameTime);
        }

        private void UpdateScreenState()
        {
            switch (currentScreen)
            {
                case ScreenState.SPLASH_SCREEN:
                    splashScreen.Update();
                    if (splashScreen.getCurrentState() == SplashScreenState.GOTO_NEXT_SCREEN && splashScreen.getTransitionState() == BasicScreenState.GO_TO_TARGET_SCREEN )
                    {
                        currentScreen = ScreenState.MENU_SCREEN;
                        splashScreen.Dispose();
                    }
                    break;
                case ScreenState.MENU_SCREEN:
                    menuScreen.Update();

                    if (menuScreen.targetScreen != ScreenState.MENU_SCREEN && menuScreen.getTransitionState() == BasicScreenState.GO_TO_TARGET_SCREEN )
                    {
                        currentScreen = menuScreen.targetScreen;
                    }
                    break;
                case ScreenState.STAGE_SCREEN:
                    stageScreen.Update();

                    if (stageScreen.targetScreen != ScreenState.MENU_SCREEN && stageScreen.getTransitionState() == BasicScreenState.GO_TO_TARGET_SCREEN )
                    {
                        currentScreen = menuScreen.targetScreen;
                    }
                    break;
                case ScreenState.TEST_SCREEN:
                    aboutScreen.Update();
                    if (aboutScreen.targetScreen != ScreenState.TEST_SCREEN)
                    {
                        currentScreen = aboutScreen.targetScreen;
                    }
                    break;
                case ScreenState.CARTESIAN_TEST_SCREEN:
                    cartesianTestScreen.Update();
                    if (cartesianTestScreen.targetScreen != ScreenState.CARTESIAN_TEST_SCREEN)
                    {
                        currentScreen = cartesianTestScreen.targetScreen;
                    }
                    break;
                case ScreenState.CREATE_PROFILE_SCREEN:
                    createProfileScreen.Update();
                    if (createProfileScreen.targetScreen != ScreenState.CREATE_PROFILE_SCREEN)
                    {
                        currentScreen = createProfileScreen.targetScreen;
                    }
                    break;
            }

           
        }

        private void UpdateInputDevices()
        {
            MyMouse.update();
            MyKeyboard.update();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
  
            switch (currentScreen)
            {
                case ScreenState.SPLASH_SCREEN:
                    splashScreen.Draw(spriteBatch);
                    break;
                case ScreenState.STAGE_SCREEN:
                    kinector.DrawVideo(spriteBatch);
                    //kinector.DrawSkeleton(spriteBatch);
                    stageScreen.Draw(spriteBatch);
                    break;
                case ScreenState.MENU_SCREEN:
                    menuScreen.Draw(spriteBatch);
                    break;
                case ScreenState.TEST_SCREEN:
                    aboutScreen.Draw(spriteBatch);
                    break;
                case ScreenState.CARTESIAN_TEST_SCREEN:
                    cartesianTestScreen.Draw(spriteBatch);
                    break;
                case ScreenState.CREATE_PROFILE_SCREEN:
                    createProfileScreen.Draw(spriteBatch);
                    break;

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
