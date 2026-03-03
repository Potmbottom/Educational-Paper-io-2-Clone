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
            builder.RegisterInstance(_configuration);
            builder.RegisterInstance(_configuration.MapBounds).AsSelf();
            builder.Register<LevelModel>(Lifetime.Singleton);
            builder.Register<TerritoryCalculator>(Lifetime.Singleton);
            builder.Register<PlayerRegistry>(Lifetime.Singleton);
            builder.Register<PlayerModel>(Lifetime.Scoped);
            builder.Register<KeyboardInputProvider>(Lifetime.Singleton).As<IInputProvider>();
            builder.RegisterEntryPoint<PlayerController>();
            builder.Register<BotFactory>(Lifetime.Singleton);
            builder.RegisterEntryPoint<BotManager>();
            builder.RegisterEntryPoint<HumanInitializer>();
            builder.RegisterComponentOnNewGameObject<LeaderboardView>(Lifetime.Singleton, "LeaderboardUI");
            builder.RegisterBuildCallback(resolver => { resolver.Resolve<LeaderboardView>(); });
            builder.RegisterComponent(_camera);
        }
    }

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
            _model.Name = "Player";
            _model.Position.Value = Vector3.zero;
            _model.PlayerColor = Color.blue;
            _model.ResetTerritoryToSpawn(Vector3.zero);

            Object.Instantiate(_config.PlayerVisualPrefab).Initialize(_model);
            Object.Instantiate(_config.TrailPrefab).Initialize(_model);
            Object.Instantiate(_config.TerritoryPrefab).Initialize(_model);
        }
    }
}