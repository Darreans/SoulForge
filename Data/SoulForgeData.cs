using Stunlock.Core;
using System.Collections.Generic;

namespace SoulForge
{
    public static class Data
    {
        public static PrefabGUID SolarusContainer = new PrefabGUID(-824445631);
        public static PrefabGUID MonsterContainer = new PrefabGUID(-1996942061);
        public static PrefabGUID ManticoreContainer = new PrefabGUID(653759442);
        public static PrefabGUID DraculaContainer = new PrefabGUID(1495743889);
        public static PrefabGUID MorganaContainer = new PrefabGUID(1724128982);

        public static HashSet<PrefabGUID> BlockedContainers = new()
        {
            SolarusContainer,
            MonsterContainer,
            ManticoreContainer,
            DraculaContainer,
            MorganaContainer
        };

        public static PrefabGUID DraculaShardItem = new PrefabGUID(666638454);
        public static PrefabGUID DraculaGlowBuff = new PrefabGUID(662242066);

        public static PrefabGUID AdamShardItem = new PrefabGUID(-1581189572);
        public static PrefabGUID AdamGlowBuff = new PrefabGUID(1237097606);

        public static PrefabGUID HorrorShardItem = new PrefabGUID(-1260254082);
        public static PrefabGUID HorrorGlowBuff = new PrefabGUID(1670636401);

        public static PrefabGUID SolarusShardItem = new PrefabGUID(-21943750);
        public static PrefabGUID SolarusGlowBuff = new PrefabGUID(1688799287);

        public static PrefabGUID MorganaShardItem = new PrefabGUID(1286615355);
        public static PrefabGUID MorganaGlowBuff = new PrefabGUID(-1841976861);

        public static Dictionary<PrefabGUID, PrefabGUID> ShardNecklacesToVisualBuffs = new()
        {
            { DraculaShardItem, DraculaGlowBuff },
            { AdamShardItem,    AdamGlowBuff    },
            { HorrorShardItem,  HorrorGlowBuff  },
            { SolarusShardItem, SolarusGlowBuff },
            { MorganaShardItem, MorganaGlowBuff }
        };
    }
}