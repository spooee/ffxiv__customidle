using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using ECommons.Automation;
using ECommons.SimpleGui;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiScene;
using System;

namespace customidle
{
    public sealed unsafe class Plugin : IDalamudPlugin
    {
        public string Name => "Custom Idle";
        private const string CommandName = "/customidle";
        private IDalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("Custom Idle");
        private ConfigWindow ConfigWindow { get; init; }
        private bool IsMoving() => AgentMap.Instance()->IsPlayerMoving == 1;
        private bool InCombat => Service.Condition[ConditionFlag.InCombat];
        private bool IsJumping => Service.Condition[ConditionFlag.Jumping];
        private bool IsBetweenAreas => Service.Condition[ConditionFlag.BetweenAreas] && Service.Condition[ConditionFlag.BetweenAreas51];
        private string Emote => Configuration.Emote;

        public Plugin(
            IDalamudPluginInterface pluginInterface,
            ICommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.PluginInterface.Create<Service>(this);
            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);
            ECommonsMain.Init(pluginInterface, this, Module.All);
            Service.Framework.Update += onFrameworkUpdate;

            ConfigWindow = new ConfigWindow(this);
            WindowSystem.AddWindow(ConfigWindow);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open the config window"
            });
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            this.PluginInterface.UiBuilder.Draw += DrawUI;
        }

        public unsafe void onFrameworkUpdate(object framework)
        {
            Chat sendEmote = new Chat();

            //There is most definitely a nicer way of handling this.
            if (emoteCooldown && CanPerformEmote())
            {
                if (EmoteList.IsValidEmote(Emote) && !IsWeaponUnsheathed() && this.Configuration.Unsheathed)
                {
                    sendEmote.SendMessage(Emote);
                    emoteCooldown = false;
                }

                else if (EmoteList.IsValidEmote(Emote) && !IsWeaponUnsheathed() && !this.Configuration.Unsheathed)
                {
                    sendEmote.SendMessage(Emote);
                    emoteCooldown = false;
                }

                else if (EmoteList.IsValidEmote(Emote) && IsWeaponUnsheathed() && this.Configuration.Unsheathed)
                {
                    sendEmote.SendMessage(Emote);
                    emoteCooldown = false;
                }

                else if (EmoteList.IsValidEmote(Emote) && IsWeaponUnsheathed() && !this.Configuration.Unsheathed)
                {
                    return;
                }
            }
            else if (!emoteCooldown && IsMoving())
            {
                emoteCooldown = true;
            }
        }
        
        private bool emoteCooldown = true; // emoteCooldown makes it so that it only sends the command once until you move again, otherwise it would spam send the command.

        private bool CanPerformEmote()
        {
            return !IsMoving() && !InCombat && !IsJumping && !IsBetweenAreas;
        }

        private bool IsWeaponUnsheathed()
        {
            return UIState.Instance()->WeaponState.IsUnsheathed;
        }

        public void Dispose()
        {
            Service.Framework.Update -= onFrameworkUpdate;
            ECommonsMain.Dispose();
        }

        private void OnCommand(string command, string args)
        {
            ConfigWindow.IsOpen = true;
        }

        public void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            this.ConfigWindow.IsOpen = true;
        }
    }
}
