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
        value = "[info] " + value;
        Console.WriteLine(value);
        if (Instance != null && Instance.ImGuiConsole != null)
            Instance.ImGuiConsole.AddLog(value);
    }
    
    public static void Warning(string value)
    {
        value = "[warning] " + value;
        Console.WriteLine(value);
        if (Instance != null && Instance.ImGuiConsole != null)
            Instance.ImGuiConsole.AddLog(value);
    }
    
    public static void Error(string value)
    {
        value = "[error] " + value;
        Console.WriteLine(value);
        if (Instance != null && Instance.ImGuiConsole != null)
            Instance.ImGuiConsole.AddLog(value);
    }
    
    public static void Log(string value)
    {
        Console.WriteLine(value);
        if (Instance != null && Instance.ImGuiConsole != null)
            Instance.ImGuiConsole.AddLog(value);
    }
    
    public static void Comment(string value)
    {
        value = "# " + value;
        Console.WriteLine(value);
        if (Instance != null && Instance.ImGuiConsole != null)
            Instance.ImGuiConsole.AddLog(value);
    }

    public override void OnEnable() { }

    public override void OnDisable() { }

    public override void Update(GameTime gameTime) { }
}