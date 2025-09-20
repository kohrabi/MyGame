using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;

namespace MyEngine.IMGUIComponents;

public abstract class ImGuiComponent
{

    private ImGuiRenderer _imGuiRenderer;
    protected ImGuiRenderer ImGuiRenderer => _imGuiRenderer;
    public ImGuiComponent(ImGuiRenderer renderer)
    {
        _imGuiRenderer = renderer;
    }
    public abstract void Update(GameTime gameTime);
    public abstract void Draw();
}