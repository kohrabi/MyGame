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
        }

        /// <summary>
        /// Initializes the game, including setting up localization and adding the 
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            scene1 = new Scene1();
            SceneManager.LoadScene(scene1);
        }

        
    }
}