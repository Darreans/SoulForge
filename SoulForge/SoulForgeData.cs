using System.Collections.Generic;
using Stunlock.Core;



namespace SoulForge
{
    public static class SoulForgeData
    {
        public static PrefabGUID SolarusContainer = new PrefabGUID(-824445631);
        public static PrefabGUID MonsterContainer = new PrefabGUID(-1996942061);
        public static PrefabGUID ManticoreContainer = new PrefabGUID(653759442);
        public static PrefabGUID DraculaContainer = new PrefabGUID(1495743889);

        public static HashSet<PrefabGUID> BlockedContainers = new()
        {
            SolarusContainer,
            MonsterContainer,
            ManticoreContainer,
            DraculaContainer
        };

        public static PrefabGUID DraculaShardItem = new PrefabGUID(-504120321);
        public static PrefabGUID DraculaGlowBuff = new PrefabGUID(662242066);

        public static PrefabGUID AdamShardItem = new PrefabGUID(403228886);
        public static PrefabGUID AdamGlowBuff = new PrefabGUID(1237097606);

        public static PrefabGUID HorrorShardItem = new PrefabGUID(1002452390);
        public static PrefabGUID HorrorGlowBuff = new PrefabGUID(1670636401);

        public static PrefabGUID SolarusShardItem = new PrefabGUID(329820611);
        public static PrefabGUID SolarusGlowBuff = new PrefabGUID(1688799287);

        public static Dictionary<PrefabGUID, PrefabGUID> ShardNecklacesToVisualBuffs = new()
        {
            { DraculaShardItem,  DraculaGlowBuff  },
            { AdamShardItem,     AdamGlowBuff     },
            { HorrorShardItem,   HorrorGlowBuff   },
            { SolarusShardItem,  SolarusGlowBuff  },
        };
    }
}
