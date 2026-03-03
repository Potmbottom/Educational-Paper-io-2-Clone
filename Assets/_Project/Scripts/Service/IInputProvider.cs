using UnityEngine;

namespace PaperClone.Service
{
    public interface IInputProvider
    {
        // Returns a normalized direction vector (x, z)
        Vector3 GetDirection();
    }
}