using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;
using System.Drawing;

namespace Greedy
{
    public class My_DijkstraPathFinder
    {
        public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,
            IEnumerable<Point> targets)
        {
            var notVisited = GetMazePoints(state).Where(p => !state.IsWallAt(p)).ToList();
            var track = new Dictionary<Point, DijkstraData>();
            track[start] = new DijkstraData { Price = 0, Previous = new Point(-1, -1) };
            while (true)
            {
                Point toOpen = GetPointToOpen(notVisited, track);
                if (toOpen.X == -1)
                    yield break;
                if (targets.Contains(toOpen))
                    yield return GetPathFromTrack(track, toOpen);
                WorkOnOpen(toOpen, state, track);
                notVisited.Remove(toOpen);
            }
        }

        private IEnumerable<Point> GetMazePoints(State state)
        {
            for (int x = 0; x < state.MapWidth; x++)
                for (int y = 0; y < state.MapHeight; y++)
                    yield return new Point(x, y);
        }

        private Point GetPointToOpen(List<Point> notVisitedPoints, Dictionary<Point, DijkstraData> track)
        {
            Point toOpen = new Point(-1, -1);
            int bestPrice = int.MaxValue;
            foreach (var p in notVisitedPoints)
            {
                if (track.ContainsKey(p) && track[p].Price < bestPrice)
                {
                    bestPrice = track[p].Price;
                    toOpen = p;
                }
            }
            return toOpen;
        }

        private PathWithCost GetPathFromTrack(Dictionary<Point, DijkstraData> track, Point target)
        {
            var result = new List<Point>();
            int cost = track[target].Price;
            while (target.X != -1)
            {
                result.Add(target);
                target = track[target].Previous;
            }
            result.Reverse();
            return new PathWithCost(cost, result.ToArray());
        }

        private void WorkOnOpen(Point toOpen, State state, Dictionary<Point, DijkstraData> track)
        {
            foreach (var p in GetNextPoints(toOpen, state))
            {
                int currentPrice = state.CellCost[p.X, p.Y] + track[toOpen].Price;
                if (!track.ContainsKey(p) || track[p].Price > currentPrice)
                    track[p] = new DijkstraData { Previous = toOpen, Price = currentPrice };
            }
        }

        public IEnumerable<Point> GetNextPoints(Point p, State state)
        {
            int[] d = { -1, 0, 1 };
            return (from x in d
                    from y in d
                    where Math.Abs(x) != Math.Abs(y)
                    select new Point(p.X + x, p.Y + y))
                   .Where(point => state.InsideMap(point) && !state.IsWallAt(point));
        }
    }

    class DijkstraData
    {
        public Point Previous { get; set; }
        public int Price { get; set; }
    }
}
