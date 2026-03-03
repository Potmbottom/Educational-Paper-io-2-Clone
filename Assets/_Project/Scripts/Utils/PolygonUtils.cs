using System.Collections.Generic;
using UnityEngine;
using Clipper2Lib;

namespace PaperClone.Utils
{
    public static class PolygonUtils
    {
        private const float Scale = 1000f;

        public static Path64 Vector3ToPath64(IEnumerable<Vector3> points)
        {
            Path64 path = new Path64();
            foreach (var p in points)
            {
                path.Add(new Point64(p.x * Scale, p.z * Scale));
            }
            return path;
        }

        public static List<Vector3> Path64ToVector3(Path64 path)
        {
            List<Vector3> result = new List<Vector3>();
            foreach (var p in path)
            {
                result.Add(new Vector3(p.X / Scale, 0.05f, p.Y / Scale));
            }
            return result;
        }

        // Checks if a point is inside a polygon
        public static bool IsPointInPolygon(Vector3 point, List<Vector3> polygon)
        {
            Point64 pt = new Point64(point.x * Scale, point.z * Scale);
            Path64 path = Vector3ToPath64(polygon);
            return Clipper.PointInPolygon(pt, path) != PointInPolygonResult.IsOutside;
        }
    }
}