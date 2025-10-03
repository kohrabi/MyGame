#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyEngine.Managers;

public class InputManager : GlobalManager
{
    public static InputManager Instance { get; private set; }
    
    private Dictionary<string, List<InputAction> > _actionsDictionary = new();
    
    KeyboardState _currentKeyboardState = new KeyboardState();
    MouseState _currentMouseState = new MouseState();
    GamePadState _currentGamePadState = new GamePadState();
    
    KeyboardState _prevKeyboardState;
    MouseState _prevMouseState;
    GamePadState _prevGamePadState;

    public InputManager()
    {
        Debug.Assert(Instance == null, "Input manager has not been initialized.");
        Instance = this;
    }
    
    public override void OnEnable() { }

    public override void OnDisable() { Enabled = true; }

    public override void Update(GameTime gameTime)
    {
        _prevKeyboardState =  _currentKeyboardState;
        _prevMouseState = _currentMouseState;
        _prevGamePadState = _currentGamePadState;
        
        _currentKeyboardState = Keyboard.GetState();
        _currentMouseState = Mouse.GetState();
        _currentGamePadState = GamePad.GetState(PlayerIndex.One);
    }

    public void AddAction(string name, InputAction action)
    {
        if (!_actionsDictionary.ContainsKey(name))
            _actionsDictionary.Add(name, new List<InputAction>());
        _actionsDictionary[name].Add(action);
    }

    public bool IsActionPerformed(string name)
    {
        if (_actionsDictionary.TryGetValue(name, out List<InputAction> actions))
        {
            foreach (var action in actions)
                if (action.Check())
                    return true;
        }

        return false;
    }

    #region Keyboard

    public static bool IsKeyDown(Keys key) => Instance._currentKeyboardState.IsKeyDown(key);
    public static bool IsKeyUp(Keys key) => Instance._currentKeyboardState.IsKeyUp(key);
    public static bool IsKeyJustPressed(Keys key) => IsKeyDown(key) && Instance._prevKeyboardState.IsKeyUp(key);
    public static bool IsKeyJustReleased(Keys key) => IsKeyUp(key) && Instance._prevKeyboardState.IsKeyDown(key);

    /// <summary>
    /// Check if combination is pressed
    /// </summary>
    /// <param name="keys">All pressed keys, Last key will be a just pressed while other is </param>
    /// <returns></returns>
    public static bool IsCombinationPressed(params Keys[]? keys)
    {
        if (keys == null || keys.Length == 0)
            return true;
        if (keys.Length == 1)
            return IsKeyJustPressed(keys[0]);
        for (int i = 0; i < keys.Length - 1; i++)
            if (IsKeyUp(keys[i]))
                return false;
        if (IsKeyJustPressed(keys[^1]))
            return true;
        return false;
    }
        
    #endregion
    
    #region Mouse
    
    public static int GetMouseScrollDelta() => Instance._currentMouseState.ScrollWheelValue - Instance._prevMouseState.ScrollWheelValue;

    public static Point GetMousePosition() => Instance._currentMouseState.Position;
    
    public static bool IsMouseDown(int index)
    {
        if (index == 0)
            return Instance._currentMouseState.LeftButton == ButtonState.Pressed;
        if (index == 1)
            return Instance._currentMouseState.RightButton == ButtonState.Pressed;
        return false;
    }
    
    public static bool IsMouseUp(int index)
    {
        if (index == 0)
            return Instance._currentMouseState.LeftButton == ButtonState.Released;
        if (index == 1)
            return Instance._currentMouseState.RightButton == ButtonState.Released;
        return false;
    }
    
    public static bool IsMouseJustPressed(int index)
    {
        if (index == 0)
            return IsMouseDown(index) && Instance._prevMouseState.LeftButton == ButtonState.Released;
        if (index == 1)
            return IsMouseDown(index) && Instance._prevMouseState.RightButton == ButtonState.Released;
        return false;
    }
    
    public static bool IsMouseJustReleased(int index)
    {
        if (index == 0)
            return IsMouseUp(index) && Instance._prevMouseState.LeftButton == ButtonState.Pressed;
        if (index == 1)
            return IsMouseUp(index) && Instance._prevMouseState.RightButton == ButtonState.Pressed;
        return false;
    }
    
    #endregion
}
