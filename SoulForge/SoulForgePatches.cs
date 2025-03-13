using HarmonyLib;
using ProjectM;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using ProjectM.Network;
using Bloodstone.API;

namespace SoulForge
{
    [HarmonyPatch(typeof(PlaceTileModelSystem), nameof(PlaceTileModelSystem.OnUpdate))]
    public static class PlaceTileModelSystemPatch
    {
        [HarmonyPrefix]
        public static void Prefix(PlaceTileModelSystem __instance)
        {
            var em = VWorld.Server.EntityManager;
            var placedEntities = __instance._BuildTileQuery.ToEntityArray(Allocator.Temp);

            foreach (var entity in placedEntities)
            {
                if (!em.HasComponent<BuildTileModelEvent>(entity)) continue;
                var buildEvent = em.GetComponentData<BuildTileModelEvent>(entity);
                var prefab = buildEvent.PrefabGuid;

                if (SoulForgeData.BlockedContainers.Contains(prefab))
                {
                    bool blockIt = false;
                    if (prefab.Equals(SoulForgeData.SolarusContainer))
                        blockIt = !SoulForgePlugin.AllowSolarusContainer.Value;
                    else if (prefab.Equals(SoulForgeData.MonsterContainer))
                        blockIt = !SoulForgePlugin.AllowMonsterContainer.Value;
                    else if (prefab.Equals(SoulForgeData.ManticoreContainer))
                        blockIt = !SoulForgePlugin.AllowManticoreContainer.Value;
                    else if (prefab.Equals(SoulForgeData.DraculaContainer))
                        blockIt = !SoulForgePlugin.AllowDraculaContainer.Value;

                    if (blockIt)
                    {
                        Entity possiblePlayer = Entity.Null;
                        if (em.HasComponent<FromCharacter>(entity))
                        {
                            var fromChar = em.GetComponentData<FromCharacter>(entity).Character;
                            if (fromChar != Entity.Null && em.HasComponent<PlayerCharacter>(fromChar))
                                possiblePlayer = fromChar;
                        }

                        if (possiblePlayer == Entity.Null && em.HasComponent<EntityOwner>(entity))
                        {
                            var owner = em.GetComponentData<EntityOwner>(entity).Owner;
                            if (owner != Entity.Null && em.HasComponent<PlayerCharacter>(owner))
                                possiblePlayer = owner;
                        }

                        if (possiblePlayer != Entity.Null)
                        {
                            var pc = em.GetComponentData<PlayerCharacter>(possiblePlayer);
                            if (pc.UserEntity != Entity.Null && em.HasComponent<User>(pc.UserEntity))
                            {
                                var user = em.GetComponentData<User>(pc.UserEntity);
                                ServerChatUtils.SendSystemMessageToClient(
                                    em,
                                    user,
                                    "<color=#FF0000>Stashing of this shard has been disabled.</color>"
                                );
                            }
                        }

                        em.DestroyEntity(entity);
                    }
                }
            }

            placedEntities.Dispose();
        }
    }

    [HarmonyPatch(typeof(ModifyUnitStatBuffSystem_Spawn), nameof(ModifyUnitStatBuffSystem_Spawn.OnUpdate))]
    public static class SoulGlowSpawnPatch
    {
        [HarmonyPrefix]
        public static void Prefix(ModifyUnitStatBuffSystem_Spawn __instance)
        {
            var em = VWorld.Server.EntityManager;
            var spawnEntities = __instance.__query_1735840491_0.ToEntityArray(Allocator.Temp);

            foreach (var buffEntity in spawnEntities)
            {
                if (!em.HasComponent<PrefabGUID>(buffEntity)) continue;
                var shardItemPrefab = em.GetComponentData<PrefabGUID>(buffEntity);

                if (SoulForgeData.ShardNecklacesToVisualBuffs.TryGetValue(shardItemPrefab, out var glowBuff))
                {
                    if (!SoulForgePlugin.Instance.IsGlowEnabled(shardItemPrefab)) continue;
                    if (em.HasComponent<EntityOwner>(buffEntity))
                    {
                        var owner = em.GetComponentData<EntityOwner>(buffEntity).Owner;
                        if (em.HasComponent<PlayerCharacter>(owner))
                        {
                            var pc = em.GetComponentData<PlayerCharacter>(owner);
                            SoulForgeHelpers.BuffPlayer(
                                character: owner,
                                user: pc.UserEntity,
                                buffPrefab: glowBuff,
                                duration: 0,
                                persistsThroughDeath: false
                            );
                        }
                    }
                }
            }
            spawnEntities.Dispose();
        }
    }

    [HarmonyPatch(typeof(ModifyUnitStatBuffSystem_Destroy), nameof(ModifyUnitStatBuffSystem_Destroy.OnUpdate))]
    public static class SoulGlowDestroyPatch
    {
        [HarmonyPrefix]
        public static void Prefix(ModifyUnitStatBuffSystem_Destroy __instance)
        {
            var em = VWorld.Server.EntityManager;
            var destroyEntities = __instance.__query_1735840524_0.ToEntityArray(Allocator.Temp);

            foreach (var buffEntity in destroyEntities)
            {
                if (!em.HasComponent<PrefabGUID>(buffEntity)) continue;
                var shardItemPrefab = em.GetComponentData<PrefabGUID>(buffEntity);

                if (SoulForgeData.ShardNecklacesToVisualBuffs.TryGetValue(shardItemPrefab, out var glowBuff))
                {
                    if (em.HasComponent<EntityOwner>(buffEntity))
                    {
                        var owner = em.GetComponentData<EntityOwner>(buffEntity).Owner;
                        if (em.HasComponent<PlayerCharacter>(owner))
                        {
                            SoulForgeHelpers.Unbuff(owner, glowBuff);
                        }
                    }
                }
            }
            destroyEntities.Dispose();
        }
    }
}
