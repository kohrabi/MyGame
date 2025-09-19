using System;
using System.Collections.Generic;
using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;
using MyEngine.GameObjects;
using MyEngine.Managers;
using MyEngine.Utils;

namespace MyEngine
{
    public enum WindowScaleMode
    {
        Default,
        MaintainAspectRatio,
        Stretch,
    }
    
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
        public static new GraphicsDeviceManager Graphics { get; private set; }

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
        public new static ContentManager Content { get; private set; }
        
        // public static RenderTarget2D RenderTarget { get; private set; } 
        
        // Resources for drawing.
        private GraphicsDeviceManager graphicsDeviceManager;
        
        private List<GlobalManager> _managers = new List<GlobalManager>();

        /// <summary>
        /// Initializes a new instance of the game. Configures platform-specific settings, 
        /// initializes services like settings and leaderboard managers, and sets up the 
        /// screen manager for screen transitions.
        /// </summary>
        public Core(string title = "My Game Title", int width = 1280, int height = 720, bool fullScreen = false)
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
            Graphics.IsFullScreen = fullScreen;
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Window.AllowUserResizing = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 120d); // 120 FPS
            
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
            // Set the core's graphics device to a reference of the base Game's
            // graphics device.
            GraphicsDevice = base.GraphicsDevice;

            // Create the sprite batch instance.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Before LoadContent
            
            base.Initialize();
            
            // After LoadContent
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
            base.UnloadContent();
        }

        private float time = 0;
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            foreach (var manager in _managers)
                manager.Update(gameTime);
        }

        /// <summary>
        /// Please put this after you already draw everything so that debug draw is on top of everything.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            // base.Draw(gameTime);
            //
            // SpriteBatch.GraphicsDevice.SetRenderTarget(null);
            //
            // DebugDraw.Instance.Draw(SpriteBatch);
            //
            // // Call BeforeLayout first to set things up
            // _imGuiRenderer.BeforeLayout(gameTime);
            //
            // // Draw our UI
            // ImGuiLayout();
            //
            // // Call AfterLayout now to finish up and draw all the things
            // _imGuiRenderer.AfterLayout();
            
        }

        public static void RegisterManager(GlobalManager manager)
        {
            Instance._managers.Add(manager);
            manager.Enabled = true;
        }
        
        
        public static void UnregisterManager(GlobalManager manager)
        {
            Instance._managers.Remove(manager);
        }

        public static T GetGlobalManager<T>() where T : GlobalManager
        {
            return Instance._managers.Find((manager) => manager is T) as T;
        }
    }
}
