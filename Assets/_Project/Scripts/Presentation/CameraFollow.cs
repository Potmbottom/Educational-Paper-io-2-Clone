using PaperClone.Domain;
using UnityEngine;
using UniRx;
using VContainer;

namespace PaperClone.Presentation
{
    public class CameraFollow : MonoBehaviour
    {
        private PlayerModel _target;
        
        [Header("Settings")]
        [SerializeField] private Vector3 _offset = new Vector3(0, 15f, -8f);
        [SerializeField] private float _smoothSpeed = 5f;
        [SerializeField] private float _lookAtOffset = 2f;

        [Inject]
        public void Construct(PlayerModel playerModel)
        {
            _target = playerModel;
        }

        private void LateUpdate()
        {
            if (_target == null) return;
            
            var targetPos = _target.Position.Value;
            var desiredPos = targetPos + _offset;
            var smoothedPos = Vector3.Lerp(transform.position, desiredPos, _smoothSpeed * Time.deltaTime);
            transform.position = smoothedPos;
            transform.LookAt(targetPos + (Vector3.forward * _lookAtOffset));
        }
    }
}