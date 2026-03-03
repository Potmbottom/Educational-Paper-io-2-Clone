using System.Collections.Generic;
using Clipper2Lib;
using UnityEngine;

namespace PaperClone.Service
{
    public class TerritoryCalculator
    {
        private const int    CP                 = 4;   
        private const double TrailInflateAmount  = 0.2; 
        private const double MorphCloseAmount    = 0.3; 

        public bool IsPointInTerritory(PathsD territory, Vector3 point)
        {
            var p = new PointD(point.x, point.z);
            var inOuter = false;
            var inHole = false;

            foreach (var path in territory)
            {
                if (Clipper.PointInPolygon(p, path) != PointInPolygonResult.IsOutside)
                {
                    if (Clipper.Area(path) >= 0)
                        inOuter = true;
                    else
                        inHole = true;
                }
            }
            return inOuter && !inHole;
        }

        public PathsD CalculateExpansion(PathsD currentTerritory, List<Vector3> trail)
        {
            if (trail.Count < 3) return currentTerritory;

            var trailPath = new PathD(trail.Count);
            foreach (var t in trail)
                trailPath.Add(new PointD(t.x, t.z));

            var trailPoly = Clipper.Union(new PathsD { trailPath }, null, FillRule.NonZero, CP);
            if (trailPoly == null || trailPoly.Count == 0) return currentTerritory;

            var inflated = Clipper.InflatePaths(trailPoly, TrailInflateAmount, JoinType.Miter, EndType.Polygon, 2.0, CP);
            if (inflated != null && inflated.Count > 0) trailPoly = inflated;

            var merged = Clipper.Union(currentTerritory, trailPoly, FillRule.NonZero, CP);

            merged = RemoveHoles(merged);
            merged = MorphologicalClose(merged, MorphCloseAmount);

            return merged;
        }

        public PathsD SubtractTerritory(PathsD subject, PathsD clip)
        {
            if (subject == null || subject.Count == 0) return new PathsD();
            if (clip == null || clip.Count == 0) return subject;

            return Clipper.Difference(subject, clip, FillRule.NonZero, CP);
        }
        
        public double CalculateTotalArea(PathsD paths)
        {
            if (paths == null || paths.Count == 0) return 0;
            
            double area = 0;
            foreach (var path in paths)
            {
                area += Clipper.Area(path);
            }
            
            return System.Math.Abs(area); 
        }

        private PathsD MorphologicalClose(PathsD paths, double amount)
        {
            var dilated = Clipper.InflatePaths(paths, amount, JoinType.Miter, EndType.Polygon, 2.0, CP);
            if (dilated == null || dilated.Count == 0) return paths;

            dilated = Clipper.Union(dilated, null, FillRule.NonZero, CP);

            var eroded = Clipper.InflatePaths(dilated, -amount, JoinType.Miter, EndType.Polygon, 2.0, CP);
            if (eroded == null || eroded.Count == 0) return paths;

            eroded = Clipper.Union(eroded, null, FillRule.NonZero, CP);
            eroded = RemoveHoles(eroded);

            return eroded;
        }

        private PathsD RemoveHoles(PathsD paths)
        {
            if (paths == null || paths.Count == 0) return new PathsD();

            var outerOnly = new PathsD();
            foreach (var path in paths)
            {
                if (Clipper.Area(path) > 0)
                    outerOnly.Add(path);
            }
            return outerOnly.Count > 0 ? outerOnly : paths;
        }
    }
}