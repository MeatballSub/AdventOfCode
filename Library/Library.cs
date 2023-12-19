using System.Numerics;
using System.Text.RegularExpressions;

namespace Library
{
    public static class Geometry
    {
        public interface IPoint3d<T> : IEquatable<T>
        {
            long X { get; }
            long Y { get; }
            long Z { get; }
            static abstract T createFrom<U>(IPoint3d<U> point);
        }

        public class Point3d : IPoint3d<Point3d>
        {
            public Point3d(long x, long y, long z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public Point3d(Point3d point) : this(point.X, point.Y, point.Z) { }

            public static implicit operator Point3d((long, long, long) p) => new Point3d(p.Item1, p.Item2, p.Item3);

            public long X { get; set; }
            public long Y { get; set; }
            public long Z { get; set; }

            public override string ToString() => "(" + X + ", " + Y + ", " + Z + ")";

            public static Point3d createFrom<T>(IPoint3d<T> point) => (point.X, point.Y, point.Z);

            public static Point3d operator +(Point3d a, Point3d b) => (a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            public static Point3d operator -(Point3d a, Point3d b) => (a.X - b.X, a.Y - b.Y, a.Z - b.Z);

            public bool Equals(Point3d? other) => !Object.ReferenceEquals(other, null) &&
                (
                    Object.ReferenceEquals(this, other) ||
                    (X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z))
                );

            public override int GetHashCode() => HashCode.Combine(X, Y, Z);
        }

        public class Point : IPoint3d<Point>
        {
            public long X { get; set; }
            public long Y { get; set; }
            public long Z { get => 0; set { } }

            public Point(long x, long y)
            {
                X = x;
                Y = y;
            }

            public Point(Point p) : this(p.X, p.Y) { }

            public static implicit operator Point((long, long) p) => new Point(p.Item1, p.Item2);

            public override string ToString() => "(" + X + ", " + Y + ")";

            public static Point createFrom<T>(IPoint3d<T> point) => (point.X, point.Y);

            public static Point operator +(Point a, Point b) => (a.X + b.X, a.Y + b.Y);
            public static Point operator -(Point a, Point b) => (a.X - b.X, a.Y - b.Y);

            public bool Equals(Point? other) => !Object.ReferenceEquals(other, null) &&
                (
                    Object.ReferenceEquals(this, other) ||
                    (X.Equals(other.X) && Y.Equals(other.Y))
                );

            public override int GetHashCode() => HashCode.Combine(X, Y);
        }

        public static long manhattanDistance<T>(IPoint3d<T> a, IPoint3d<T> b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);

        public static double euclideanDistance<T>(IPoint3d<T> a, IPoint3d<T> b)
        {
            Point3d delta = new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            return Math.Sqrt((delta.X * delta.X) + (delta.Y * delta.Y) + (delta.Z * delta.Z));
        }

        public static T Left<T>(this T point, long dist = 1) where T : IPoint3d<T> => T.createFrom(new Point3d(point.X - dist, point.Y, point.Z));
        public static T Right<T>(this T point, long dist = 1) where T : IPoint3d<T> => T.createFrom(new Point3d(point.X + dist, point.Y, point.Z));
        public static T Up<T>(this T point, long dist = 1) where T : IPoint3d<T> => T.createFrom(new Point3d(point.X, point.Y - dist, point.Z));
        public static T Down<T>(this T point, long dist = 1) where T : IPoint3d<T> => T.createFrom(new Point3d(point.X, point.Y + dist, point.Z));
        public static T Toward<T>(this T point, long dist = 1) where T : IPoint3d<T> => T.createFrom(new Point3d(point.X, point.Y, point.Z - dist));
        public static T Away<T>(this T point, long dist = 1) where T : IPoint3d<T> => T.createFrom(new Point3d(point.X, point.Y, point.Z + dist));

