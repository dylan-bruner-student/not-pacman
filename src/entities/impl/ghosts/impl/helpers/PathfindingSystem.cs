﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1.src.entities.impl.ghosts.impl.helpers
{
    public class PathfindingSystem
    {
        // Try to get the shortest path using A* algorithm
        public static List<ResultPoint>? TryGetPath(List<List<TileState>> map, Vector2 start, Vector2 end)
        {
            if (map == null || map.Count == 0 || start.Equals(end)) return null;

            var openList = new List<Node>(); // The list of nodes to be evaluated
            var closedList = new HashSet<Vector2>(); // The list of nodes already evaluated
            var cameFrom = new Dictionary<Vector2, Vector2>(); // Stores the optimal path
            var gScore = new Dictionary<Vector2, float>(); // Cost from start to current
            var fScore = new Dictionary<Vector2, float>(); // Estimated total cost (g + heuristic)

            // Initialize the start node
            openList.Add(new Node(start, 0, Heuristic(start, end)));
            gScore[start] = 0;
            fScore[start] = Heuristic(start, end);

            while (openList.Any())
            {
                // Get the node with the lowest fScore
                var current = openList.OrderBy(n => fScore.GetValueOrDefault(n.Position, float.MaxValue)).First();
                openList.Remove(current);

                // If we've reached the end, reconstruct the path
                if (current.Position.Equals(end))
                {
                    return ReconstructPath(cameFrom, current.Position);
                }

                closedList.Add(current.Position);

                foreach (var neighbor in GetNeighbors(current.Position, map))
                {
                    if (closedList.Contains(neighbor) || map[(int)neighbor.Y][(int)neighbor.X] == TileState.BLOCKED)
                        continue;

                    var tentativeGScore = gScore.GetValueOrDefault(current.Position, float.MaxValue) + 1;

                    if (!openList.Any(n => n.Position.Equals(neighbor)) || tentativeGScore < gScore.GetValueOrDefault(neighbor, float.MaxValue))
                    {
                        cameFrom[neighbor] = current.Position;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + Heuristic(neighbor, end);

                        if (!openList.Any(n => n.Position.Equals(neighbor)))
                            openList.Add(new Node(neighbor, gScore[neighbor], fScore[neighbor]));
                    }
                }
            }

            return null; // No path found
        }

        // Heuristic: Using Manhattan distance
        private static float Heuristic(Vector2 a, Vector2 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        // Reconstruct the path by tracing the 'cameFrom' dictionary
        private static List<ResultPoint> ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
        {
            var path = new List<ResultPoint>();

            while (cameFrom.ContainsKey(current))
            {
                var previous = cameFrom[current];
                var direction = GetDirection(previous, current);
                path.Insert(0, direction);
                current = previous;
            }

            return path;
        }

        // Convert a pair of coordinates to a direction (UP, DOWN, LEFT, RIGHT)
        private static ResultPoint GetDirection(Vector2 from, Vector2 to)
        {
            if (to.Y < from.Y) return ResultPoint.UP;
            if (to.Y > from.Y) return ResultPoint.DOWN;
            if (to.X < from.X) return ResultPoint.LEFT;
            return ResultPoint.RIGHT;
        }

        // Get the valid neighbors for a given position
        private static List<Vector2> GetNeighbors(Vector2 position, List<List<TileState>> map)
        {
            var neighbors = new List<Vector2>();

            var directions = new Vector2[]
            {
                new Vector2(0, -1), // UP
                new Vector2(0, 1),  // DOWN
                new Vector2(-1, 0), // LEFT
                new Vector2(1, 0)   // RIGHT
            };

            foreach (var direction in directions)
            {
                var newPosition = position + direction;
                if (newPosition.X >= 0 && newPosition.Y >= 0 && newPosition.X < map[0].Count && newPosition.Y < map.Count)
                {
                    neighbors.Add(newPosition);
                }
            }

            return neighbors;
        }

        public enum TileState { BLOCKED, OPEN }
        public enum ResultPoint { UP, DOWN, LEFT, RIGHT }

        // Node class to keep track of position and costs
        private class Node
        {
            public Vector2 Position { get; }
            public float GScore { get; }
            public float FScore { get; }

            public Node(Vector2 position, float gScore, float fScore)
            {
                Position = position;
                GScore = gScore;
                FScore = fScore;
            }
        }

        // Vector2 class for positions (simplified version)
        public struct Vector2
        {
            public float X { get; set; }
            public float Y { get; set; }

            public Vector2(float x, float y)
            {
                X = x;
                Y = y;
            }

            public static Vector2 operator +(Vector2 v1, Vector2 v2)
            {
                return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
            }

            public bool Equals(Vector2 other)
            {
                return X == other.X && Y == other.Y;
            }

            // Override GetHashCode and Equals for proper usage in collections
            public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
            public override bool Equals(object obj) => obj is Vector2 other && Equals(other);
        }
    }
}
