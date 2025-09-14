using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine.Utils;

namespace MyEngine
{
    public class Core : Game
    {
        internal static Core s_instance;
        
        /// <summary>
        /// Gets a reference to the Core instance.
        /// </summary>
        public static Core Instance => s_instance;

        /// <summary>
        /// Gets the graphics device manager to control the presentation of graphics.
        /// </summary>
        public static GraphicsDeviceManager Graphics { get; private set; }

        /// <summary>
        /// Gets the graphics device used to create graphical resources and perform primitive rendering.
        /// </summary>
        public static new GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Gets the sprite batch used for all 2D rendering.
        /// </summary>
        public static SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Gets the content manager used to load global assets.
        /// </summary>
        public static new ContentManager Content { get; private set; }
        
        // Resources for drawing.
        private GraphicsDeviceManager graphicsDeviceManager;

        /// <summary>
        /// Initializes a new instance of the game. Configures platform-specific settings, 
        /// initializes services like settings and leaderboard managers, and sets up the 
        /// screen manager for screen transitions.
        /// </summary>
        public Core(string title = "My Game Title", int width = 800, int height = 600, bool fullScreen = false)
        {
            // Ensure that multiple cores are not created.
            if (s_instance != null)
            {
                throw new InvalidOperationException($"Only a single Core instance can be created");
            }

            // Store reference to engine for global member access.
            s_instance = this;

            // Create a new graphics device manager.
            Graphics = new GraphicsDeviceManager(this);

            // Set the graphics defaults.
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.IsFullScreen = fullScreen;
            
            // Share GraphicsDeviceManager as a service.
            Services.AddService(typeof(GraphicsDeviceManager), Graphics);

            // Apply the graphic presentation changes.
            Graphics.ApplyChanges();

            // Set the window title.
            Window.Title = title;

            // Set the core's content manager to a reference of the base Game's
            // content manager.
            Content = base.Content;

            // Set the root directory for content.
            Content.RootDirectory = "Content";

            // Mouse is visible by default.
            IsMouseVisible = true;
        }

        /// <summary>
        /// Initializes the game, including setting up localization and adding the 
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            
            // Set the core's graphics device to a reference of the base Game's
            // graphics device.
            GraphicsDevice = base.GraphicsDevice;

            // Create the sprite batch instance.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

        }

        /// <summary>
        /// Loads game content, such as textures and particle systems.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            
            DebugDraw.Instance.LoadContent(GraphicsDevice, Content);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Please put this after you already draw everything so that debug draw is on top of everything.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            DebugDraw.Instance.Draw(SpriteBatch);
        }
    }
}
