using PaperClone.Domain;
using PaperClone.Presentation;
using PaperClone.Service;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PaperClone.Application
{
    public class BotFactory
    {
        private readonly GameConfiguration _config;
        private readonly LevelModel _levelModel;
        private readonly TerritoryCalculator _calculator;
        private readonly PlayerRegistry _registry;

        public BotFactory(
            GameConfiguration config, 
            LevelModel levelModel, 
            TerritoryCalculator calculator,
            PlayerRegistry registry)
        {
            _config = config;
            _levelModel = levelModel;
            _calculator = calculator;
            _registry = registry;
        }

        public PlayerController CreateBot(Vector3 startPosition, int index)
        {
            var model = new PlayerModel
            {
                Name = $"Bot {index + 1}",
                Speed = _config.BotSpeed,
                PlayerColor = _config.BotColors[index % _config.BotColors.Count]
            };
            model.Position.Value = startPosition;
            model.ResetTerritoryToSpawn(startPosition);

            var botRoot = Object.Instantiate(_config.BotRootPrefab);
            botRoot.Initialize($"Bot_{index}");
            botRoot.transform.position = startPosition;

            SpawnVisual(botRoot.transform, _config.PlayerVisualPrefab, model);
            SpawnVisual(botRoot.transform, _config.TrailPrefab, model);
            SpawnVisual(botRoot.transform, _config.TerritoryPrefab, model);

            var aiInput = new AIInputProvider(model, _levelModel);

            var controller = new PlayerController(model, _levelModel, aiInput, _calculator, _registry);

            return controller;
        }

        private void SpawnVisual<T>(Transform parent, T prefab, PlayerModel model) where T : MonoBehaviour
        {
            var instance = Object.Instantiate(prefab, parent);
            if(instance is PlayerView pv) pv.Initialize(model);
            else if(instance is TrailView tv) tv.Initialize(model);
            else if(instance is TerritoryView tev) tev.Initialize(model);
        }
    }
}