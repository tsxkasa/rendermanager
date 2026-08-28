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
using RenderManager.System;

namespace RenderManager.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;

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
            plugin.FrameLimiter.TargetFps = tarFps;
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

        var webhookEnabled = DiscordWebhook.IsWebhookEnabled;
        if (ImGui.Checkbox("Discord Webhook Enabled", ref webhookEnabled)) {
            DiscordWebhook.IsWebhookEnabled = webhookEnabled;
        }

        var hookUrl = DiscordWebhook.WebhookUrl;
        if (ImGui.InputText("Webhook Url", ref hookUrl, 512)) {
            DiscordWebhook.WebhookUrl = hookUrl;
        }

        var webOnJoin = DiscordWebhook.IsHookOnPartyJoin;
        if (ImGui.Checkbox("Notify on join", ref webOnJoin)) {
            DiscordWebhook.IsHookOnPartyJoin = webOnJoin;
        }

        var webOnLeave = DiscordWebhook.IsHookOnPartyLeave;
        if (ImGui.Checkbox("Notify on join", ref webOnLeave)) {
            DiscordWebhook.IsHookOnPartyJoin = webOnLeave;
        }
    }
}
