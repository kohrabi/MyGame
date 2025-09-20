using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;
using MyEngine.GameObjects;
using Num =  System.Numerics;

namespace MyEngine.Debug.IMGUIComponents;

public class ImGuiGameObjectsWindow : ImGuiComponent
{
    private Scene _scene;
    
    private GameObject _selectedGameObject;

    public ImGuiGameObjectsWindow(ImGuiRenderer imGuiRenderer, Scene scene)
        : base(imGuiRenderer)
    {
        _scene = scene;
    }

    public override void Update(GameTime gameTime)
    {
        
    }

    public override void Draw()
    {
        if (ImGui.Begin("GameObjects"))
        {
            foreach (var gameObject in _scene.GameObjects)
            {
                if (gameObject.Transform.Parent == null)
                    DrawTreeNode(gameObject);
            }
        }
        ImGui.End();

        if (ImGui.Begin("Components"))
        {
            if (_selectedGameObject != null)
            {
                foreach (var component in _selectedGameObject.Components)
                {
                    DrawTreeNode(component);
                }
            }
        }
        ImGui.End();
    }

    private void DrawTreeNode(GameObject gameObject)
    {
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None;
        if (gameObject.Transform.Children.Count == 0)
            flags |= ImGuiTreeNodeFlags.Leaf;
        if (gameObject == _selectedGameObject)
            flags |= ImGuiTreeNodeFlags.Selected;
        if (ImGui.TreeNodeEx(gameObject.Name + "##" + gameObject.GetHashCode(), flags))
        {
            if (ImGui.IsItemClicked())
                _selectedGameObject = gameObject;
            foreach (Transform child in gameObject.Transform.Children)
            {
                DrawTreeNode(child.GameObject);
            }
            ImGui.TreePop();
        }
    }
    
    private void DrawTreeNode(Component component)
    {
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None;
        if (ImGui.TreeNodeEx(component.GetType().Name + "##" + component.GetHashCode(), flags))
        {
            foreach (var property in component.GetType().GetProperties())
            {
                ImGui.Text(property.Name );
            }
            ImGui.TreePop();
        }
    }
}