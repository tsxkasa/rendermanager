using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using RenderManager.Windows;
using ECommons;
using RenderManager.System;

namespace RenderManager;

public sealed class Plugin : IDalamudPlugin
{
    private IDalamudPluginInterface PluginInterface { get; init; }
    private ICommandManager CommandManager { get; init; }
    private const string CommandName = "/renman";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("RenderManager");


    public FrameLimiter FrameLimiter { get; init; }
    private MainWindow MainWindow { get; init; }

    public Plugin(IDalamudPluginInterface pluginInterface, ICommandManager commandManager) 
    {
        pluginInterface.Create<Service>();
        PluginInterface = pluginInterface;
        CommandManager = commandManager;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        FrameLimiter = new FrameLimiter();

        ECommonsMain.Init(PluginInterface, this, Module.All);

        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "..."
        });

        // Tell the UI system that we want our windows to be drawn through the window system
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        // Adds another button doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        // Example Output: 00:57:54.959 | INF | [RenderManager] ===A cool log message from Sample Plugin===
        Service.Log.Information($"==={PluginInterface.Manifest.Name} finished setup===");
    }

    public void Dispose()
    {
        // Unregister all actions to not leak anything during disposal of plugin
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        WindowSystem.RemoveAllWindows();

        MainWindow.Dispose();
        ECommonsMain.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // In response to the slash command, toggle the display status of our main ui
        MainWindow.Toggle();
    }
    public void ToggleMainUi() => MainWindow.Toggle();
}
