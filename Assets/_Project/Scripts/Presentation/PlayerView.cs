using PaperClone.Domain;
using UnityEngine;
using UniRx;
using VContainer;

namespace PaperClone.Presentation
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        
        private PlayerModel _model;
        private bool _initialized = false;

        [Inject]
        public void Construct(PlayerModel model)
        {
            _model = model;
        }

        private void Start()
        {
            if (_model != null && !_initialized)
            {
                BindModel();
            }
        }
        
        public void Initialize(PlayerModel model)
        {
            _model = model;
            if (!_initialized)
            {
                BindModel();
            }
        }

        private void BindModel()
        {
            _initialized = true;

            if (_meshRenderer != null)
                _meshRenderer.material.color = _model.PlayerColor;
            
            _model.Position
                .Subscribe(pos => transform.position = pos)
                .AddTo(this);
            
            _model.Rotation
                .Subscribe(rot => transform.rotation = rot)
                .AddTo(this);
        }
    }
}