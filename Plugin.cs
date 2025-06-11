using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Stunlock.Core;

namespace SoulForge
{
    [BepInPlugin("SoulForge", "SoulForge", "2.0.1")] 
    public class Plugin : BasePlugin
    {
        private Harmony _harmony;
        public static Plugin Instance { get; private set; }
        public bool StartupCheckDone { get; set; } = false; 

        public static ConfigEntry<bool> AllowSolarusContainer { get; private set; }
        public static ConfigEntry<bool> AllowMonsterContainer { get; private set; }
        public static ConfigEntry<bool> AllowManticoreContainer { get; private set; }
        public static ConfigEntry<bool> AllowDraculaContainer { get; private set; }
        public static ConfigEntry<bool> AllowMorganaContainer { get; private set; }

        public static ConfigEntry<bool> EnableGlowDracula { get; private set; }
        public static ConfigEntry<bool> EnableGlowAdam { get; private set; }
        public static ConfigEntry<bool> EnableGlowHorror { get; private set; }
        public static ConfigEntry<bool> EnableGlowSolarus { get; private set; }
        public static ConfigEntry<bool> EnableGlowMorgana { get; private set; }

        public override void Load()
        {
            Instance = this;

            AllowSolarusContainer = Config.Bind("Containers", "Allow_Solarus", true, "Allow the Solarus container?");
            AllowMonsterContainer = Config.Bind("Containers", "Allow_Monster", true, "Allow the Adam(Monster) container?");
            AllowManticoreContainer = Config.Bind("Containers", "Allow_Manticore", true, "Allow the Winged Horror(Manticore) container?");
            AllowDraculaContainer = Config.Bind("Containers", "Allow_Dracula", true, "Allow the Dracula container?");
            AllowMorganaContainer = Config.Bind("Containers", "Allow_Morgana", true, "Allow the Morgana container?");

            EnableGlowDracula = Config.Bind("Glows", "Enable_Dracula_Glow", true, "Enable glow for Dracula Shard?");
            EnableGlowAdam = Config.Bind("Glows", "Enable_Adam_Glow", true, "Enable glow for Adam Shard?");
            EnableGlowHorror = Config.Bind("Glows", "Enable_Horror_Glow", true, "Enable glow for Winged Horror Shard?");
            EnableGlowSolarus = Config.Bind("Glows", "Enable_Solarus_Glow", true, "Enable glow for Solarus Shard?");
            EnableGlowMorgana = Config.Bind("Glows", "Enable_Morgana_Glow", true, "Enable glow for Morgana Shard?");

            _harmony = new Harmony("SoulForge");
            _harmony.PatchAll();

            Log.LogInfo("Plugin SoulForge is loaded.");
        }

        public override bool Unload()
        {
            _harmony?.UnpatchSelf();
            return true;
        }

        public bool IsGlowEnabled(PrefabGUID shardItem)
        {
            if (shardItem.Equals(Data.DraculaShardItem)) return EnableGlowDracula.Value;
            if (shardItem.Equals(Data.AdamShardItem)) return EnableGlowAdam.Value;
            if (shardItem.Equals(Data.HorrorShardItem)) return EnableGlowHorror.Value;
            if (shardItem.Equals(Data.SolarusShardItem)) return EnableGlowSolarus.Value;
            if (shardItem.Equals(Data.MorganaShardItem)) return EnableGlowMorgana.Value;
            return true;
        }
    }
}