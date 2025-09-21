using MyEngine.Managers;

namespace MyEngine.GameObjects;

// This is the class for creating Prefab
// Inherit this class and add component within Initialize
// You can also use PrefabBuilder
public abstract class Prefab
{
    private GameObject _gameObject;

    public Prefab(string name = "")
    {
        _gameObject = SceneManager.Instance.CurrentScene.Instantiate(name);
    }
    
    public abstract void Initialize();
}