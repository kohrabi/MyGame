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
        GameObject gameObject = new GameObject("Hello");
        MainCamera = gameObject.AddComponent<Camera>();
        
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
        
        a = new GameObject();
        a.Transform.Position = new Vector2(100.0f, 100.0f);
        a.Transform.Scale = Vector2.One * 0.2f;
        Sprite asprite = a.AddComponent<Sprite>();
        asprite.Texture = test;
        
        b = new GameObject();
        b.Transform.Position = new Vector2(-100.0f, 300.0f);
        a.Transform.AddChild(b.Transform);
        var bsprite = b.AddComponent<Sprite>();
        bsprite.Texture = test;
        
        MainCamera.Transform.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
    }

    public override void LoadContent()
    {
        test = Content.Load<Texture2D>("Sprites/test");
    }

    public override void UnloadContent()
    {
        
    }

    public override void Update(GameTime gameTime)
    {
        DebugDraw.DrawCircle(Vector2.Zero, Color.White, 256.0f);
        DebugDraw.DrawString("This is a debug draw", 
            new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height / 2.0f), 
            Color.Red,
            Vector2.One * 2.0f);

        // (float sin, float cos) = MathF.SinCos(MathHelper.ToRadians((float)(gameTime.TotalGameTime.TotalSeconds * 20.0f)));
        // Console.WriteLine(sin);
        // MainCamera.Transform.Position = new Vector2(sin, 0.0F) * 200.0f;

        // TODO: Add your update logic here
        a.Transform.Rotation += MathHelper.ToRadians((float)(15.0f * gameTime.ElapsedGameTime.TotalSeconds));
        b.Transform.Rotation += MathHelper.ToRadians((float)(15.0f * gameTime.ElapsedGameTime.TotalSeconds));

    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        MainCamera.Begin(SpriteBatch);
        
        a.DrawComponents(SpriteBatch, gameTime);
        b.DrawComponents(SpriteBatch, gameTime);
        
        MainCamera.End(SpriteBatch);

    }
}