using System;
using System.Linq;
using MyEngine.Debug.IMGUIComponents;
using MyEngine.Utils.Attributes;

namespace MyEngine.Utils;

public static class BasicImGuiConsoleCommands
{
    
    [ImGuiConsoleCommand("HELLO")]
    public static string Hello()
    {
        return "Hello";
    }
    
    [ImGuiConsoleCommand("HELP_COMMAND", "Print fields, description of a command")]
    public static string HelpCommand(string command)
    {
        if (ImGuiConsoleCommand.ConsoleCommands.TryGetValue(command, out ImGuiConsoleCommand consoleCommand))
            return ImGuiConsole.GetCommandInfo(consoleCommand);
        return "[warning] Command not found";
    }
    
    [ImGuiConsoleCommand("HELP", "Print all commands according to page")]
    public static string Help(int page = 0)
    {
        const int MAX_RESULT = 8;
        if (page * MAX_RESULT > ImGuiConsoleCommand.ConsoleCommands.Count)
            return "[error] Page doesn't exists";
        string result = "[info] Commands Page " + (page) + "/" + (ImGuiConsoleCommand.ConsoleCommands.Count / MAX_RESULT) + '\n';
        int i = 0;
        foreach (var command in ImGuiConsoleCommand.ConsoleCommands)
        {
            if (i < page * MAX_RESULT)
            {
                i++;
                continue;
            }

            if (i >= page * MAX_RESULT + MAX_RESULT)
                break;
            result += ImGuiConsole.GetCommandInfo(command.Value) + "\n";
            i++;
        }

        return result;
    }
}