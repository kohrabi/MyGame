using Microsoft.Xna.Framework.Input;

namespace MyEngine.Managers;

public enum InputType
{
    Keyboard,
    Mouse,
    Gamepad
}

public enum InputActionType
{
    Pressed,
    Released,
    JustPressed,
    JustReleased
}

public class InputAction
{
    public Keys Key;
    public int MouseIndex;
    
    public InputType Type;
    public InputActionType ActionType;

    public bool Check()
    {
        switch (Type)
        {
            case  InputType.Keyboard: return CheckKeyboard();
            case  InputType.Mouse: return CheckMouse();
        }
        return false;
    }
    
    private bool CheckMouse()
    {
        switch (ActionType)
        {
            case InputActionType.Pressed: return InputManager.IsMouseDown(MouseIndex);
            case InputActionType.Released: return InputManager.IsMouseUp(MouseIndex);
            case InputActionType.JustPressed: return InputManager.IsMouseJustPressed(MouseIndex);
            case InputActionType.JustReleased: return InputManager.IsMouseJustReleased(MouseIndex);
        }
        return false;
    }
    
    private bool CheckKeyboard()
    {
        switch (ActionType)
        {
            case InputActionType.Pressed: return InputManager.IsKeyDown(Key);
            case InputActionType.Released: return InputManager.IsKeyUp(Key);
            case InputActionType.JustPressed: return InputManager.IsKeyJustPressed(Key);
            case InputActionType.JustReleased: return InputManager.IsKeyJustReleased(Key);
        }
        return false;
    }
    
    public static InputAction Create(Keys key, InputActionType type)
    {
        InputAction action = new InputAction();
        action.Key = key;
        action.Type = InputType.Keyboard;
        action.ActionType = type;
        return action;
    }
    
    public static InputAction Create(int mouseIndex, InputActionType type)
    {
        InputAction action = new InputAction();
        action.MouseIndex = mouseIndex;
        action.Type = InputType.Mouse;
        action.ActionType = type;
        return action;
    }
};