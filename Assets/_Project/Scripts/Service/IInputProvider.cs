using UnityEngine;

namespace PaperClone.Service
{
    public interface IInputProvider
    {
        Vector3 GetDirection();
    }
}