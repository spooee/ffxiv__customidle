using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace customidle
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public string Emote { get; set; } = string.Empty;
        public bool Unsheathed { get; set; } = true;

        [NonSerialized]
        private IDalamudPluginInterface? PluginInterface;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }
    }
}
