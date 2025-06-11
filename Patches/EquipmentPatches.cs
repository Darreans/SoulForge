using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using SoulForge.Utils;
using Stunlock.Core;
using Unity.Entities;

namespace SoulForge.Patches
{
    [HarmonyPatch(typeof(Equipment), nameof(Equipment.SetEquipped))]
    public static class OnItemEquippedPatch
    {
        // THIS IS THE CORRECTED METHOD
        public static void Postfix(EntityManager entityManager, Entity target, PrefabGUID statItemId)
        {
            if (!entityManager.HasComponent<PlayerCharacter>(target)) return;


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