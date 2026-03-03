using UnityEngine;

namespace PaperClone.Utils
{
    public static class GeometryUtils
    {
        /// <summary>
        /// Checks if line segment (p1-p2) intersects with (p3-p4).
        /// </summary>
        public static bool IsSegmentIntersecting(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            Vector2 a = new Vector2(p1.x, p1.z);
            Vector2 b = new Vector2(p2.x, p2.z);
            Vector2 c = new Vector2(p3.x, p3.z);
            Vector2 d = new Vector2(p4.x, p4.z);

            float denominator = (d.y - c.y) * (b.x - a.x) - (d.x - c.x) * (b.y - a.y);
            
            if (Mathf.Abs(denominator) < Mathf.Epsilon) return false;

            float ua = ((d.x - c.x) * (a.y - c.y) - (d.y - c.y) * (a.x - c.x)) / denominator;
            float ub = ((b.x - a.x) * (a.y - c.y) - (b.y - a.y) * (a.x - c.x)) / denominator;
            return (ua > 0.001f && ua < 0.999f && ub > 0.001f && ub < 0.999f);
        }
    }
}