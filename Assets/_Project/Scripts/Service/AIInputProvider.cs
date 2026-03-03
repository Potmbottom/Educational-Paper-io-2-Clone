using UnityEngine;
using PaperClone.Domain;
using PaperClone.Utils;

namespace PaperClone.Service
{
    public class AIInputProvider : IInputProvider
    {
        private readonly PlayerModel _myModel;
        private readonly LevelModel _levelModel;

        private Vector3 _currentTarget;
        private Vector3 _safeHomePosition;
        private AIState _currentState;
        
        private const float ExpansionDistance = 25.0f; // How far to go OUT
        private const float SideDistance = 15.0f;      // How far to go SIDEWAYS (Width)
        private const float ReachThreshold = 2.0f;
        private const int PanicTrailLength = 80;

        private enum AIState
        {
            Idle,           // Safe at home
            MovingOut,      // Step 1: Go deep into map
            MovingSideways, // Step 2: Move lateral to create WIDTH
            Returning       // Step 3: Go home to close loop
        }

        public AIInputProvider(PlayerModel model, LevelModel levelModel)
        {
            _myModel = model;
            _levelModel = levelModel;
            _currentState = AIState.Idle;
        }

        public Vector3 GetDirection()
        {
            var currentPos = _myModel.Position.Value;
            
            if (_myModel.Trail.Count == 0)
            {
                _currentState = AIState.Idle;
            }
            
            if (_currentState != AIState.Idle && _currentState != AIState.Returning)
            {
                if (_myModel.Trail.Count > PanicTrailLength)
                {
                    _currentState = AIState.Returning;
                }
            }
            
            switch (_currentState)
            {
                case AIState.Idle:
                    _safeHomePosition = currentPos;
                    PickOutboundTarget(currentPos);
                    _currentState = AIState.MovingOut;
                    break;

                case AIState.MovingOut:
                    if (HasReachedTarget(currentPos))
                    {
                        PickSidewaysTarget(currentPos);
                        _currentState = AIState.MovingSideways;
                    }
                    break;

                case AIState.MovingSideways:
                    if (HasReachedTarget(currentPos))
                    {
                        _currentState = AIState.Returning;
                    }
                    break;

                case AIState.Returning:
                    _currentTarget = _safeHomePosition;
                    break;
            }

            
            var direction = (_currentTarget - currentPos).normalized;
            return ApplyCollisionAvoidance(currentPos, direction);
        }

        private bool HasReachedTarget(Vector3 currentPos)
        {
            return Vector3.Distance(currentPos, _currentTarget) < ReachThreshold;
        }

        private void PickOutboundTarget(Vector3 origin)
        {
            var randomDir = Random.insideUnitCircle.normalized;
            var offset = new Vector3(randomDir.x, 0, randomDir.y) * ExpansionDistance;
            
            _currentTarget = ClampToMap(origin + offset);
        }

        private void PickSidewaysTarget(Vector3 currentPos)
        {
            var vectorFromHome = (currentPos - _safeHomePosition).normalized;
            var side = (Random.value > 0.5f) ? 1f : -1f;
            var perpDir = Vector3.Cross(vectorFromHome, Vector3.up) * side;
            var offset = perpDir * SideDistance;
            var biasHome = (_safeHomePosition - currentPos).normalized * 2.0f;
            
            _currentTarget = ClampToMap(currentPos + offset + biasHome);
        }

        private Vector3 ClampToMap(Vector3 target)
        {
            var b = _levelModel.Bounds * 0.9f;
            target.x = Mathf.Clamp(target.x, -b, b);
            target.z = Mathf.Clamp(target.z, -b, b);
            return target;
        }

        private Vector3 ApplyCollisionAvoidance(Vector3 currentPos, Vector3 desiredDir)
        {
            var lookAhead = currentPos + desiredDir * 3.0f;

            if (IsSelfIntersecting(currentPos, lookAhead))
            {
                return Vector3.Cross(desiredDir, Vector3.up).normalized;
            }
            return desiredDir;
        }

        private bool IsSelfIntersecting(Vector3 start, Vector3 end)
        {
            if (_myModel.Trail.Count < 5) return false;

            for (var i = 0; i < _myModel.Trail.Count - 5; i++)
            {
                var p1 = _myModel.Trail[i];
                var p2 = _myModel.Trail[i + 1];
                if (GeometryUtils.IsSegmentIntersecting(start, end, p1, p2)) return true;
            }
            return false;
        }
    }
}