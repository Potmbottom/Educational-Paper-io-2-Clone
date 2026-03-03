// --- FILE: BotFactory.cs ---
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

        public BotFactory(
            GameConfiguration config, 
            LevelModel levelModel, 
            TerritoryCalculator calculator)
        {
            _config = config;
            _levelModel = levelModel;
            _calculator = calculator;
        }

        public (PlayerController controller, PlayerModel model) CreateBot(Vector3 startPosition, int index)
        {
            // 1. Create Model
            var model = new PlayerModel
            {
                Speed = _config.BotSpeed,
                PlayerColor = _config.BotColors[index % _config.BotColors.Count]
            };
            model.Position.Value = startPosition;
            model.ResetTerritoryToSpawn(startPosition);

            // 2. Create View (Root)
            var botRoot = Object.Instantiate(_config.BotRootPrefab);
            botRoot.Initialize($"Bot_{index}");
            botRoot.transform.position = startPosition;

            // 3. Assemble Visuals (Sub-views)
            // We manually instantiate and initialize the sub-components to keep strict separation
            SpawnVisual(botRoot.transform, _config.PlayerVisualPrefab, model);
            SpawnVisual(botRoot.transform, _config.TrailPrefab, model);
            SpawnVisual(botRoot.transform, _config.TerritoryPrefab, model);

            // 4. Create Input Strategy
            var aiInput = new AIInputProvider(model, _levelModel);

            // 5. Create Controller
            var controller = new PlayerController(model, _levelModel, aiInput, _calculator);

            return (controller, model);
        }

        // Helper to handle the "View.Initialize(Model)" pattern generic way
        private void SpawnVisual<T>(Transform parent, T prefab, PlayerModel model) where T : MonoBehaviour
        {
            var instance = Object.Instantiate(prefab, parent);
            // Reflection or Interface could be used here, but dynamic binding is fine for this scope
            // to support the existing Initialize methods in your view classes.
            if(instance is PlayerView pv) pv.Initialize(model);
            else if(instance is TrailView tv) tv.Initialize(model);
            else if(instance is TerritoryView tev) tev.Initialize(model);
        }
    }
}