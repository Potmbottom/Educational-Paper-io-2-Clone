// --- FILE: BotManager.cs ---
using System.Collections.Generic;
using PaperClone.Domain;
using UnityEngine;
using VContainer.Unity;

namespace PaperClone.Application
{
    public class BotManager : IStartable, ITickable
    {
        private readonly BotFactory _factory;
        private readonly GameConfiguration _config;
        private readonly PlayerModel _humanModel; // To check distance for spawning

        private readonly List<PlayerController> _activeControllers = new List<PlayerController>();

        public BotManager(
            BotFactory factory, 
            GameConfiguration config,
            PlayerModel humanModel)
        {
            _factory = factory;
            _config = config;
            _humanModel = humanModel;
        }

        public void Start()
        {
            var humanPos = _humanModel.Position.Value;

            for (int i = 0; i < _config.BotCount; i++)
            {
                var spawnPos = GetSafeSpawnPosition(humanPos);
                var (controller, _) = _factory.CreateBot(spawnPos, i);
                _activeControllers.Add(controller);
            }
        }

        public void Tick()
        {
            // Centralized Loop for all bots
            foreach (var controller in _activeControllers)
            {
                controller.Tick();
            }
        }

        private Vector3 GetSafeSpawnPosition(Vector3 avoidPos)
        {
            var limit = _config.MapBounds;
            
            for (var i = 0; i < 20; i++)
            {
                var x = Random.Range(-limit, limit);
                var z = Random.Range(-limit, limit);
                var candidate = new Vector3(x, 0, z);

                if (Vector3.Distance(candidate, avoidPos) >= _config.MinSpawnDistance)
                {
                    return candidate;
                }
            }
            // Fallback
            return new Vector3(limit * 0.5f, 0, limit * 0.5f);
        }
    }
}