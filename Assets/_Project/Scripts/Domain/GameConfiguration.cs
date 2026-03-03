using System;
using System.Collections.Generic;
using PaperClone.Presentation;
using UnityEngine;

namespace PaperClone.Application
{
    [Serializable]
    public class GameConfiguration
    {
        [Header("Game Settings")]
        public int BotCount = 5;
        public float MapBounds = 100f;
        public float MinSpawnDistance = 15f;

        [Header("Bot Stats")]
        public float BotSpeed = 6f;
        public List<Color> BotColors = new List<Color>
        {
            Color.red, Color.green, Color.magenta, Color.cyan, Color.yellow
        };
        
        [Header("Prefabs")]
        public BotView BotRootPrefab;
        public PlayerView PlayerVisualPrefab;
        public TerritoryView TerritoryPrefab;
        public TrailView TrailPrefab;
    }
}