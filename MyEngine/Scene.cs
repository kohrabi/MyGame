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
    private Camera _mainCamera;
    
    public Camera MainCamera
    {
        get => _mainCamera;
        set => _mainCamera = value;
    }

    protected ContentManager Content { get; }
   
    public List<GameObject> GameObjects => gameObjects;
    // Core stuff
    public GraphicsDeviceManager Graphics => Core.Graphics;
    public GraphicsDevice GraphicsDevice => Core.GraphicsDevice;
    public SpriteBatch SpriteBatch => Core.SpriteBatch;
    
    

    public Scene()
    {
        
        // Create a content manager for the scene
        Content = new ContentManager(Core.Content.ServiceProvider);

        // Set the root directory for content to the same as the root directory
        // for the game's content.
        Content.RootDirectory = Core.Content.RootDirectory;
    }

    ~Scene() => Dispose(false);

    public virtual void Initialize()
    {
        foreach (var component in GameObjects)
        {
            component.Initialize();
        }
    }

    public virtual void LoadContent()
    {
        foreach (var gameObject in GameObjects)
        {
            gameObject.LoadContent(content);
        }
    }

    public virtual void UnloadContent()
    {
        Content.Unload();
    }

    public virtual void Update(GameTime gameTime)
    {
        foreach (var gameObject in GameObjects)
        {
            gameObject.UpdateComponents(gameTime);
        }
    }

    public virtual void Draw(GameTime gameTime)
    {
        foreach (var gameObject in GameObjects)
        {
            gameObject.DrawComponents(SpriteBatch, gameTime);
        }
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