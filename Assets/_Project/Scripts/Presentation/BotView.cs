// --- FILE: BotView.cs (Refactored from BotAgent) ---
using PaperClone.Domain;
using PaperClone.Presentation;
using UnityEngine;

namespace PaperClone.Application
{
    public class BotView : MonoBehaviour
    {
        // This view is now just a container for the visual representation
        // It does NOT contain logic, models, or controllers.
        
        public void Initialize(string botName)
        {
            name = botName;
        }
    }
}