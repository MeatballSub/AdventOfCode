using Library;
using System;
using System.Diagnostics;
using System.Text;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

long start_time = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
long timeSinceStart () => (Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond) - start_time;

char[][] parse(string file_name)
{
string[] lines = readFileLines(file_name);
    return lines.Select(l => l.ToCharArray()).ToArray();
}

Point findEndpoint(char[][] grid, int row)
{
    for (int i = 0; i < grid[row].Length; ++i)
    {
        if (grid[row][i] == '.')
        {
            return new Point(i, row);
        }
    }
    throw new IndexOutOfRangeException();
}

Point findStart (char[][] grid) => findEndpoint(grid, 0);

Point findEnd (char[][] grid) => findEndpoint(grid, grid.Length - 1);

var grid = parse("input.txt");
Point start = findStart(grid);
Point end = findEnd(grid);

Graph g = new(grid, start, end);

long max_step_count = 0;
Stack<Point> visited = new();
Stack<(Point, Point?, long count)> frontier = new();
frontier.Push((g.Start, null, 0));
while(frontier.Count > 0)
{
    (Point curr, Point parent, long count) = frontier.Pop();
    while((parent != null) && !parent.Equals(visited.Peek()))
    {
        visited.Pop();
    }
    if(curr.Equals(g.End))
    {
        if(count > max_step_count)
        {
            max_step_count = count;
            Console.WriteLine($"{max_step_count} - {timeSinceStart()} ms");
        }
    }
    else
    {
        visited.Push(curr);
        foreach (Point p in g.Neighbors(curr))
        {
            if (!visited.Contains(p))
            {
                frontier.Push((p, curr, count + g.Cost(p, curr)));
            }
        }
    }
}

Console.WriteLine($"max_step_count = {max_step_count} - {timeSinceStart()} ms");


class Graph
{
    public HashSet<Point> vertices;
    public Dictionary<Point, Dictionary<Point, long>> edges = new();
    public Point Start;
    public Point End;
    public Graph(char[][] grid, Point start, Point end)
    {
        Start = start;
        End = end;
        vertices = new() { start, end };
        HashSet<Point> visited = new();
        Stack<Point> frontier = new();
        frontier.Push(start);
        while( frontier.Count > 0 )
        {
            Point p = frontier.Pop();
            if (visited.Contains(p)) continue;
            visited.Add(p);
            var neighbors = p.orthogonalNeighbors().Where(n => grid.inBounds(n) && grid.at(n) != '#');
            if(neighbors.Count() > 2)
            {
                vertices.Add(p);
            }
            neighbors.ToList().ForEach(n => frontier.Push(n));
        }

        foreach (var vertex in vertices)
        {
            foreach(var v_neighbor in vertex.orthogonalNeighbors().Where(n => grid.inBounds(n) && grid.at(n) != '#'))
            {
                visited.Clear();
                visited.Add(vertex);
                long steps = 0;
                frontier.Push(v_neighbor);
                while (frontier.Count > 0)
                {
                    Point p = frontier.Pop();
                    if (visited.Contains(p)) continue;
                    visited.Add(p);
                    ++steps;
                    var neighbors = p.orthogonalNeighbors().Where(n => grid.inBounds(n) && grid.at(n) != '#');
                    if (vertices.Contains(p))
                    {
                        if (!edges.ContainsKey(vertex))
                        {
                            edges[vertex] = new();
                        }
                        edges[vertex][p] = steps;
                    }
                    else
                    {
                        neighbors.Where(n => !visited.Contains(n)).ToList().ForEach(n => frontier.Push(n));
                    }
                }
            }
        }
    }

    public List<Point> Neighbors(Point p) => edges[p].Select(e => e.Key).ToList();

    public long Cost(Point a, Point b) => edges[a][b];
}

static class Extensions
{
    public static ref char at(this char[][] arr, Point p) => ref arr[p.Y][p.X];

    public static bool inBounds(this char[][] arr, Point p) => 0 <= p.Y && p.Y < arr.Length && 0 <= p.X && p.X < arr[p.Y].Length;
}