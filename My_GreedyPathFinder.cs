using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;
using NUnit.Framework.Constraints;

namespace Greedy
{
    public class My_GreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State state)
        {
            var result = new List<Point>();
            if (state.Goal > state.Chests.Count)
                return result;
            var dijkstraPath = new DijkstraPathFinder();
            var energy = 0;
            var target = new HashSet<Point>(state.Chests);
            var position = state.Position;

            for (var i = 0; i < state.Goal; i++)
            {
                var pathWithCost = dijkstraPath.GetPathsByDijkstra(state, position, target).FirstOrDefault();
                energy += pathWithCost.Cost;
                if (state.Energy < pathWithCost.Cost || pathWithCost == default(PathWithCost))
                    break;
                var path = pathWithCost.Path;
                position = path.Last();
                target.Remove(position);
                result.AddRange(path);
            }

            return result;

        }
    }
}