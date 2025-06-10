using ProjectM;
using ProjectM.Network;
using ProjectM.Shared;
using SoulForge.Utils;
using Stunlock.Core;
using Unity.Entities;

namespace SoulForge
{
    public static class Helpers
    {
        private static bool TryGetBuffEntity(Entity character, PrefabGUID buffGuid, out Entity buffEntity)
        {
            var em = VWorld.EntityManager;
            buffEntity = Entity.Null;
            if (!em.HasComponent<BuffBuffer>(character))
            {
                return false;
            }

            DynamicBuffer<BuffBuffer> buffBuffer = em.GetBuffer<BuffBuffer>(character);
            foreach (var buff in buffBuffer)
            {
                if (buff.PrefabGuid.Equals(buffGuid))
                {
                    buffEntity = buff.Entity;
                    return true;
                }
            }
            return false;
        }

        public static bool BuffPlayer(Entity character, Entity user, PrefabGUID buffPrefab, int duration = -1, bool persistsThroughDeath = false)
        {
            if (TryGetBuffEntity(character, buffPrefab, out _))
            {
                return false;
            }

            var em = VWorld.EntityManager;
            var debugEventsSystem = VWorld.Server.GetExistingSystemManaged<DebugEventsSystem>();
            var fromCharacter = new FromCharacter { User = user, Character = character };
            debugEventsSystem.ApplyBuff(fromCharacter, new ApplyBuffDebugEvent { BuffPrefabGUID = buffPrefab });

            if (TryGetBuffEntity(character, buffPrefab, out var buffEntity))
            {
                if (em.HasComponent<CreateGameplayEventsOnSpawn>(buffEntity))
                    em.RemoveComponent<CreateGameplayEventsOnSpawn>(buffEntity);

                if (em.HasComponent<GameplayEventListeners>(buffEntity))
                    em.RemoveComponent<GameplayEventListeners>(buffEntity);

                if (persistsThroughDeath)
                {
                    em.AddComponent<Buff_Persists_Through_Death>(buffEntity);
                }

                if (em.HasComponent<LifeTime>(buffEntity))
                {
                    var lifeTime = em.GetComponentData<LifeTime>(buffEntity);
                    if (duration == 0)
                    {
                        lifeTime.Duration = -1;
                        lifeTime.EndAction = LifeTimeEndAction.None;
                    }
                    else if (duration > 0)
                    {
                        lifeTime.Duration = duration;
                    }
                    em.SetComponentData(buffEntity, lifeTime);
                }
                return true;
            }
            return false;
        }

        public static void Unbuff(Entity character, PrefabGUID buffPrefab)
        {
            var em = VWorld.EntityManager;
            if (TryGetBuffEntity(character, buffPrefab, out var buffEntity))
            {
                DestroyUtility.Destroy(em, buffEntity, DestroyDebugReason.TryRemoveBuff);
            }
        }
    }
}