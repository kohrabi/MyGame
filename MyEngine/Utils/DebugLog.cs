#nullable enable
using System;
using Microsoft.Xna.Framework;
using MyEngine.Managers;

namespace MyEngine.Utils;

public enum DebugLogType
{
    None,
    Info,
    Warning,
    Error,
    Comment
}

public class DebugLog : GlobalManager
{
    public static DebugLog? Instance { get; private set; }

    public Action<DebugLogType, string>? LogAction { get; set; } = null;

    public DebugLog()
    {
        System.Diagnostics.Debug.Assert(Instance == null);
        Instance = this;
    }

    public static void Log(DebugLogType type, string message)
    {
        switch (type)
        {
            case DebugLogType.None:
            {
                Console.WriteLine(message);
                break;
            }
            case DebugLogType.Info:
            {
                message = "[info] " + message;
                Console.WriteLine(message);
                break;
            }
            case DebugLogType.Warning:
            {
                message = "[warning] " + message;
                Console.WriteLine(message);
                break;
            }
            case DebugLogType.Error:
            {
                message = "[error] " + message;
                Console.WriteLine(message);
                break;
            }
            case DebugLogType.Comment:
            {
                message = "# " + message;
                Console.WriteLine(message);
                break;
            }
        }
        Instance?.LogAction?.Invoke(type, message);
    }
    
    public static void Info(string value)
    {
        Log(DebugLogType.Info, value);
    }
    
    public static void Warning(string value)
    {
        Log(DebugLogType.Warning, value);
    }
    
    public static void Error(string value)
    {
        Log(DebugLogType.Error, value);
    }
    
    public static void Log(string value)
    {
        Log(DebugLogType.None, value);   
    }
    
    public static void Comment(string value)
    {
        Log(DebugLogType.Comment, value);
    }

    public override void OnEnable() { }

    public override void OnDisable() { }

    public override void Update(GameTime gameTime) { }
}