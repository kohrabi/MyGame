using System;
using MyGame.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine;
using MyEngine.Components;
using MyEngine.GameObjects;
using MyEngine.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace MyGame.Core
{
    /// <summary>
    /// The main class for the game, responsible for managing game components, settings, 
    /// and platform-specific configurations.
    /// </summary>
    public class Game1 : MyEngine.Core
    {
        
        Scene1 scene1;
        
        public Game1()
            : base("My Game Title", 800, 600, false)
        {
            scene1 = new Scene1();
        }

        /// <summary>
        /// Initializes the game, including setting up localization and adding the 
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            scene1.Initialize();
            

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            scene1.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            
            scene1.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            scene1.Update(gameTime);
            
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            scene1.Draw(gameTime);
            
        }
        
    }
}