using Bloodstone.API;
using Bloody.Core.API.v1;
using Bloody.Core;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Entities;

namespace SoulForge
{
    public static class SoulForgeHelpers
    {
        public static bool BuffPlayer(Entity character, Entity user, PrefabGUID buffPrefab, int duration = -1, bool persistsThroughDeath = false)
        {
            var em = VWorld.Server.EntityManager;
            var debugSys = VWorld.Server.GetExistingSystemManaged<DebugEventsSystem>();
            if (debugSys == null) return false;

            if (BuffUtility.TryGetBuff(em, character, buffPrefab, out var _))
            {
                return false;
            }

      
            var applyEvent = new ApplyBuffDebugEvent { BuffPrefabGUID = buffPrefab };
            var fromChar = new FromCharacter { User = user, Character = character };
            debugSys.ApplyBuff(fromChar, applyEvent);

            if (BuffUtility.TryGetBuff(em, character, buffPrefab, out var newBuff))
            {
                if (newBuff.Has<CreateGameplayEventsOnSpawn>())
                    newBuff.Remove<CreateGameplayEventsOnSpawn>();
                if (newBuff.Has<GameplayEventListeners>())
                    newBuff.Remove<GameplayEventListeners>();

                if (persistsThroughDeath)
                {
                    newBuff.Add<Buff_Persists_Through_Death>();
                    if (newBuff.Has<RemoveBuffOnGameplayEvent>())
                        newBuff.Remove<RemoveBuffOnGameplayEvent>();
                    if (newBuff.Has<RemoveBuffOnGameplayEventEntry>())
                        newBuff.Remove<RemoveBuffOnGameplayEventEntry>();
                }

                if (duration > 0)
                {
                    if (newBuff.Has<LifeTime>())
                    {
                        var life = newBuff.Read<LifeTime>();
                        life.Duration = duration;
                        newBuff.Write(life);
                    }
                }
                else if (duration == 0)
                {
                    if (newBuff.Has<LifeTime>())
                    {
                        var life = newBuff.Read<LifeTime>();
                        life.Duration = -1f;
                        life.EndAction = LifeTimeEndAction.None;
                        newBuff.Write(life);
                    }
                    if (newBuff.Has<RemoveBuffOnGameplayEvent>())
                        newBuff.Remove<RemoveBuffOnGameplayEvent>();
                    if (newBuff.Has<RemoveBuffOnGameplayEventEntry>())
                        newBuff.Remove<RemoveBuffOnGameplayEventEntry>();
                }

                return true;
            }

            return false;
        }

        public static void Unbuff(Entity character, PrefabGUID buffPrefab)
        {
            BuffSystem.Unbuff(character, buffPrefab);
        }
    }
}

//add kill recharging soon maybe, idk.