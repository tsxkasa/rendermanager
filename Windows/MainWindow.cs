using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ECommons.GameFunctions;
using ECommons.Throttlers;
using Lumina.Excel.Sheets;

namespace RenderManager.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;


    // We give this window a hidden ID using ##.
    // The user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin)
        : base("Manage Render", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var renderEnabled = plugin.Configuration.IsRenderingEnabled;
        if (ImGui.Checkbox("Enable Rendering", ref renderEnabled))
        {
            if (!renderEnabled) {
                RenderDisableManager.PlaceRequest();
            }
            if (renderEnabled) {
                RenderDisableManager.RemoveRequest();
            }


            plugin.Configuration.IsRenderingEnabled = renderEnabled;
            plugin.Configuration.Save();
        }

        var tarFps = plugin.Configuration.FpsCap;
        if (ImGui.SliderInt("FPS Cap", ref tarFps, 1, 60)) {
            plugin.Configuration.FpsCap = tarFps;

            plugin.Configuration.Save();
        }

        var fpsLocked = plugin.Configuration.IsFpsCapped;
        if (ImGui.Checkbox("Cap FPS", ref fpsLocked)) {
            plugin.Configuration.IsFpsCapped = fpsLocked;

            plugin.FrameLimiter.IsEnabled = fpsLocked;
            plugin.FrameLimiter.TargetFps = plugin.Configuration.FpsCap;

            plugin.Configuration.Save();
        }
    }
}
