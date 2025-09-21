using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;

namespace MyEngine.Debug.IMGUIComponents;

public abstract class ImGuiComponent
{
    private Scene _scene;
    private ImGuiRenderer _imGuiRenderer;
    protected Scene Scene => _scene;
    protected ImGuiRenderer ImGuiRenderer => _imGuiRenderer;
    public ImGuiComponent(ImGuiRenderer renderer, Scene scene)
    {
        _imGuiRenderer = renderer;
        _scene = scene;
    }
    public abstract void Update(GameTime gameTime);
    public abstract void Draw();
}