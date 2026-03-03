using PaperClone.Domain;
using UnityEngine;
using UniRx;
using VContainer;

namespace PaperClone.Presentation
{
    [RequireComponent(typeof(LineRenderer))]
    public class TrailView : MonoBehaviour
    {
        private PlayerModel _model;
        private LineRenderer _lineRenderer;
        private bool _initialized = false;

        [Inject]
        public void Construct(PlayerModel model)
        {
            _model = model;
        }

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 0;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.startWidth = 0.5f;
            _lineRenderer.endWidth = 0.5f;
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

            var c = _model.PlayerColor;
            c.a = 0.5f;
            
            if (_lineRenderer.sharedMaterial == null) 
                _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            
            _lineRenderer.material.color = c;
            _lineRenderer.startColor = c;
            _lineRenderer.endColor = c;

            _model.Trail.ObserveAdd().Subscribe(evt => 
            {
                _lineRenderer.positionCount++;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, evt.Value);
            }).AddTo(this);

            _model.Trail.ObserveReset().Subscribe(_ => 
            {
                _lineRenderer.positionCount = 0;
            }).AddTo(this);
        }
    }
}