        public static IEnumerable<T> orthogonalNeighbors<T>(this T point) where T : IPoint3d<T> => allNeighbors(point).Where(_ => manhattanDistance(_, point) == 1);

        public static IEnumerable<T> diagonalNeighbors<T>(this T point) where T : IPoint3d<T> => allNeighbors(point).Where(_ => manhattanDistance(_, point) == 2);

        public static IEnumerable<T> superDiagonalNeighbors<T>(this T point) where T : IPoint3d<T> => allNeighbors(point).Where(_ => manhattanDistance(_, point) == 3);

        public static IEnumerable<T> allNeighbors<T>(this T point) where T : IPoint3d<T>
        {
            T source = T.createFrom(new Point3d(point.X, point.Y, point.Z));
            List<T> neighbors = new();
            foreach (long z in Enumerable.Range(-1, 3))
            {
                foreach (long y in Enumerable.Range(-1, 3))
                {
                    foreach (long x in Enumerable.Range(-1, 3))
                    {
                        neighbors.Add(T.createFrom(new Point3d(point.X + x, point.Y + y, point.Z + z)));
                    }
                }
            }
            return neighbors.Where(_ => !_.Equals(source)).Distinct();
        }
    }


    public static class Parsing
    {
        public static string readFile(string file_name) => File.ReadAllText(file_name);

        public static string[] SplitLines(this string str) => str.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        public static string[] readFileLines(string file_name) => File.ReadAllText(file_name).SplitLines();

        public static IEnumerable<long> ExtractLongs(this string str) => Regex.Matches(str, @"\d+").Select(_ => long.Parse(_.Value));
        public static IEnumerable<long> ExtractSignedLongs(this string str) => Regex.Matches(str, @"-?\d+").Select(_ => long.Parse(_.Value));
        public static IEnumerable<IEnumerable<long>> ExtractLongs(this IEnumerable<string> str) => str.Select(s => Regex.Matches(s, @"\d+").Select(_ => long.Parse(_.Value)));
        public static IEnumerable<IEnumerable<long>> ExtractSignedLongs(this IEnumerable<string> str) => str.Select(s => Regex.Matches(s, @"-?\d+").Select(_ => long.Parse(_.Value)));

        public static string get(this Match m, string named_capture) => m.Groups[named_capture].Value;
    }

    public static class LinqExtensions
    {
        public static bool IsOrdered<T>(this IEnumerable<T> enumerable, Func<T, T, bool> orderingFunc) => enumerable.Zip(enumerable.Skip(1), (a, b) => orderingFunc(a, b)).All(_ => _);
        public static bool IsOrderedAsc<T>(this IEnumerable<T> enumerable) where T : IComparable<T> =>
            IsOrdered(enumerable, (a, b) => a.CompareTo(b) <= 0);

        public static bool IsOrderedDesc<T>(this IEnumerable<T> enumerable) where T : IComparable<T> =>
            IsOrdered(enumerable, (a, b) => a.CompareTo(b) >= 0);

        public static bool IsOrderedStrictAsc<T>(this IEnumerable<T> enumerable) where T : IComparable<T> =>
            IsOrdered(enumerable, (a, b) => a.CompareTo(b) < 0);

        public static bool IsOrderedStrictDesc<T>(this IEnumerable<T> enumerable) where T : IComparable<T> =>
            IsOrdered(enumerable, (a, b) => a.CompareTo(b) > 0);

        public static T Product<T>(this IEnumerable<T> enumerable) where T : IMultiplyOperators<T, T, T>, IMultiplicativeIdentity<T, T> =>
            enumerable.Aggregate(T.MultiplicativeIdentity, (acc, cur) => acc * cur);
    }

    public static class MathStuff
    {
        public static long GCD(long a, long b)
        {
            while (b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }

            return a;
        }

        public static long LCM(long a, long b) => a / GCD(a, b) * b;

        public static long LCM(this IEnumerable<long> values) => values.Aggregate(LCM);

