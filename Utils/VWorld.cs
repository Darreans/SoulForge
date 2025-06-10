using ProjectM;
using Unity.Entities;

namespace SoulForge.Utils
{
    public static class VWorld
    {
        private static World _serverWorld;
        private static EntityManager _entityManager;

        public static World Server
        {
            get
            {
                if (_serverWorld != null && _serverWorld.IsCreated) return _serverWorld;
                _serverWorld = GetWorld("Server");
                if (_serverWorld != null) _entityManager = _serverWorld.EntityManager;
                else _entityManager = default;
                return _serverWorld;
            }
        }

        public static EntityManager EntityManager
        {
            get
            {
                if (_entityManager != default && (_serverWorld == null || !_serverWorld.IsCreated)) _entityManager = default;
                if (_entityManager == default && Server != null) _entityManager = Server.EntityManager;
                return _entityManager;
            }
        }

        private static World GetWorld(string name)
        {
            if (World.s_AllWorlds == null) return null;
            foreach (var world in World.s_AllWorlds)
            {
                if (world.Name == name && world.IsCreated) return world;
            }
            return null;
        }

        public static bool IsServerReady()
        {
            return Server != null && Server.IsCreated && EntityManager != default;
        }
    }
}