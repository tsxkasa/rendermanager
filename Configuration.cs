using Dalamud.Configuration;
using System;

namespace RenderManager;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsRenderingEnabled { get; set; } = true;

    public int FpsCap {get; set; } = 60;
    public bool IsFpsCapped { get; set; } = false;
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
