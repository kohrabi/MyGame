using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyEngine.Editor;


public class Editor : MyEngine.Core
{
        
    EditorScene1 scene1;
        
    public Editor()
        : base("My Game Title", 1280, 720, false)
    {
    }

    /// <summary>
    /// Initializes the game, including setting up localization and adding the 
    /// initial screens to the ScreenManager.
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();
        
        scene1 = new EditorScene1();
        SceneManager.LoadScene(scene1);
    }
}