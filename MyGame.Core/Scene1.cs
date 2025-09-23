using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine;
using MyEngine.Components;
using MyEngine.GameObjects;
using MyEngine.Managers;
using MyEngine.Utils;
using MyEngine.Utils.Tween;
using MyGame.Core.Localization;

namespace MyGame.Core;

public class Scene1 : Scene
{

    private GameObject a;
    private GameObject b;

    private Texture2D test;
    
    public Scene1()
    {
    }

    /// <summary>
    /// Initializes the game, including setting up localization and adding the 
    /// initial screens to the ScreenManager.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        // // Load supported languages and set the default language.
        // List<CultureInfo> cultures = LocalizationManager.GetSupportedCultures();
        // var languages = new List<CultureInfo>();
        // for (int i = 0; i < cultures.Count; i++)
        // {
        //     languages.Add(cultures[i]);
        // }
        // // TODO You should load this from a settings file or similar,
        // // based on what the user or operating system selected.
        // var selectedLanguage = LocalizationManager.DEFAULT_CULTURE_CODE;
        // LocalizationManager.SetCulture(selectedLanguage);
        //
        
        
        MainCamera = Instantiate().AddComponent<Camera>();
        MainCamera.Transform.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
    }

    protected override void LoadContent()
    {
        // test = Content.Load<Texture2D>("Sprites/test");
        
        
        //
        // a = Instantiate();
        // a.Transform.Position = new Vector2(100.0f, 100.0f);
        // a.Transform.Scale = Vector2.One * 0.2f;
        // Sprite asprite = a.AddComponent<Sprite>();
        // asprite.Texture = test;
        //
        // b = Instantiate();
        // b.Transform.Position = new Vector2(-100.0f, 300.0f);
        // a.Transform.AddChild(b.Transform);
        // var bsprite = b.AddComponent<Sprite>();
        // bsprite.Texture = test;
    }

    public override void UnloadContent()
    {
        
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        
        DebugDraw.DrawCircle(MainCamera.ScreenToWorld(Mouse.GetState().Position), Color.White, 80.0f);

        // (float sin, float cos) = MathF.SinCos(MathHelper.ToRadians((float)(gameTime.TotalGameTime.TotalSeconds * 20.0f)));
        // Console.WriteLine(sin);
        // MainCamera.Transform.Position = new Vector2(sin, 0.0F) * 200.0f;

        // TODO: Add your update logic here
        
        // a.Transform.Rotation += MathHelper.ToRadians((float)(15.0f * gameTime.ElapsedGameTime.TotalSeconds));
        // b.Transform.Rotation += MathHelper.ToRadians((float)(15.0f * gameTime.ElapsedGameTime.TotalSeconds));

    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}