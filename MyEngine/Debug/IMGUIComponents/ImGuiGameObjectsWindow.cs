using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.Components;
using MyEngine.GameObjects;
using MyEngine.Utils;
using MyEngine.Utils.Attributes;
using Num =  System.Numerics;

namespace MyEngine.Debug.IMGUIComponents;

public class ImGuiGameObjectsWindow : ImGuiComponent
{
    private GameObject _selectedGameObject;
    public GameObject SelectedGameObject => _selectedGameObject;
    
    public delegate void SelectedGameObjectChange(GameObject selectedGameObject);
    
    public event SelectedGameObjectChange OnSelectedGameObjectChanged;
    
    public ImGuiGameObjectsWindow(ImGuiRenderer imGuiRenderer, Scene scene)
        : base(imGuiRenderer, scene)
    {
    }

    public override void Update(GameTime gameTime)
    {
        
    }

    public override void Draw()
    {
        if (ImGui.Begin("GameObjects"))
        {
            foreach (var gameObject in Scene.GameObjects)
            {
                if (gameObject.Transform.Parent == null && !gameObject.IsDebugGameObject)
                    DrawTreeNode(gameObject);
            }
        }
        ImGui.End();

        if (ImGui.Begin("Components"))
        {
            if (_selectedGameObject != null)
            {
                DrawTreeNode(_selectedGameObject.Transform);
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
            {
                _selectedGameObject = gameObject;
                OnSelectedGameObjectChanged?.Invoke(_selectedGameObject);
            }
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
        if (ImGui.CollapsingHeader(component.GetType().Name + "##" + component.GetHashCode(), flags))
        {
            foreach (var member in component.GetType().GetMembers())
            {
                if (member.GetCustomAttribute<SerializeField>() != null)
                {
                    if (member is FieldInfo fieldInfo)   
                        FieldToImGui(fieldInfo, component);
                    else if (member is PropertyInfo propertyInfo)
                        PropertyToImGui(propertyInfo, component);
                }
            }
        }
    }
    
    private void DrawTreeNode(Transform transform)
    {
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None;
        if (ImGui.CollapsingHeader(transform.GetType().Name + "##" + transform.GetHashCode(), flags))
        {
            foreach (var member in transform.GetType().GetMembers())
            {
                if (member.GetCustomAttribute<SerializeField>() != null)
                {
                    if (member is FieldInfo fieldInfo)   
                        FieldToImGui(fieldInfo, transform);
                    else if (member is PropertyInfo propertyInfo)
                        PropertyToImGui(propertyInfo, transform);
                }
            }
        }
    }

    private void PropertyToImGui(PropertyInfo property, object target)
    {
        // ImGui.Text(property.Name);
        string propertyName = property.Name + "##" + property.GetHashCode();
        if (property.PropertyType == typeof(bool))
        {
            bool valueRef = (bool)property.GetValue(target);
            if (ImGui.Checkbox(propertyName, ref valueRef))
            {
                property.SetValue(target, valueRef);
            }
        }
        else if (property.PropertyType == typeof(int))
        {
            int valueRef = (int)property.GetValue(target);
            if (ImGui.DragInt(propertyName, ref valueRef))
            {
                property.SetValue(target, valueRef);
            }
        }
        else if (property.PropertyType == typeof(float))
        {
            float valueRef = (float)property.GetValue(target);
            if (ImGui.DragFloat(propertyName, ref valueRef))
            {
                property.SetValue(target, (float)valueRef);
            }
        }
        else if (property.PropertyType == typeof(double))
        {
            double value = (double)property.GetValue(target);
            float valueRef = (float)value;
            if (ImGui.DragFloat(propertyName, ref valueRef))
            {
                value = valueRef;
                property.SetValue(target, value);
            }
        }
        else if (property.PropertyType == typeof(Vector2))
        {
            Num.Vector2 valueRef = ((Vector2)property.GetValue(target)).ToNumerics();
            if (ImGui.DragFloat2(propertyName, ref valueRef))
            {
                property.SetValue(target, valueRef.ToXna());
            }
        }
        else if (property.PropertyType == typeof(Vector3))
        {
            Num.Vector3 valueRef = ((Vector3)property.GetValue(target)).ToNumerics();
            if (ImGui.DragFloat3(propertyName, ref valueRef))
            {
                property.SetValue(target, valueRef.ToXna());
            }
        }
    }
    
    private void FieldToImGui(FieldInfo field, object target)
    {
        // ImGui.Text(property.Name);
        string fieldName = field.Name + "##" + field.GetHashCode();
        if (field.FieldType == typeof(bool))
        {
            bool valueRef = (bool)field.GetValue(target);
            if (ImGui.Checkbox(fieldName, ref valueRef))
            {
                field.SetValue(target, valueRef);
            }
        }
        else if (field.FieldType == typeof(int))
        {
            int valueRef = (int)field.GetValue(target);
            if (ImGui.DragInt(fieldName, ref valueRef))
            {
                field.SetValue(target, valueRef);
            }
        }
        else if (field.FieldType == typeof(float))
        {
            float valueRef = (float)field.GetValue(target);
            if (ImGui.DragFloat(fieldName, ref valueRef))
            {
                field.SetValue(target, (float)valueRef);
            }
        }
        else if (field.FieldType == typeof(double))
        {
            double value = (double)field.GetValue(target);
            float valueRef = (float)value;
            if (ImGui.DragFloat(fieldName, ref valueRef))
            {
                value = valueRef;
                field.SetValue(target, value);
            }
        }
        else if (field.FieldType == typeof(Vector2))
        {
            Num.Vector2 valueRef = ((Vector2)field.GetValue(target)).ToNumerics();
            if (ImGui.DragFloat2(fieldName, ref valueRef))
            {
                field.SetValue(target, valueRef.ToXna());
            }
        }
        else if (field.FieldType == typeof(Vector3))
        {
            Num.Vector3 valueRef = ((Vector3)field.GetValue(target)).ToNumerics();
            if (ImGui.DragFloat3(fieldName, ref valueRef))
            {
                field.SetValue(target, valueRef.ToXna());
            }
        }
    }
}