using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;
using MyEngine.Graphics;
using Num =  System.Numerics;

namespace MyEngine.Debug.IMGUIComponents;

public class ImGuiViewportRender : ImGuiComponent
{
    readonly Vector2 DEFAULT_RENDER_TARGET_SIZE = new Vector2(1280, 720);
    readonly Vector2 PADDING = new Vector2(-20, -30);
    private MyRenderTarget _myRenderTarget;
    private nint _renderTargetPointer;
    private Texture2D _renderTargetTexture;
    Color[] _renderTargetData;
    private bool _resizeWindow = false;
    private Vector2 _windowSize;
    private Camera _camera;
    
    public bool IsWindowFocused { get; private set; }

    public ImGuiViewportRender(ImGuiRenderer imGuiRenderer, Scene scene, int id, Camera camera)
        : base(imGuiRenderer, scene, id)
    {
        _camera = camera;
        CreateRenderTarget(DEFAULT_RENDER_TARGET_SIZE);
    }

    public override void Update(GameTime gameTime)
    {
        if (_resizeWindow)
        {
            _resizeWindow = false;
            CreateRenderTarget(_windowSize);
        }
    }
    
    public override void Draw()
    {
        ImGui.SetNextWindowSizeConstraints(new Num.Vector2(100f), new Num.Vector2(2560f, 1440f));
        if (ImGui.Begin("GameWindow"))
        {
            IsWindowFocused = false;
            if (ImGui.IsWindowFocused())
                IsWindowFocused = true;
            
            // Get Render data
            _myRenderTarget.RenderTarget.GetData(_renderTargetData);
            _renderTargetTexture.SetData(_renderTargetData);
            
            // Set center
            Num.Vector2 size = new Num.Vector2(_myRenderTarget.RenderTarget.Width, _myRenderTarget.RenderTarget.Height);
            Num.Vector2 avail = ImGui.GetContentRegionAvail();
            ImGui.SetCursorPos(ImGui.GetCursorPos() + (avail - size) / 2.0f);
            _myRenderTarget.Position = ImGui.GetWindowPos() + ImGui.GetCursorPos() - (avail - size) / 2.0f;
            
            // Render
            ImGui.Image(_renderTargetPointer, size);
            
            // If it is resized, queue and recreate 
            _windowSize = avail;
            if (!_windowSize.Equals(new Num.Vector2(_myRenderTarget.RenderTarget.Width, _myRenderTarget.RenderTarget.Height)))
                _resizeWindow = true;
            
        }
        ImGui.End();
    }
    
    
    private void CreateRenderTarget(Vector2 size)
    {
        if (size.X <= 20 || size.Y <= 20)
            return;
        
        if (_myRenderTarget.RenderTarget != null)
            _myRenderTarget.RenderTarget.Dispose();
        _myRenderTarget.RenderTarget = new RenderTarget2D(Core.GraphicsDevice, (int)size.X, (int)size.Y);
        
        _renderTargetData = new Color[_myRenderTarget.RenderTarget.Width * _myRenderTarget.RenderTarget.Height];
        if (_renderTargetTexture != null)
            _renderTargetTexture.Dispose();
        _renderTargetTexture = new Texture2D(Core.GraphicsDevice, _myRenderTarget.RenderTarget.Width, _myRenderTarget.RenderTarget.Height);
        
        if (_renderTargetPointer != 0)
            ImGuiRenderer.UnbindTexture(_renderTargetPointer);
        _renderTargetPointer = ImGuiRenderer.BindTexture(_renderTargetTexture);
        _camera.MyRenderTarget = _myRenderTarget;
    }
}