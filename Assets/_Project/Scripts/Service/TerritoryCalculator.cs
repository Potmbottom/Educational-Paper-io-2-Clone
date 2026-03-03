using System.Collections.Generic;
using Clipper2Lib;
using UnityEngine;

namespace PaperClone.Service
{
    public class TerritoryCalculator
    {
        // ── Tuning constants ──────────────────────────────────
        private const int    CP                 = 4;   // 10^4 internal scaling (was defaulting to 2!)
        private const double TrailInflateAmount  = 0.2; // overlap guarantee
        private const double MorphCloseAmount    = 0.3; // gap-filling radius
        // ──────────────────────────────────────────────────────

        public bool IsPointInTerritory(PathsD territory, Vector3 point)
        {
            var p = new PointD(point.x, point.z);
            foreach (var path in territory)
            {
                if (Clipper.PointInPolygon(p, path) != PointInPolygonResult.IsOutside)
                    return true;
            }
            return false;
        }

        public PathsD CalculateExpansion(PathsD currentTerritory, List<Vector3> trail)
        {
            if (trail.Count < 3) return currentTerritory;

            var trailPath = new PathD(trail.Count);
            foreach (var t in trail)
                trailPath.Add(new PointD(t.x, t.z));

            // 1. Resolve trail self-intersections  (precision = 4 now!)
            var trailPoly = Clipper.Union(
                new PathsD { trailPath }, null, FillRule.NonZero, CP);
            if (trailPoly == null || trailPoly.Count == 0)
                return currentTerritory;

            // 2. Inflate trail to guarantee overlap with existing territory
            var inflated = Clipper.InflatePaths(
                trailPoly, TrailInflateAmount,
                JoinType.Miter, EndType.Polygon, 2.0, CP);
            if (inflated != null && inflated.Count > 0)
                trailPoly = inflated;

            // 3. Merge with territory  (precision = 4)
            var merged = Clipper.Union(
                currentTerritory, trailPoly, FillRule.NonZero, CP);

            // 4. Discard holes (Paper.io fills enclosed areas)
            merged = RemoveHoles(merged);

            // 5. Morphological close — seals every remaining micro-gap
            merged = MorphologicalClose(merged, MorphCloseAmount);

            return merged;
        }

        // ── Inflate → Union (merge overlaps) → Deflate → Union → RemoveHoles ──
        private PathsD MorphologicalClose(PathsD paths, double amount)
        {
            // Dilate outward
            var dilated = Clipper.InflatePaths(
                paths, amount,
                JoinType.Miter, EndType.Polygon, 2.0, CP);
            if (dilated == null || dilated.Count == 0) return paths;

            // Merge any overlapping regions the dilation created
            dilated = Clipper.Union(dilated, null, FillRule.NonZero, CP);

            // Erode back to original size
            var eroded = Clipper.InflatePaths(
                dilated, -amount,
                JoinType.Miter, EndType.Polygon, 2.0, CP);
            if (eroded == null || eroded.Count == 0) return paths;

            // Final cleanup
            eroded = Clipper.Union(eroded, null, FillRule.NonZero, CP);
            eroded = RemoveHoles(eroded);

            return eroded;
        }

        private PathsD RemoveHoles(PathsD paths)
        {
            if (paths == null || paths.Count == 0)
                return new PathsD();

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