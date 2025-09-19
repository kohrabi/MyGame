using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MyEngine.Managers;

public class SceneManager : GlobalManager
{
    private Scene _currentScene;
    private Scene _nextScene;
    private bool _changeScene = false;
    
    public Scene CurrentScene => _currentScene;
    public Scene NextScene => _nextScene;
    
    public delegate void SceneChangeEvent(Scene scene);
    public event SceneChangeEvent OnSceneChanged;
    
    public override void OnEnable() {}

    public override void OnDisable() {}

    public override void Update(GameTime gameTime)
    {
        if (_changeScene)
        {
            _changeScene = false;
            TransitionScene();    
        }
    }
    
    public void ChangeScene(Scene nextScene) {
        if (nextScene == _currentScene)
            return;
        Debug.Assert(nextScene != null, "Cannot change scene to null.");
        _nextScene = nextScene;
        _changeScene = true;
    }
    
    private void TransitionScene()
    {
        if (_currentScene != null)
            _currentScene.Dispose();
        GC.Collect();

        _currentScene = _nextScene;
        _nextScene = null;
        
        _currentScene.Initialize();
        _currentScene.LoadContent();
    }
}