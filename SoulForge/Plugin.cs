using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Bloodstone.API;
using Bloodstone.Hooks;
using Bloody.Core.GameData.v1;
using Unity.Entities;
using Stunlock.Core;
using ProjectM;

namespace SoulForge
{
    public static class MyPluginInfo
    {
        public const string PLUGIN_GUID = "SoulForge";
        public const string PLUGIN_NAME = "SoulForge";
        public const string PLUGIN_VERSION = "1.0.0";
    }

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.Bloodstone", BepInDependency.DependencyFlags.HardDependency)]
    public class SoulForgePlugin : BasePlugin
    {
        private Harmony _harmony;
        public static SoulForgePlugin Instance { get; private set; }

        public static ConfigEntry<bool> AllowSolarusContainer { get; private set; }
        public static ConfigEntry<bool> AllowMonsterContainer { get; private set; }
        public static ConfigEntry<bool> AllowManticoreContainer { get; private set; }
        public static ConfigEntry<bool> AllowDraculaContainer { get; private set; }

        public static ConfigEntry<bool> EnableGlowDracula { get; private set; }
        public static ConfigEntry<bool> EnableGlowAdam { get; private set; }
        public static ConfigEntry<bool> EnableGlowHorror { get; private set; }
        public static ConfigEntry<bool> EnableGlowSolarus { get; private set; }

        public override void Load()
        {
            Instance = this;

            AllowSolarusContainer   = Config.Bind("Containers", "Allow_Solarus", true, "Allow the Solarus container?");
            AllowMonsterContainer   = Config.Bind("Containers", "Allow_Monster", true, "Allow the Adam(Monster) container?");
            AllowManticoreContainer = Config.Bind("Containers", "Allow_Manticore", true, "Allow the Winged Horror(Manticore) container?");
            AllowDraculaContainer   = Config.Bind("Containers", "Allow_Dracula", true, "Allow the Dracula container?");

            EnableGlowDracula = Config.Bind("Glows", "Enable_Dracula_Glow", true, "Enable glow for Dracula Shard?");
            EnableGlowAdam    = Config.Bind("Glows", "Enable_Adam_Glow", true, "Enable glow for Adam Shard?");
            EnableGlowHorror  = Config.Bind("Glows", "Enable_Horror_Glow", true, "Enable glow for Winged Horror Shard?");
            EnableGlowSolarus = Config.Bind("Glows", "Enable_Solarus_Glow", true, "Enable glow for Solarus Shard?");

            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            Chat.OnChatMessage += HandleReloadCommand;
        }

        public override bool Unload()
        {
            _harmony?.UnpatchSelf();
            Chat.OnChatMessage -= HandleReloadCommand;
            return true;
        }

        private void HandleReloadCommand(VChatEvent ev)
        {
            if (ev.Message.Equals("!reload", System.StringComparison.OrdinalIgnoreCase) && ev.User.IsAdmin)
            {
                try
                {
                    Config.Reload();
                    Config.Save();
                    RecheckAllPlayersGlows();
                    ev.User.SendSystemMessage("<color=#00FF00>SoulForge config reloaded successfully!</color>");
                }
                catch (System.Exception ex)
                {
                    ev.User.SendSystemMessage($"<color=#FF0000>SoulForge reload failed:</color> {ex.Message}");
                    Log.LogError($"Error reloading SoulForge config: {ex}");
                }
            }
        }

        private void RecheckAllPlayersGlows()
        {
            var em = VWorld.Server.EntityManager;

            foreach (var user in GameData.Users.All)
            {
                if (!user.IsConnected) continue;
                var charEnt = user.Character.Entity;
                if (charEnt == Entity.Null || !em.Exists(charEnt)) continue;

                // Only remove buffs for shards that are toggled OFF
                foreach (var kvp in SoulForgeData.ShardNecklacesToVisualBuffs)
                {
                    var shardItem = kvp.Key;
                    var glowBuff = kvp.Value;

                    if (!IsGlowEnabled(shardItem) && BuffUtility.TryGetBuff(em, charEnt, glowBuff, out _))
                    {
                        SoulForgeHelpers.Unbuff(charEnt, glowBuff);
                    }
                }
            }
        }

        public bool IsGlowEnabled(PrefabGUID shardItem)
        {
            if (shardItem.Equals(SoulForgeData.DraculaShardItem)) return EnableGlowDracula.Value;
            if (shardItem.Equals(SoulForgeData.AdamShardItem)) return EnableGlowAdam.Value;
            if (shardItem.Equals(SoulForgeData.HorrorShardItem)) return EnableGlowHorror.Value;
            if (shardItem.Equals(SoulForgeData.SolarusShardItem)) return EnableGlowSolarus.Value;
            return true;
        }
    }
}

