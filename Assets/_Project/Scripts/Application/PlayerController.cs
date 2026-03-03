using System.Linq;
using PaperClone.Domain;
using PaperClone.Service;
using PaperClone.Utils;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace PaperClone.Application
{
    public class PlayerController : ITickable, System.IDisposable
    {
        private readonly PlayerModel _model;
        private readonly LevelModel _levelModel;
        private readonly IInputProvider _input;
        private readonly TerritoryCalculator _calculator;
        private readonly PlayerRegistry _registry;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private Vector3 _lastTrailPos;
        private bool _isOutsideTerritory = false;

        public PlayerModel Model => _model;

        public PlayerController(
            PlayerModel model, 
            LevelModel levelModel, 
            IInputProvider input,
            TerritoryCalculator calculator,
            PlayerRegistry registry) 
        {
            _model = model;
            _levelModel = levelModel;
            _input = input;
            _calculator = calculator;
            
            _registry = registry;
            _registry.Register(this);
            
            _lastTrailPos = model.Position.Value;
            
            _model.OwnedTerritory.Subscribe(paths => 
            {
                double area = _calculator.CalculateTotalArea(paths);
                double mapWidth = _levelModel.Bounds * 2.0; 
                double totalArea = mapWidth * mapWidth;
                _model.TerritoryPercentage.Value = (float)((area / totalArea) * 100.0);
            }).AddTo(_disposables);
        }

        public void Dispose()
        {
            _registry.Unregister(this);
            _disposables.Dispose();
        }

        public void Tick()
        {
            var dt = Time.deltaTime;
            var inputDir = _input.GetDirection();
            var currentPos = _model.Position.Value;

            if (inputDir != Vector3.zero)
            {
                var targetRot = Quaternion.LookRotation(inputDir);
                _model.Rotation.Value = Quaternion.Slerp(_model.Rotation.Value, targetRot, dt * 10f);
            }
            
            var forward = _model.Rotation.Value * Vector3.forward;
            var nextPos = currentPos + (forward * _model.Speed * dt);
            
            var b = _levelModel.Bounds;
            nextPos.x = Mathf.Clamp(nextPos.x, -b, b);
            nextPos.z = Mathf.Clamp(nextPos.z, -b, b);
            
            if (_isOutsideTerritory && _model.Trail.Count > 2)
            {
                if (IsCrossingSelf(currentPos, nextPos))
                {
                    Die();
                    return; 
                }
            }

            foreach (var otherController in _registry.AllControllers.ToList())
            {
                if (otherController == this) continue;

                if (IsCrossingOther(currentPos, nextPos, otherController.Model))
                {
                    otherController.Die();
                }
            }

            _model.Position.Value = nextPos;
            UpdateTerritoryState(nextPos);
        }

        public void Die()
        {
            var limit = _levelModel.Bounds - 3f;
            var rx = Random.Range(-limit, limit);
            var rz = Random.Range(-limit, limit);
            var respawnPos = new Vector3(rx, 0, rz);
            
            _model.Kill(respawnPos);
            
            _isOutsideTerritory = false;
            _lastTrailPos = respawnPos;
        }

        private bool IsCrossingSelf(Vector3 currentPos, Vector3 nextPos)
        {
            var safetyCount = 2; 
            var count = _model.Trail.Count;

            if (count <= safetyCount) return false;

            for (var i = 0; i < count - safetyCount - 1; i++)
            {
                var p1 = _model.Trail[i];
                var p2 = _model.Trail[i+1];

                if (GeometryUtils.IsSegmentIntersecting(currentPos, nextPos, p1, p2))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsCrossingOther(Vector3 currentPos, Vector3 nextPos, PlayerModel other)
        {
            var count = other.Trail.Count;
            if (count == 0) return false;

            for (var i = 0; i < count - 1; i++)
            {
                var p1 = other.Trail[i];
                var p2 = other.Trail[i + 1];

                if (GeometryUtils.IsSegmentIntersecting(currentPos, nextPos, p1, p2)) return true;
            }

            if (count > 0)
            {
                var lastTrail = other.Trail[count - 1];
                var otherPos = other.Position.Value;
                if (GeometryUtils.IsSegmentIntersecting(currentPos, nextPos, lastTrail, otherPos)) return true;
            }

            return false;
        }

        private void UpdateTerritoryState(Vector3 currentPos)
        {
            var isInside = _calculator.IsPointInTerritory(_model.OwnedTerritory.Value, currentPos);

            if (!isInside)
            {
                _isOutsideTerritory = true;
                if (Vector3.Distance(currentPos, _lastTrailPos) > _model.TrailSpawnDistance)
                {
                    _model.Trail.Add(currentPos);
                    _lastTrailPos = currentPos;
                }
            }
            else
            {
                if (_isOutsideTerritory && _model.Trail.Count > 0)
                {
                    CaptureTerritory();
                    _isOutsideTerritory = false;
                }
            }
        }

        private void CaptureTerritory()
        {
            var newTerritory = _calculator.CalculateExpansion(_model.OwnedTerritory.Value, _model.Trail.ToList());
            _model.OwnedTerritory.Value = newTerritory;
            _model.Trail.Clear();
            _lastTrailPos = _model.Position.Value;

            foreach (var otherController in _registry.AllControllers.ToList())
            {
                if (otherController == this) continue;

                var otherModel = otherController.Model;
                var reduced = _calculator.SubtractTerritory(otherModel.OwnedTerritory.Value, newTerritory);

                if (reduced == null || reduced.Count == 0)
                {
                    otherController.Die();
                }
                else
                {
                    otherModel.OwnedTerritory.Value = reduced;
                }
            }
        }
    }
}