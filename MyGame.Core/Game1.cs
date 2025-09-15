using System;
using MyGame.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public Game1()
            : base("My Game Title", 800, 600, false)
        {
        }

        /// <summary>
        /// Initializes the game, including setting up localization and adding the 
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Load supported languages and set the default language.
            List<CultureInfo> cultures = LocalizationManager.GetSupportedCultures();
            var languages = new List<CultureInfo>();
            for (int i = 0; i < cultures.Count; i++)
            {
                languages.Add(cultures[i]);
            }

            // TODO You should load this from a settings file or similar,
            // based on what the user or operating system selected.
            var selectedLanguage = LocalizationManager.DEFAULT_CULTURE_CODE;
            LocalizationManager.SetCulture(selectedLanguage);
        }

        private Texture2D test;
        protected override void LoadContent()
        {
            base.LoadContent();
            
            test = Content.Load<Texture2D>("Sprites/splash");
        }

        protected override void Update(GameTime gameTime)
        {
            // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            DebugDraw.DrawCircle(Vector2.Zero, Color.White, 256.0f);
            DebugDraw.DrawString("This is a debug draw", 
                new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height / 2.0f), 
                Color.Red,
                Vector2.One * 2.0f);

            (float sin, float cos) = MathF.SinCos(MathHelper.ToRadians((float)(gameTime.TotalGameTime.TotalSeconds * 20.0f)));
            Console.WriteLine(sin);
            MainCamera.Transform.Position = new Vector2(sin, 0.0F) * 200.0f;

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin(transformMatrix: MainCamera.Transform.WorldMatrix);
            
            SpriteBatch.Draw(test, Vector2.Zero, Color.White);
            SpriteBatch.Draw(test, new Vector2(-300.0f, 300.0f), Color.White);
            
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}