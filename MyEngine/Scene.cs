using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine.Components;
using MyEngine.GameObjects;
using MyEngine.Utils;

namespace MyEngine;

public abstract class Scene : IDisposable
{
    private ContentManager content;
    private List<GameObject> gameObjects;
    private List<GameObject> uninitGameObjects;
    private Camera _mainCamera;
    
    public Camera MainCamera
    {
        get => _mainCamera;
        set => _mainCamera = value;
    }

    public Color BackgroundColor = Color.CornflowerBlue;
    
    protected ContentManager Content { get; }
   
    protected List<GameObject> GameObjects => gameObjects;
    // Core stuff
    protected GraphicsDeviceManager Graphics => Core.Graphics;
    protected GraphicsDevice GraphicsDevice => Core.GraphicsDevice;
    protected SpriteBatch SpriteBatch => Core.SpriteBatch;

    public Scene()
    {
        gameObjects = new List<GameObject>();
        uninitGameObjects = new List<GameObject>();
        // Create a content manager for the scene
        Content = new ContentManager(Core.Content.ServiceProvider);

        // Set the root directory for content to the same as the root directory
        // for the game's content.
        Content.RootDirectory = Core.Content.RootDirectory;
    }

    ~Scene() => Dispose(false);

    // Callback order
    // Constructor => Initialize => LoadContent
    public virtual void Initialize()
    {
        LoadContent();
    }

    public abstract void LoadContent();

    public virtual void UnloadContent()
    {
        Content.Unload();
    }

    public virtual void Update(GameTime gameTime)
    {
        foreach (var gameObject in uninitGameObjects)
        {
            gameObject.Initialize();
            gameObject.LoadContent(content);
            gameObjects.Add(gameObject);
        }
        uninitGameObjects.Clear();
        
        foreach (var gameObject in GameObjects)
        {
            gameObject.UpdateComponents(gameTime);
        }
    }

    public virtual void Draw(GameTime gameTime)
    {
        if (MainCamera != null)
            MainCamera.Begin(SpriteBatch);
        else
            SpriteBatch.Begin(
                sortMode: SpriteSortMode.FrontToBack,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null
                );
        GraphicsDevice.Clear(BackgroundColor);
        foreach (var gameObject in GameObjects)
        {
            gameObject.DrawComponents(SpriteBatch, gameTime);
        }
        
        
        if (MainCamera != null)
            MainCamera.End(SpriteBatch);
        else
            SpriteBatch.End();

    }

    // This function is deffered (Wait until the next update)
    public GameObject Instantiate(string gameObjectName = "")
    {
        GameObject gameObject = new GameObject(this, gameObjectName);
        AddGameObject(gameObject);
        return gameObject;
    }
    
    // This function is deffered (Wait until the next update)
    public void AddGameObject(GameObject gameObject)
    {
        uninitGameObjects.Add(gameObject);
    }

    // This function is deffered (Wait until the next update)
    public void RemoveGameObject(GameObject gameObject)
    {
        uninitGameObjects.Remove(gameObject);
    }
    
    /// <summary>
    /// Gets a value that indicates if the scene has been disposed of.
    /// </summary>
    public bool IsDisposed { get; private set; }
    
    /// <summary>
    /// Disposes of this scene.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Disposes of this scene.
    /// </summary>
    /// <param name="disposing">'
    /// Indicates whether managed resources should be disposed.  This value is only true when called from the main
    /// Dispose method.  When called from the finalizer, this will be false.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }
        

        if (disposing)
        {
            UnloadContent();
            Content.Dispose();
        }
    }
}