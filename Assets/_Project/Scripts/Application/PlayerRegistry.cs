using System.Collections.Generic;

namespace PaperClone.Application
{
    public class PlayerRegistry
    {
        public List<PlayerController> AllControllers { get; } = new List<PlayerController>();

        public void Register(PlayerController controller)
        {
            if (!AllControllers.Contains(controller))
                AllControllers.Add(controller);
        }

        public void Unregister(PlayerController controller)
        {
            AllControllers.Remove(controller);
        }
    }
}