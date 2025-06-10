using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using SoulForge.Utils;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;

namespace SoulForge.Patches
{
    [HarmonyPatch(typeof(PlaceTileModelSystem), nameof(PlaceTileModelSystem.OnUpdate))]
    public static class PlaceTileModelSystemPatch
    {
        [HarmonyPrefix]
        public static void Prefix(PlaceTileModelSystem __instance)
        {
            var em = VWorld.EntityManager;
            var placedEntities = __instance._BuildTileQuery.ToEntityArray(Allocator.Temp);

            foreach (var entity in placedEntities)
            {
                if (!em.HasComponent<BuildTileModelEvent>(entity)) continue;
                var buildEvent = em.GetComponentData<BuildTileModelEvent>(entity);
                var prefab = buildEvent.PrefabGuid;

                if (Data.BlockedContainers.Contains(prefab))
                {
                    bool blockIt = false;
                    if (prefab.Equals(Data.SolarusContainer))
                        blockIt = !Plugin.AllowSolarusContainer.Value;
                    else if (prefab.Equals(Data.MonsterContainer))
                        blockIt = !Plugin.AllowMonsterContainer.Value;
                    else if (prefab.Equals(Data.ManticoreContainer))
                        blockIt = !Plugin.AllowManticoreContainer.Value;
                    else if (prefab.Equals(Data.DraculaContainer))
                        blockIt = !Plugin.AllowDraculaContainer.Value;
                    else if (prefab.Equals(Data.MorganaContainer))
                        blockIt = !Plugin.AllowMorganaContainer.Value;

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
                                var message = new FixedString512Bytes("<color=#FF0000>Stashing of this shard has been disabled.</color>");
                                ServerChatUtils.SendSystemMessageToClient(em, user, ref message);
                            }
                        }
                        em.DestroyEntity(entity);
                    }
                }
            }
            placedEntities.Dispose();
        }
    }

    [HarmonyPatch(typeof(Equipment), nameof(Equipment.SetEquipped))]
    public static class OnItemEquippedPatch
    {
        public static void Postfix(EntityManager entityManager, Entity target, PrefabGUID statItemId)
        {
            if (!entityManager.HasComponent<PlayerCharacter>(target)) return;

            foreach (var glowBuff in Data.ShardNecklacesToVisualBuffs.Values)
            {
                Helpers.Unbuff(target, glowBuff);
            }

            if (Data.ShardNecklacesToVisualBuffs.TryGetValue(statItemId, out var glowBuffToApply))
            {
                if (Plugin.Instance.IsGlowEnabled(statItemId))
                {
                    var userEntity = entityManager.GetComponentData<PlayerCharacter>(target).UserEntity;
                    Helpers.BuffPlayer(target, userEntity, glowBuffToApply, 0, false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Equipment), nameof(Equipment.UnequipItem), new[] { typeof(EntityManager), typeof(Entity), typeof(EquipmentType) })]
    public static class OnItemUnequippedPatch
    {
        public static void Prefix(ref Equipment __instance, Entity target, EquipmentType equipmentType)
        {
            PrefabGUID unequippedItemId = __instance.GetEquipmentItemId(equipmentType);

            if (unequippedItemId.IsEmpty())
            {
                return;
            }

            if (Data.ShardNecklacesToVisualBuffs.TryGetValue(unequippedItemId, out var glowBuff))
            {
                Helpers.Unbuff(target, glowBuff);
            }
        }
    }
}