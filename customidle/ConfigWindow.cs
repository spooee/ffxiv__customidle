using System;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace customidle
{
    public class ConfigWindow : Window, IDisposable
    {
        private Configuration Configuration;

        public ConfigWindow(Plugin plugin) : base(
            "Custom Idle",
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.Size = new Vector2(280, 180);
            this.SizeCondition = ImGuiCond.Always;
            this.Configuration = plugin.Configuration;
        }

        public override void Draw()
        {
            ImGui.TextWrapped("Please enter the emote name you wish to use, WITH the slash.");
            ImGui.Spacing();
            ImGui.TextWrapped("Example: '/vpose', '/breathcontrol'.");
            ImGui.Spacing();
            var emoteConfig = this.Configuration.Emote;
            if (ImGui.InputText("", ref emoteConfig, 256))
            {
                if (EmoteList.IsValidEmote(emoteConfig))
                {
                    this.Configuration.Emote = emoteConfig;
                    this.Configuration.Save();
                }
                else
                {
                    ImGui.TextColored(new Vector4(1, 0, 0, 1), "Invalid emote. Please enter a valid emote.");
                    this.Configuration.Emote = string.Empty;
                    this.Configuration.Save();
                }
            }

            var unsheathedConfig = this.Configuration.Unsheathed;
            if (ImGui.Checkbox("Also perform emote while unsheated?", ref unsheathedConfig))
            {
                 this.Configuration.Unsheathed = unsheathedConfig;
                 this.Configuration.Save();
            }
        }

        public void Dispose() { }
    }
}
