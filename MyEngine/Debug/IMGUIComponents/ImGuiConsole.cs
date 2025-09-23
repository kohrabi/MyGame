#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImGuiNET;
using ImGuiNET.Renderer;
using Microsoft.Xna.Framework;
using MyEngine.Utils.Attributes;
using Num = System.Numerics;

namespace MyEngine.Debug.IMGUIComponents;

public class ImGuiConsole : ImGuiComponent
{
    private const int MAX_BUFFER_SIZE = 256;
    private const int MAX_HISTORY_SIZE = 16;
    
    List<string> _logs = new List<string>();
    List<string> _history = new List<string>();
    private Dictionary<string, ImGuiConsoleCommand> _commands => ImGuiConsoleCommand.ConsoleCommands;
    bool _autoScroll = true;
    private string _filter = "";
    
    private string _buf = "";
    private bool _scrollToBottom = false;
    
    
    public ImGuiConsole(ImGuiRenderer renderer, Scene scene) : base(renderer, scene)
    {
        
    }


    public override void Update(GameTime gameTime) { }

    private void ClearLog()
    {
        _logs.Clear();
    }
    
    public void AddLog(string log)
    {
        _logs.Add(log);
    }
    
    public override void Draw()
    {
        ImGui.SetNextWindowSizeConstraints(new Num.Vector2(50f), new Num.Vector2(1280, 1080));
        if (ImGui.Begin("Console"))
        {
            if (ImGui.SmallButton("Clear"))
            {
                ClearLog();
            }
            
            ImGui.InputText("Filter", ref _filter, MAX_BUFFER_SIZE);
            
            float footer_height_to_reserve = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
            if (ImGui.BeginChild("ScrollingRegion", new Num.Vector2(0, -footer_height_to_reserve), ImGuiChildFlags.NavFlattened, ImGuiWindowFlags.HorizontalScrollbar))
            {
                if (ImGui.BeginPopupContextWindow())
                {
                    if (ImGui.Selectable("Clear")) ClearLog();
                    ImGui.EndPopup();
                }
                
                
                
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Num.Vector2(4, 1)); // Tighten spacing
                foreach (string log in _logs)
                {
                    if (_filter.Length > 0 && !log.Contains(_filter))
                        continue;
                    
                    // Normally you would store more information in your item than just a string.
                    // (e.g. make Items[] an array of structure, store color/type etc.)
                    Color color = Color.White;
                    bool hasColor = false;
                    if (log.StartsWith("[error]")) { color = Color.Crimson; hasColor = true; }
                    else if (log.StartsWith("# ")) { color = Color.LightSkyBlue; hasColor = true; }
                    else if (log.StartsWith("[info]")) { color = Color.Green; hasColor = true; }
                    else if (log.StartsWith("[warning]")) { color = Color.Yellow; hasColor = true; }
                    
                    
                    if (hasColor)
                        ImGui.PushStyleColor(ImGuiCol.Text, color.ToVector4().ToNumerics());
                    ImGui.TextUnformatted(log);
                    if (hasColor)
                        ImGui.PopStyleColor();
                }
                // Keep up at the bottom of the scroll region if we were already at the bottom at the beginning of the frame.
                // Using a scrollbar or mouse-wheel will take away from the bottom edge.
                if (_scrollToBottom || (_autoScroll && ImGui.GetScrollY() >= ImGui.GetScrollMaxY()))
                    ImGui.SetScrollHereY(1.0f);
                _scrollToBottom = false;

                ImGui.PopStyleVar();
            }
            ImGui.EndChild();
            ImGui.Separator();
            
            ImGuiInputTextFlags inputTextFlags = ImGuiInputTextFlags.EnterReturnsTrue | 
                                                 ImGuiInputTextFlags.EscapeClearsAll | 
                                                 ImGuiInputTextFlags.CallbackCompletion | 
                                                 ImGuiInputTextFlags.CallbackHistory;
            bool reclaimFocus = false;
            unsafe
            {
                if (ImGui.InputText("Input", ref _buf, MAX_BUFFER_SIZE, inputTextFlags, Callback))
                {
                    ExecCommand(_buf);
                    _buf = "";
                    reclaimFocus = true;
                }
            }

            // Auto-focus on window apparition
            ImGui.SetItemDefaultFocus();
            if (reclaimFocus)
                ImGui.SetKeyboardFocusHere(-1); // Auto focus previous widget
        }
        ImGui.End();
    }

    private unsafe int Callback(ImGuiInputTextCallbackData* data)
    {
        return 0;
    }
    
    private void ExecCommand(string fullCommand)
    {
        fullCommand = fullCommand.Trim().ToUpper();
        if (fullCommand.Length <= 0) return;
        
        // Split command and parameters
        string[] commandParts = fullCommand.Split(' ');
        string command = commandParts[0];
        string[] parameters = commandParts.Skip(1).ToArray();
        
        // History
        if (_history.Count > MAX_HISTORY_SIZE)
            _history.RemoveAt(0);
        _history.Remove(command);
        _history.Add(command);
        _scrollToBottom = true;

        // Get all methods
        if (_commands.TryGetValue(command, out ImGuiConsoleCommand? methodInfo))
        {
            AddLog("> Executing command: " + command);
            
            // Try best fit
            // Doesn't work
            // List<object>? parameterList = null;
            // ImGuiConsoleCommand? targetMethod = null;
            // foreach (var methodInfo in methodInfos)
            // {
            //     List<object>? tryParameter = ConvertParameter(methodInfo, parameters);
            //     if (parameterList == null)
            //         continue;
            //     if (targetMethod == null ||
            //         Math.Abs(parameters.Length - parameterList.Count) >= Math.Abs(parameters.Length - methodInfo.Method.GetParameters().Length))
            //     {
            //         if (Math.Abs(parameters.Length - parameterList.Count) == Math.Abs(parameters.Length - methodInfo.Method.GetParameters().Length))
            //             AddLog("[warning] " + methodInfo.Method.Name + " has existing alternative. Please Delete one");
            //         targetMethod = methodInfo;
            //         parameterList = tryParameter;
            //     }
            // }
            
            List<object>? parameterList = ConvertParameter(methodInfo, parameters);
            if (parameterList == null)
                return;
            
            object? result = methodInfo.Method.Invoke(null, parameterList.ToArray());
            if (result != null && methodInfo.Method.ReturnType != typeof(void))
            {
                AddLog(result.ToString());
            }
        }
        else
        {
            AddLog("[error] Command Not Found: " + command);
        }

    }

    private List<object>? ConvertParameter(ImGuiConsoleCommand methodInfo, string[] parameters)
    {
        List<object> parameterList = new List<object>();
        int i = 0;
        bool failed = false;
        foreach (var parameter in methodInfo.Method.GetParameters())
        {
            if (i >= parameters.Length)
            {
                if (parameter.HasDefaultValue)
                {
                    parameterList.Add(parameter.DefaultValue);
                    continue;
                }
                failed = true;
                break;
            }
            if (parameter.ParameterType == typeof(string))
                parameterList.Add(parameters[0]);
            else if (parameter.ParameterType == typeof(float) && float.TryParse(parameters[0], out float floatValue))
                parameterList.Add(floatValue);
            else if (parameter.ParameterType == typeof(double) && double.TryParse(parameters[0], out double doubleValue))
                parameterList.Add(doubleValue);
            else if (parameter.ParameterType == typeof(bool) &&  bool.TryParse(parameters[0], out bool boolValue))
                parameterList.Add(boolValue);
            else if (parameter.ParameterType == typeof(int) && int.TryParse(parameters[0], out int intValue))
                parameterList.Add(intValue);
            else
                failed = true;
            
            if (failed)
                break;
            i++;
        }
        if (failed)
        {
            LogExecFailed(methodInfo);
            return null;
        }

        // Ignoring parameters
        if (i < parameters.Length)
        {
            string ignored = "# Ignoring: ";
            for (; i < parameters.Length; i++)
                ignored += parameters[i] + ", ";
            _logs.Add(ignored);
        }
        return parameterList;
    }

    private void LogExecFailed(ImGuiConsoleCommand command)
    {
        AddLog("[error] Failed to Execute Command: " + command.Command + '\n' + GetCommandInfo(command));
    }

    public static string GetCommandInfo(ImGuiConsoleCommand command)
    {
        string commandInfo = "[info] Command: "+ command.Command + " ";
        foreach (var parameter in command.Method.GetParameters())
        {
            commandInfo += parameter.Name + "(" + parameter.ParameterType.Name + ") ";
        }
        
        commandInfo += '\n' + "Description: " + command.Description; 

        return commandInfo;
    }
}