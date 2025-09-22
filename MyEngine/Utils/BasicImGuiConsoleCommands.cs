using System;
using MyEngine.Debug.IMGUIComponents;
using MyEngine.Utils.Attributes;

namespace MyEngine.Utils;

public static class BasicImGuiConsoleCommands
{
    
    [ImGuiConsoleCommand("HELLO", typeof(BasicImGuiConsoleCommands), nameof(Hello))]
    public static string Hello()
    {
        return "Hello";
    }
    
    [ImGuiConsoleCommand("HELP_COMMAND", typeof(BasicImGuiConsoleCommands), nameof(HelpCommand), "Print fields, description of a command")]
    public static string HelpCommand(string command)
    {
        ImGuiConsoleCommand? consoleCommand = 
            ImGuiConsoleCommand.ConsoleCommands.Find((value) => value.Command.Equals(command));
        if (consoleCommand != null)
            return ImGuiConsole.GetCommandInfo(consoleCommand);
        return "[warning] Command not found";
    }
    
    [ImGuiConsoleCommand("HELP", typeof(BasicImGuiConsoleCommands), nameof(Help), "Print all commands according to pages")]
    public static string Help(int page = 0)
    {
        const int MAX_RESULT = 8;
        if (page * MAX_RESULT > ImGuiConsoleCommand.ConsoleCommands.Count)
            return "[error] Page doesn't exists";
        string result = "[info] Commands Page " + (page + 1) + "/" + (ImGuiConsoleCommand.ConsoleCommands.Count / MAX_RESULT + 1) + '\n';
        for (int i = 0; i < Math.Min(MAX_RESULT, ImGuiConsoleCommand.ConsoleCommands.Count); i++)
        {
            int index = page * MAX_RESULT + i;
            result += ImGuiConsole.GetCommandInfo(ImGuiConsoleCommand.ConsoleCommands[index]) + "\n";
        }

        return result;
    }
}