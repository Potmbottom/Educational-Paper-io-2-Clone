using Clipper2Lib;
using UniRx;
using UnityEngine;

namespace PaperClone.Domain
{
    public class PlayerModel : ReactiveModel
    {
        public ReactiveProperty<Vector3> Position { get; } = new ReactiveProperty<Vector3>();
        public ReactiveProperty<Quaternion> Rotation { get; } = new ReactiveProperty<Quaternion>();
        public ReactiveCollection<Vector3> Trail { get; } = new ReactiveCollection<Vector3>();
        public ReactiveProperty<PathsD> OwnedTerritory { get; } = new ReactiveProperty<PathsD>();
        
        public float Speed = 5f;
        public float TrailSpawnDistance = 0.5f;
        public Color PlayerColor = Color.blue;
        
        private const double InitialSize = 3.0;

        public PlayerModel()
        {
            ResetTerritoryToSpawn(Vector3.zero);
        }
        
        public void Kill(Vector3 respawnPos)
        {
            Debug.Log($"Player {PlayerColor} Died! Respawning at {respawnPos}");
            
            Trail.Clear();
            Position.Value = respawnPos;
            ResetTerritoryToSpawn(respawnPos);
        }

        public void ResetTerritoryToSpawn(Vector3 centerPos)
        {
            double cx = centerPos.x;
            double cy = centerPos.z;
            double halfSize = InitialSize / 2.0;

            var initialPath = new PathD
            {
                new PointD(cx - halfSize, cy - halfSize),
                new PointD(cx + halfSize, cy - halfSize),
                new PointD(cx + halfSize, cy + halfSize),
                new PointD(cx - halfSize, cy + halfSize)
            };

            OwnedTerritory.Value = new PathsD { initialPath };
        }
    }
}