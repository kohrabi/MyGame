#nullable enable
using System;
using MyEngine.Components;
using MyEngine.Managers;

namespace MyEngine.GameObjects;

public class PrefabBuilder
{
    private GameObject _gameObject;
    
    private PrefabBuilder(GameObject gameObject)
    {
        _gameObject = gameObject;
    }

    public PrefabBuilder AddComponent<T>(Action<T>? componentSetup = null)  where T : Component
    {
        var component = _gameObject.AddComponent<T>();
        componentSetup?.Invoke(component);
        return this;
    }
    
    public PrefabBuilder GetGameObject(Action<GameObject>? gameObjectSetup = null) 
    {
        gameObjectSetup?.Invoke(_gameObject);
        return this;
    }
    
    public static PrefabBuilder Instatiate()
    {
        return new PrefabBuilder(SceneManager.Instance.CurrentScene.Instantiate());
    }
}