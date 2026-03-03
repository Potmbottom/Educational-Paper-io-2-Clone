using PaperClone.Domain;
using PaperClone.Presentation;
using PaperClone.Service;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PaperClone.Application
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameConfiguration _configuration;
        [SerializeField] private CameraFollow _camera;

        protected override void Configure(IContainerBuilder builder)
        {
            // 1. Register Configuration
            builder.RegisterInstance(_configuration);
            builder.RegisterInstance(_configuration.MapBounds).AsSelf(); // If LevelModel needs generic float, or:

            // 2. Core Domain Services
            builder.Register<LevelModel>(Lifetime.Singleton);
            builder.Register<TerritoryCalculator>(Lifetime.Singleton);

            // 3. Human Player (Scoped MVC)
            builder.Register<PlayerModel>(Lifetime.Scoped);
            builder.Register<KeyboardInputProvider>(Lifetime.Singleton).As<IInputProvider>();
            builder.RegisterEntryPoint<PlayerController>(); // Ticks the Human

            // 4. Bot System
            builder.Register<BotFactory>(Lifetime.Singleton);
            builder.RegisterEntryPoint<BotManager>(); // Starts and Ticks the Bots

            // 5. Setup Human View (Doing this in Start via a small component or EntryPoint is cleaner)
            builder.RegisterEntryPoint<HumanInitializer>();

            builder.RegisterComponent(_camera);
        }
    }

    // Small helper to spawn the Human Visuals using the config
    public class HumanInitializer : IStartable
    {
        private readonly GameConfiguration _config;
        private readonly PlayerModel _model;

        public HumanInitializer(GameConfiguration config, PlayerModel model)
        {
            _config = config;
            _model = model;
        }

        public void Start()
        {
            // Initialize Model
            _model.Position.Value = Vector3.zero;
            _model.PlayerColor = Color.blue; // Human Color
            _model.ResetTerritoryToSpawn(Vector3.zero);

            // Instantiate Visuals
            // Note: In a pure MVC, we might have a HumanFactory too, but for now:
            Object.Instantiate(_config.PlayerVisualPrefab).Initialize(_model);
            Object.Instantiate(_config.TrailPrefab).Initialize(_model);
            Object.Instantiate(_config.TerritoryPrefab).Initialize(_model);
        }
    }
}