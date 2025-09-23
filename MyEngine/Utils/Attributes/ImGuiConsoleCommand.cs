using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyEngine.Utils.Attributes;

[AttributeUsage(AttributeTargets.Method)]
// Only for static Functions
// You will need to pass in the command name, type of the class, and the method name because attribute is just metadata
// Return a string inorder to
public class ImGuiConsoleCommand : Attribute
{
    const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    
    private string _command;
    private MethodInfo _method;
    private string _description;
    public string Command => _command;
    public MethodInfo Method => _method;
    public string Description => _description;
    
    public ImGuiConsoleCommand(string command, Type type, string methodName, string description = "")
    {
        _command = command.ToUpper();
        // If this throw some wacky shit, it usually means you have two of the same function
        _method = type.GetMethod(methodName, flags);
        _description = description;
    }

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
            foreach (var method in type.GetMethods(flags))
            {
                var command = method.GetCustomAttribute<ImGuiConsoleCommand>();
                if (command != null && !commands.ContainsKey(command.Command))
                {
                    commands.Add(command.Command, command);   
                }
            }
        }

        ConsoleCommands = commands;
    }
}