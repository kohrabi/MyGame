using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyEngine.GameObjects;
using MyEngine.Utils;

namespace MyEngine.Components;

#nullable enable
public class Transform
{
    private GameObject _gameObject;
    private List<Transform> _children = new();
    
    private Vector2 _position;
    private float _rotation;
    private Vector2 _scale;
    private Transform _parent;
    private Matrix _localMatrix;


    public int ChildCount
    {
        get => _children.Count;
    }
    public GameObject GameObject
    {
        get => _gameObject;
        private set => _gameObject = value;
    }
    
    // Local Position of the object
    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            RecalculateMatrix();
        }
    }

    // Local Rotation in radians
    public float Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            RecalculateMatrix();
        }
    }

    // Local Scale of the object
    public Vector2 Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            RecalculateMatrix();
        }
    }

    public Transform? Parent
    {
        get => _parent;
        set
        {
            _parent = value;
        }
    }

    public Vector2 GlobalPosition
    {
        get
        {
            if (WorldMatrix.Decompose(out _, out _, out Vector3 translation))
                return translation.ToVector2();
            return Vector2.Zero;
        }
        set
        {
            if (Parent != null)
            {
                Matrix parentInverse = Matrix.Invert(Parent.WorldMatrix);
                Vector3 localPos3 = Vector3.Transform(new Vector3(value, 0), parentInverse);
                Position = localPos3.ToVector2();
            }
            else
            {
                Position = value;
            }
        }
    }

    
    // TODO: Implement GlobalRotation properly
    // public float GlobalRotation
    // {
    //     get
    //     {
    //         if (Parent != null)
    //         {
    //             return Rotation + Parent.GlobalRotation;
    //         }
    //         else
    //         {
    //             return Rotation;
    //         }
    //     }
    //     set
    //     {
    //         if (Parent != null)
    //         {
    //             Rotation = value - Parent.GlobalRotation;
    //         }
    //         else
    //         {
    //             Rotation = value;
    //         }
    //     }
    // }

    // TODO: Implement GlobalScale properly
    // public Vector2 GlobalScale
    // {
    //     get
    //     {
    //         if (WorldMatrix.Decompose(out Vector3 scale, out _, out _))
    //             return scale.ToVector2();
    //         return Vector2.One;
    //     }
    //     set
    //     {
    //         if (Parent != null)
    //         {
    //             
    //         }
    //         return Scale
    //     }
    // }
    
    public Matrix LocalMatrix
    {
        get => _localMatrix;
        private set => _localMatrix = value;
    }

    public Matrix WorldMatrix
    {
        get => Parent != null ? LocalMatrix * Parent.WorldMatrix : LocalMatrix;
    }

    public Transform(GameObject gameObject)
    {
        Position = Vector2.Zero;
        Rotation = 0f;
        Scale = Vector2.One;
        Parent = null;
        LocalMatrix = Matrix.Identity;
    }

    private void RecalculateMatrix()
    {
        Matrix translationMatrix = Matrix.CreateTranslation(new Vector3(Position, 0));
        Matrix rotationMatrix = Matrix.CreateRotationZ(Rotation);
        Matrix scaleMatrix = Matrix.CreateScale(new Vector3(Scale, 1));
        
        LocalMatrix = scaleMatrix * rotationMatrix * translationMatrix;
    }
    
    public Transform? GetChild(int index)
    {
        if (index < 0 || index >= _children.Count)
        {
            return null;
        }
        return _children[index];
    }
    
    public void AddChild(Transform child)
    {
        if (child == null || _children.Contains(child)) return;
        child.Parent = this;
        _children.Add(child);
    }
    
    public void RemoveChild(Transform child)
    {
        if (child == null || !_children.Contains(child)) return;
        child.Parent = null;
        _children.Remove(child);
    }
}