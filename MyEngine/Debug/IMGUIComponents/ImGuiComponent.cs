using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;

namespace MyEngine.Debug.IMGUIComponents;

public abstract class ImGuiComponent
{
    protected readonly int _id = 0;
    protected Scene _scene;
    protected ImGuiRenderer _imGuiRenderer;
    public int ID => _id;
    protected Scene Scene => _scene;
    protected ImGuiRenderer ImGuiRenderer => _imGuiRenderer;
    public ImGuiComponent(ImGuiRenderer renderer, Scene scene, int id)
    {
        _imGuiRenderer = renderer;
        _scene = scene;
        _id = id;
    }
    public abstract void Update(GameTime gameTime);
    public abstract void Draw();
}