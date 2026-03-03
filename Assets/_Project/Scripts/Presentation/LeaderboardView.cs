using UnityEngine;
using VContainer;
using System.Linq;
using PaperClone.Application;

namespace PaperClone.Presentation
{
    public class LeaderboardView : MonoBehaviour
    {
        private PlayerRegistry _registry;
        
        [Header("UI Settings")]
        [SerializeField] private float _width = 350f;
        [SerializeField] private int _fontSize = 24;
        [SerializeField] private float _rowHeight = 40f;
        [SerializeField] private float _topPadding = 20f;
        [SerializeField] private float _rightPadding = 20f;

        private GUIStyle _playerLabelStyle;
        private GUIStyle _headerStyle;

        [Inject]
        public void Construct(PlayerRegistry registry)
        {
            _registry = registry;
        }

        private void OnGUI()
        {
            if (_registry == null || _registry.AllControllers.Count == 0) return;
            
            if (_playerLabelStyle == null)
            {
                _playerLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = _fontSize,
                    alignment = TextAnchor.MiddleLeft 
                };
            }
            else
            {
                _playerLabelStyle.fontSize = _fontSize;
            }

            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(GUI.skin.box)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = _fontSize + 2,
                    alignment = TextAnchor.UpperCenter
                };
                _headerStyle.normal.textColor = Color.white;
            }
            else
            {
                _headerStyle.fontSize = _fontSize + 2;
            }
            
            var sortedPlayers = _registry.AllControllers
                .OrderByDescending(c => c.Model.TerritoryPercentage.Value)
                .ToList();
            
            var headerHeight = _rowHeight * 1.2f;
            var totalHeight = headerHeight + (sortedPlayers.Count * _rowHeight);
            
            var x = Screen.width - _width - _rightPadding;
            var y = _topPadding;
            GUI.Box(new Rect(x, y, _width, totalHeight), "Leaderboard", _headerStyle);

            for (var i = 0; i < sortedPlayers.Count; i++)
            {
                var model = sortedPlayers[i].Model;
                _playerLabelStyle.normal.textColor = model.PlayerColor;
                var text = $"{i + 1}. {model.Name} - {model.TerritoryPercentage.Value:F1}%";
                var rowY = y + headerHeight + (i * _rowHeight);
                GUI.Label(new Rect(x + 15, rowY - 5, _width - 30, _rowHeight), text, _playerLabelStyle);
            }
        }
    }
}