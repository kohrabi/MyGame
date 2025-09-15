using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine.Components;
using MyEngine.GameObjects;
using MyEngine.Utils;

namespace MyEngine;

public abstract class Scene
{
    internal static Scene s_instance;
    public static Scene CurrentScene => s_instance;
    private Camera _mainCamera;
    
    public Camera MainCamera
    {
        get => _mainCamera;
        set => _mainCamera = value;
    }



    public GraphicsDeviceManager Graphics => Core.Graphics;
    public GraphicsDevice GraphicsDevice => Core.GraphicsDevice;
    public SpriteBatch SpriteBatch => Core.SpriteBatch;

    public ContentManager Content => Core.Content;
        
    public abstract void Initialize();
    public abstract void LoadContent();
    public abstract void UnloadContent();
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime);
}