        public static long GetBinCoeff(long n, long k)
        {
            long r = 1;
            if (k > n) return 0;
            for (long d = 1; d <= k; d++)
            {
                r *= n--;
                r /= d;
            }
            return r;
        }
    }

    public static class Optimize
    {
        public interface IGraph<VertexType, CostType> where VertexType : IEquatable<VertexType> where CostType : IComparable<CostType>, IAdditiveIdentity<CostType, CostType>, IAdditionOperators<CostType, CostType, CostType>
        {
            List<VertexType> neighbors(VertexType vertex);
            CostType cost(VertexType vertex1, VertexType vertex2);
            CostType heuristic(VertexType vertex);
            void visit(VertexType vertex);
            void setPredecessor(VertexType vertex, VertexType predecessor);
            void setBestCost(VertexType vertex, CostType bestCost);
            CostType? getBestCost(VertexType vertex);
            bool containsBestCostFor(VertexType vertex);
        }

        public abstract class BaseGraph<VertexType, CostType> : IGraph<VertexType, CostType> where VertexType : IEquatable<VertexType> where CostType : IComparable<CostType>, IAdditiveIdentity<CostType, CostType>, IAdditionOperators<CostType, CostType, CostType>
        {
            protected Dictionary<VertexType, CostType?> best_costs = new();
            protected Dictionary<VertexType, VertexType?> predecessors = new();

            public abstract CostType cost(VertexType vertex1, VertexType vertex2);
            public abstract List<VertexType> neighbors(VertexType vertex);
            public virtual void visit(VertexType vertex) { }

            public virtual CostType heuristic(VertexType vertex) => CostType.AdditiveIdentity;

            public virtual CostType? getBestCost(VertexType vertex) => best_costs.GetValueOrDefault(vertex, default);

            public virtual void setBestCost(VertexType vertex, CostType bestCost) => best_costs[vertex] = bestCost;
            public virtual bool containsBestCostFor(VertexType vertex) => best_costs.ContainsKey(vertex);
            public virtual void setPredecessor(VertexType vertex, VertexType predecessor) => predecessors[vertex] = predecessor;
        }

        public static bool isBetterThan<T>(this T value, T other) where T : IComparable<T>
        {
            return value.CompareTo(other) < 0;
        }

        public static void Search<VertexType, CostType>(IGraph<VertexType, CostType> graph, VertexType start_vertex) where VertexType : IEquatable<VertexType> where CostType : IComparable<CostType>, IAdditiveIdentity<CostType, CostType>, IAdditionOperators<CostType, CostType, CostType>
        {            
            HashSet<VertexType> visited = new();
            PriorityQueue<VertexType, CostType> frontier = new();

            CostType start_cost = CostType.AdditiveIdentity;
            CostType start_potential = start_cost + graph.heuristic(start_vertex);

            graph.setBestCost(start_vertex, start_cost);
            frontier.Enqueue(start_vertex, start_potential);

            while(frontier.TryDequeue(out VertexType? curr_vertex, out var _))
            {
                if(visited.Contains(curr_vertex))
                {
                    continue;
                }

                graph.visit(curr_vertex);
                visited.Add(curr_vertex);                

                foreach(VertexType neighbor_vertex in graph.neighbors(curr_vertex))
                {
                    CostType neighbor_cost = graph.getBestCost(curr_vertex) + graph.cost(curr_vertex, neighbor_vertex);
                    CostType neighbor_potential = neighbor_cost + graph.heuristic(neighbor_vertex);

                    if (!graph.containsBestCostFor(neighbor_vertex) || neighbor_cost.isBetterThan(graph.getBestCost(neighbor_vertex)))
                    {
                        graph.setPredecessor(neighbor_vertex, curr_vertex);
                        graph.setBestCost(neighbor_vertex, neighbor_cost);
                        frontier.Enqueue(neighbor_vertex, neighbor_potential);
                    }
                }
            }
        }
    }
}