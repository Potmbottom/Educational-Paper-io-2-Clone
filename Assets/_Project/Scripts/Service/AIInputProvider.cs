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
        
        private int _currentPanicThreshold; 
        private int _waypointsVisited;
        private int _waypointsToVisitBeforeReturn;
        
        private float _noiseOffset;
        
        private enum AIState
        {
            Idle,
            Roaming,  
            Returning
        }

        public AIInputProvider(PlayerModel model, LevelModel levelModel)
        {
            _myModel = model;
            _levelModel = levelModel;
            _currentState = AIState.Idle;
            _noiseOffset = Random.value * 100f;
            ResetDecisionLogic();
        }

        public Vector3 GetDirection()
        {
            var currentPos = _myModel.Position.Value;
            if (_myModel.Trail.Count == 0)
            {
                if (_currentState != AIState.Idle)
                {
                    ResetDecisionLogic();
                }
                _currentState = AIState.Idle;
                _safeHomePosition = currentPos;
            }
            
            if (_currentState != AIState.Idle && _currentState != AIState.Returning)
            {
                if (_myModel.Trail.Count > _currentPanicThreshold)
                {
                    _currentState = AIState.Returning;
                }
            }
            
            switch (_currentState)
            {
                case AIState.Idle:
                    PickNextRoamingTarget(currentPos);
                    _currentState = AIState.Roaming;
                    break;

                case AIState.Roaming:
                    if (HasReachedTarget(currentPos))
                    {
                        _waypointsVisited++;
                        if (_waypointsVisited >= _waypointsToVisitBeforeReturn)
                        {
                            _currentState = AIState.Returning;
                        }
                        else
                        {
                            PickNextRoamingTarget(currentPos);
                        }
                    }
                    break;

                case AIState.Returning:
                    _currentTarget = _safeHomePosition;
                    break;
            }
            
            var baseDirection = (_currentTarget - currentPos).normalized;
            
            var noiseFreq = 0.5f;
            var noiseAmp = 0.3f; 
            var noise = Mathf.PerlinNoise(Time.time * noiseFreq, _noiseOffset) - 0.5f;
            
            var perp = Vector3.Cross(baseDirection, Vector3.up);
            var noisyDirection = (baseDirection + (perp * noise * noiseAmp)).normalized;
            return ApplyCollisionAvoidance(currentPos, noisyDirection);
        }

        private void ResetDecisionLogic()
        {
            _currentPanicThreshold = Random.Range(40, 120); 
            _waypointsToVisitBeforeReturn = Random.Range(2, 5); 
            _waypointsVisited = 0;
        }

        private bool HasReachedTarget(Vector3 currentPos)
        {
            return Vector3.Distance(currentPos, _currentTarget) < 2.5f;
        }

        private void PickNextRoamingTarget(Vector3 origin)
        {
            var stepDistance = Random.Range(10.0f, 35.0f);

            Vector3 direction;

            if (_waypointsVisited == 0)
            {
                var randomDir = Random.insideUnitCircle.normalized;
                direction = new Vector3(randomDir.x, 0, randomDir.y);
            }
            else
            {
                var randomDir = Random.insideUnitCircle.normalized;
                direction = new Vector3(randomDir.x, 0, randomDir.y);
            }

            var proposedPos = origin + (direction * stepDistance);
            _currentTarget = ClampToMap(proposedPos);
        }

        private Vector3 ClampToMap(Vector3 target)
        {
            var b = _levelModel.Bounds * 0.95f;
            target.x = Mathf.Clamp(target.x, -b, b);
            target.z = Mathf.Clamp(target.z, -b, b);
            return target;
        }

        private Vector3 ApplyCollisionAvoidance(Vector3 currentPos, Vector3 desiredDir)
        {
            var lookAheadDist = 4.0f;
            var lookAhead = currentPos + desiredDir * lookAheadDist;

            if (IsSelfIntersecting(currentPos, lookAhead))
            {
                var turnDir = (Time.time % 1.0f > 0.5f) ? 1.0f : -1.0f;
                return (Vector3.Cross(desiredDir, Vector3.up) * turnDir).normalized;
            }
            return desiredDir;
        }

        private bool IsSelfIntersecting(Vector3 start, Vector3 end)
        {
            if (_myModel.Trail.Count < 10) return false;
            for (var i = 0; i < _myModel.Trail.Count - 8; i++)
            {
                var p1 = _myModel.Trail[i];
                var p2 = _myModel.Trail[i + 1];
                if (GeometryUtils.IsSegmentIntersecting(start, end, p1, p2)) return true;
            }
            return false;
        }
    }
}