using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyEngine.Editor;


public class Editor : MyEngine.Core
{
        
    EditorScene1 scene1;
        
    public Editor()
        : base("My Game Title", 800, 600, false)
    {
        scene1 = new EditorScene1();
    }

    /// <summary>
    /// Initializes the game, including setting up localization and adding the 
    /// initial screens to the ScreenManager.
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();
        scene1.Initialize();
            

    }

    protected override void LoadContent()
    {
        base.LoadContent();
        scene1.LoadContent();
    }

    protected override void UnloadContent()
    {
        base.UnloadContent();
            
        scene1.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        scene1.Update(gameTime);
            
    }

    protected override void Draw(GameTime gameTime)
    {
        scene1.Draw(gameTime);
            
        base.Draw(gameTime);
    }
    
}