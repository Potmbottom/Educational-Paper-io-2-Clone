using UnityEngine;

namespace PaperClone.Service
{
    public class KeyboardInputProvider : IInputProvider
    {
        public Vector3 GetDirection()
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");
            
            if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f) 
                return Vector3.zero;

            return new Vector3(h, 0, v).normalized;
        }
    }
}