using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyEngine.Components;
using MyEngine.Utils.MyMath;

namespace MyEngine.Utils;

public class Gizmo : Component
{
    private Texture2D _gizmoTexture;
    
    private Texture2D _gizmoX;
    private RectangleF _gizmoXRect;
    
    private Texture2D _gizmoY;
    private RectangleF _gizmoYRect;
    
    private RectangleF _gizmoAxisRect;
    private Texture2D _gizmoAxis;
    private int overIndex = 0;
    private int holdIndex = 0;
    private Vector2 offset = Vector2.Zero;
    private Vector2 ogPosition = Vector2.Zero;

    public void Reset()
    {
        offset = Vector2.Zero;
        ogPosition = Vector2.Zero;
        overIndex = 0;
        holdIndex = 0;
    }
    
    protected override void LoadContent(ContentManager content)
    {
        base.LoadContent(content);
        GameObject.IsDebugGameObject = true;     
        _gizmoTexture = content.Load<Texture2D>("Engine/gizmo");
        _gizmoX = content.Load<Texture2D>("Engine/gizmoX");
        _gizmoY = content.Load<Texture2D>("Engine/gizmoY");
        _gizmoAxis = content.Load<Texture2D>("Engine/gizmoAxis");
        
        _gizmoXRect = new RectangleF(Transform.Position.X, Transform.Position.Y, _gizmoX.Width, _gizmoX.Height);
        _gizmoYRect = new RectangleF(Transform.Position.X, Transform.Position.Y, _gizmoY.Width, _gizmoY.Height);
        _gizmoAxisRect = new RectangleF(Transform.Position.X, Transform.Position.Y, _gizmoAxis.Width, _gizmoAxis.Height);
        
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (Transform.Parent == null)
            return;
        
        _gizmoAxisRect.Location = Transform.GlobalPosition - _gizmoAxisRect.Size / 2.0f;
        _gizmoXRect.Location = _gizmoAxisRect.Location + Vector2.UnitX * _gizmoAxis.Width;
        _gizmoYRect.Location = _gizmoAxisRect.Location + Vector2.UnitY * _gizmoAxis.Height;

        var mouse = Mouse.GetState();
        Vector2 mousePosition = GameObject.Scene.MainCamera.ScreenToWorld(mouse.Position);
        if (mouse.LeftButton == ButtonState.Released)
        {
            holdIndex = 0;
            overIndex = 0;
        }
        if (holdIndex != 0)
        {
            if (overIndex == 1)
                Transform.Parent.GlobalPosition = mousePosition + offset;
            else if (overIndex == 2)
                Transform.Parent.GlobalPosition = new Vector2(mousePosition.X, ogPosition.Y) + offset;
            else if (overIndex == 3)
                Transform.Parent.GlobalPosition = new Vector2(ogPosition.X, mousePosition.Y) + offset;
        }
        else
        {
            if (_gizmoAxisRect.Contains(mousePosition))
                overIndex = 1;
            else if (_gizmoXRect.Contains(mousePosition))
                overIndex = 2;
            else if (_gizmoYRect.Contains(mousePosition))
                overIndex = 3;
            
            if (overIndex != 0 && mouse.LeftButton == ButtonState.Pressed)
            {
                holdIndex = overIndex;
                offset = (Transform.Parent.GlobalPosition - mousePosition);
                if (overIndex == 2)
                    offset.Y = 0f;
                if (overIndex == 3)
                    offset.X = 0f;
                ogPosition = Transform.Parent.GlobalPosition;
            }
        }
        // DebugDraw.DrawCircle(mousePosition, Color.White, 4.0f);
        // if ()
    }
    
    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.Draw(spriteBatch, gameTime);
        if (Transform.Parent == null || Transform.Parent.GameObject.HasComponent<Camera>())
            return;

        spriteBatch.Draw(
            _gizmoAxis, 
            _gizmoAxisRect.Location, 
            null, 
            overIndex == 1 || holdIndex == 1 ? new Color(Color.Black, 0.5f) : Color.White, 
            0f, 
            Vector2.Zero, 
            Transform.Scale, 
            SpriteEffects.None, 
            1f);

        spriteBatch.Draw(
            _gizmoX, 
            _gizmoXRect.Location, 
            null, 
            overIndex == 2 || holdIndex == 2 ? new Color(Color.Black, 0.5f) : Color.White, 
            0f, 
            Vector2.Zero, 
            Transform.Scale, 
            SpriteEffects.None, 
            1f);

        spriteBatch.Draw(
            _gizmoY, 
            _gizmoYRect.Location, 
            null, 
            overIndex == 3 || holdIndex == 3 ? new Color(Color.Black, 0.5f) : Color.White, 
            0f, 
            Vector2.Zero, 
            Transform.Scale, 
            SpriteEffects.None, 
            1f);
    }
}