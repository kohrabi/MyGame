using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyEngine.Utils.Attributes;

[AttributeUsage(AttributeTargets.Method)]
// Only for static Functions
// You will need to pass in the command name
public class ImGuiConsoleCommand : Attribute
{
    const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    
    private string _command;
    private MethodInfo _method;
    private string _description;
    public string Command => _command;
    public MethodInfo Method => _method;
    public string Description => _description;
    
    public ImGuiConsoleCommand(string command, string description = "")
    {
        _command = command.ToUpper();;
        _description = description;
    }

    public void SetMethod(MethodInfo method) => _method = method;
    
    private static Dictionary<string, ImGuiConsoleCommand> _commands;
    public static Dictionary<string, ImGuiConsoleCommand> ConsoleCommands
    {
        get 
        { 
            if (_commands == null)
                GetAllConsoleCommands();
            return _commands;
        }
        private set
        {
            _commands = value;
        }
    }
    public static void GetAllConsoleCommands()
    {
        Dictionary<string, ImGuiConsoleCommand> commands = new Dictionary<string, ImGuiConsoleCommand>();
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            // method-level attributes
            foreach (MethodInfo method in type.GetMethods(flags))
            {
                ImGuiConsoleCommand command = method.GetCustomAttribute<ImGuiConsoleCommand>();
                if (command != null && !commands.ContainsKey(command.Command))
                {
                    command.SetMethod(method);
                    commands.Add(command.Command, command);   
                }
            }
        }

        ConsoleCommands = commands;
    }
}