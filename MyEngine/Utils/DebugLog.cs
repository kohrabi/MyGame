#nullable enable
using System;
using Microsoft.Xna.Framework;
using MyEngine.Debug.IMGUIComponents;
using MyEngine.Managers;

namespace MyEngine.Utils;

public class DebugLog : GlobalManager
{
    public static DebugLog? Instance { get; private set; }

    private ImGuiConsole? _console;

    public ImGuiConsole? ImGuiConsole
    {
        get => _console;
        set => _console = value;
    }

    public DebugLog()
    {
        System.Diagnostics.Debug.Assert(Instance == null);
        Instance = this;
    }
    
    public static void Info(string value)
    {
        if (Instance == null || Instance.ImGuiConsole == null)
        {
            Console.WriteLine(value);
            return;
        }
        else
        {
            Instance.ImGuiConsole.AddLog("[info] " + value);
        }
    }
    
    public static void Warning(string value)
    {
        if (Instance == null || Instance.ImGuiConsole == null)
        {
            Console.WriteLine(value);
            return;
        }
        else
        {
            Instance.ImGuiConsole.AddLog("[warning] " + value);
        }
    }
    
    public static void Error(string value)
    {
        if (Instance == null || Instance.ImGuiConsole == null)
        {
            Console.WriteLine(value);
            return;
        }
        else
        {
            Instance.ImGuiConsole.AddLog("[error] " + value);
        }
    }
    
    public static void Log(string value)
    {
        if (Instance == null || Instance.ImGuiConsole == null)
        {
            Console.WriteLine(value);
            return;
        }
        else
        {
            Instance.ImGuiConsole.AddLog(value);
        }
    }
    
    public static void Comment(string value)
    {
        if (Instance == null || Instance.ImGuiConsole == null)
        {
            Console.WriteLine(value);
            return;
        }
        else
        {
            Instance.ImGuiConsole.AddLog("# " + value);
        }
    }

    public override void OnEnable() { }

    public override void OnDisable() { }

    public override void Update(GameTime gameTime) { }
}