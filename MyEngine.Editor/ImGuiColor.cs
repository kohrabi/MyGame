using System.Numerics;
using ImGuiNET;

namespace MyEngine.Editor;

public static class ImGuiColor
{
    public static void CherryTheme() {
        var style = ImGui.GetStyle();
        style.Colors[(int)ImGuiCol.Text]                  = new Vector4(0.860f, 0.930f, 0.890f, 0.78f);
        style.Colors[(int)ImGuiCol.TextDisabled]          = new Vector4(0.860f, 0.930f, 0.890f, 0.28f);
        style.Colors[(int)ImGuiCol.WindowBg]              = new Vector4(0.13f, 0.14f, 0.17f, 1.00f);
        // style.Colors[ImGuiCol.ChildWindowBg]         = new Vector4(0.200f, 0.220f, 0.270f, 0.58f);
        style.Colors[(int)ImGuiCol.PopupBg]               = new Vector4(0.200f, 0.220f, 0.270f, 0.9f);
        style.Colors[(int)ImGuiCol.Border]                = new Vector4(0.31f, 0.31f, 1.00f, 0.00f);
        style.Colors[(int)ImGuiCol.BorderShadow]          = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        style.Colors[(int)ImGuiCol.FrameBg]               = new Vector4(0.200f, 0.220f, 0.270f, 1.00f);
        style.Colors[(int)ImGuiCol.FrameBgHovered]        = new Vector4(0.455f, 0.198f, 0.301f, 0.78f);
        style.Colors[(int)ImGuiCol.FrameBgActive]         = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBg]               = new Vector4(0.232f, 0.201f, 0.271f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBgActive]         = new Vector4(0.502f, 0.075f, 0.256f,  1.00f);
        style.Colors[(int)ImGuiCol.TitleBgCollapsed]      = new Vector4(0.200f, 0.220f, 0.270f, 0.75f);
        style.Colors[(int)ImGuiCol.MenuBarBg]             = new Vector4(0.200f, 0.220f, 0.270f, 0.47f);
        style.Colors[(int)ImGuiCol.ScrollbarBg]           = new Vector4(0.200f, 0.220f, 0.270f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab]         = new Vector4(0.09f, 0.15f, 0.16f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered]  = new Vector4(0.455f, 0.198f, 0.301f, 0.78f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive]   = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
        style.Colors[(int)ImGuiCol.CheckMark]             = new Vector4(0.71f, 0.22f, 0.27f, 1.00f);
        style.Colors[(int)ImGuiCol.SliderGrab]            = new Vector4(0.47f, 0.77f, 0.83f, 0.14f);
        style.Colors[(int)ImGuiCol.SliderGrabActive]      = new Vector4(0.71f, 0.22f, 0.27f, 1.00f);
        style.Colors[(int)ImGuiCol.Button]                = new Vector4(0.47f, 0.77f, 0.83f, 0.14f);
        style.Colors[(int)ImGuiCol.ButtonHovered]         = new Vector4(0.455f, 0.198f, 0.301f, 0.86f);
        style.Colors[(int)ImGuiCol.ButtonActive]          = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
        style.Colors[(int)ImGuiCol.Header]                = new Vector4(0.455f, 0.198f, 0.301f, 0.76f);
        style.Colors[(int)ImGuiCol.HeaderHovered]         = new Vector4(0.455f, 0.198f, 0.301f, 0.86f);
        style.Colors[(int)ImGuiCol.HeaderActive]          = new Vector4(0.502f, 0.075f, 0.256f,  1.00f);
        // style.Colors[ImGuiCol.Column]                = new Vector4(0.14f, 0.16f, 0.19f, 1.00f);
        // style.Colors[ImGuiCol.ColumnHovered]         = new Vector4(0.455f, 0.198f, 0.301f, 0.78f);
        // style.Colors[ImGuiCol.ColumnActive]          = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
        style.Colors[(int)ImGuiCol.ResizeGrip]            = new Vector4(0.47f, 0.77f, 0.83f, 0.04f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered]     = new Vector4(0.455f, 0.198f, 0.301f, 0.78f);
        style.Colors[(int)ImGuiCol.ResizeGripActive]      = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotLines]             = new Vector4(0.860f, 0.930f, 0.890f, 0.63f);
        style.Colors[(int)ImGuiCol.PlotLinesHovered]      = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotHistogram]         = new Vector4(0.860f, 0.930f, 0.890f, 0.63f);
        style.Colors[(int)ImGuiCol.PlotHistogramHovered]  = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
        style.Colors[(int)ImGuiCol.TextSelectedBg]        = new Vector4(0.455f, 0.198f, 0.301f, 0.43f);
        // [...].
        style.Colors[(int)ImGuiCol.ModalWindowDimBg]  = new Vector4(0.200f, 0.220f, 0.270f, 0.73f);

        style.WindowPadding            = new Vector2(6, 4);
        style.WindowRounding           = 0.0f;
        style.FramePadding             = new Vector2(5, 2);
        style.FrameRounding            = 3.0f;
        style.ItemSpacing              = new Vector2(7, 1);
        style.ItemInnerSpacing         = new Vector2(1, 1);
        style.TouchExtraPadding        = new Vector2(0, 0);
        style.IndentSpacing            = 6.0f;
        style.ScrollbarSize            = 12.0f;
        style.ScrollbarRounding        = 16.0f;
        style.GrabMinSize              = 20.0f;
        style.GrabRounding             = 2.0f;

        style.WindowTitleAlign.X = 0.50f;

        style.Colors[(int)ImGuiCol.Border] = new Vector4(0.539f, 0.479f, 0.255f, 0.162f);
        style.FrameBorderSize = 0.0f;
        style.WindowBorderSize = 1.0f;
    }
}