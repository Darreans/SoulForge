using HarmonyLib;
using ProjectM;
using SoulForge.Utils;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;

namespace SoulForge.Patches
{
    [HarmonyPatch(typeof(ServerTimeSystem_Server), nameof(ServerTimeSystem_Server.OnUpdate))]
    public static class ServerTimeSystem_Patch
    {
        public static void Postfix()
        {
            if (Plugin.Instance.StartupCheckDone || !VWorld.IsServerReady())
            {
                return;
            }

            Plugin.Instance.Log.LogInfo("Server is ready. Performing one-time check for equipped shards...");

            var em = VWorld.EntityManager;
            var playerQuery = em.CreateEntityQuery(ComponentType.ReadOnly<PlayerCharacter>(), ComponentType.ReadOnly<Equipment>());
            var players = playerQuery.ToEntityArray(Allocator.Temp);

            foreach (var playerEntity in players)
            {
                var equipment = em.GetComponentData<Equipment>(playerEntity);

                var equippedItems = new NativeList<Entity>(Allocator.Temp);
                equipment.GetAllEquipmentEntities(equippedItems);

                foreach (var itemEntity in equippedItems)
                {
                    if (em.HasComponent<PrefabGUID>(itemEntity))
                    {
                        var equippedItemId = em.GetComponentData<PrefabGUID>(itemEntity);

                        if (Data.ShardNecklacesToVisualBuffs.TryGetValue(equippedItemId, out var glowBuff))
                        {
                            if (Plugin.Instance.IsGlowEnabled(equippedItemId))
                            {
                                Plugin.Instance.Log.LogInfo($"Found player with shard [{equippedItemId.GuidHash}] on startup. Re-applying glow buff.");
                                var userEntity = em.GetComponentData<PlayerCharacter>(playerEntity).UserEntity;
                                Helpers.BuffPlayer(playerEntity, userEntity, glowBuff, 0, false);
                            }
                        }
                    }
                }
                equippedItems.Dispose();
            }

            players.Dispose();

            Plugin.Instance.Log.LogInfo("Shard check complete.");
            Plugin.Instance.StartupCheckDone = true;
        }
    }
}