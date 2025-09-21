using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Num =  System.Numerics;

namespace MyEngine.Debug.IMGUIComponents;

public class ImGuiViewportRender : ImGuiComponent
{
    
    readonly Vector2 DEFAULT_RENDER_TARGET_SIZE = new Vector2(1280, 720);
    readonly Vector2 PADDING = new Vector2(-20, -30);
    private RenderTarget2D _renderTarget;
    private nint _renderTargetPointer;
    private Texture2D _renderTargetTexture;
    Color[] _renderTargetData;
    private bool _resizeWindow = false;
    private Vector2 _windowSize;

    public ImGuiViewportRender(ImGuiRenderer imGuiRenderer, Scene scene)
        : base(imGuiRenderer, scene)
    {
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

        if (ImGui.Begin("GameWindow"))
        {

            // Get Render data
            _renderTarget.GetData(_renderTargetData);
            _renderTargetTexture.SetData(_renderTargetData);
            
            // Set center
            Num.Vector2 size = new Num.Vector2(_renderTarget.Width, _renderTarget.Height);
            Num.Vector2 avail = ImGui.GetContentRegionAvail();
            ImGui.SetCursorPos(ImGui.GetCursorPos() + (avail - size) / 2.0f);
            
            // Render
            ImGui.Image(_renderTargetPointer, size);
            
            // If it is resized, queue and recreate 
            _windowSize = avail;
            if (!_windowSize.Equals(new Num.Vector2(_renderTarget.Width, _renderTarget.Height)))
                _resizeWindow = true;
            
        }
        ImGui.End();
    }
    
    
    private void CreateRenderTarget(Vector2 size)
    {
        if (_renderTarget != null)
            _renderTarget.Dispose();
        _renderTarget = new RenderTarget2D(Core.GraphicsDevice, (int)size.X, (int)size.Y);
        
        _renderTargetData = new Color[_renderTarget.Width * _renderTarget.Height];
        if (_renderTargetTexture != null)
            _renderTargetTexture.Dispose();
        _renderTargetTexture = new Texture2D(Core.GraphicsDevice, _renderTarget.Width, _renderTarget.Height);
        
        if (_renderTargetPointer != 0)
            ImGuiRenderer.UnbindTexture(_renderTargetPointer);
        _renderTargetPointer = ImGuiRenderer.BindTexture(_renderTargetTexture);
        Scene.MainCamera.RenderTarget2D = _renderTarget;
    }
}