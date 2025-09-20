using MyEngine.Components;

namespace MyEngine.GameObjects;

public class PrefabBuilder
{
    private GameObject _gameObject;

    public delegate void ComponentSetup<in T>(T component) where T : Component;
    
    private PrefabBuilder(GameObject gameObject)
    {
        _gameObject = gameObject;
    }

    public PrefabBuilder AddComponent<T>(ComponentSetup<T> componentSetup)  where T : Component
    {
        var component = _gameObject.AddComponent<T>();
        componentSetup.Invoke(component);
        return this;
    }
    
    public static PrefabBuilder Instatiate()
    {
        return new PrefabBuilder(Core.SceneManager.CurrentScene.Instantiate());
    }
}