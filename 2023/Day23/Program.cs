using Library;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

using SpaghettiStackNode = (Library.Geometry.Point loc, int index, int parent_index);

long start_time = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
long timeSinceStart() => (Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond) - start_time;

char[][] parse(string file_name)
{
    string[] lines = readFileLines(file_name);
    return lines.Select(l => l.ToCharArray()).ToArray();
}

Point? findEndpoint(char[][] grid, int row)
{
    for (int i = 0; i < grid[row].Length; ++i)
    {
        if (grid[row][i] == '.')
        {
            return new Point(i, row);
        }
    }
    return null;
}

Point? findStart(char[][] grid) => findEndpoint(grid, 0);

Point? findEnd(char[][] grid) => findEndpoint(grid, grid.Length - 1);

bool inBounds(Point p, char[][] grid) => 0 <= p.Y && p.Y < grid.Length && 0 <= p.X && p.X < grid[p.Y].Length;

IEnumerable<Point> part1Neighbors(char[][] grid, Point p)
{
    Dictionary<char, List<Point>> possibilities = new()
    {
        { '<', new List<Point>() {p.Left()} },
        { '>', new List<Point>() {p.Right()} },
        { '^', new List<Point>() {p.Up()} },
        { 'v', new List<Point>() {p.Down()} },
        { '.', new List<Point>() {p.Left(), p.Right(), p.Up(), p.Down()} },
    };

    return possibilities[grid.at(p)].Where(p => inBounds(p, grid) && grid.at(p) != '#');
}

IEnumerable<Point> part2Neighbors(char[][] grid, Point p)
{
    return new List<Point>() { p.Left(), p.Right(), p.Up(), p.Down() }.Where(p => inBounds(p, grid) && grid.at(p) != '#');
}


long solve(string file_name, GetNeighbors getNeighbors)
{
    char[][] grid = parse(file_name);
    Point? start = findStart(grid);
    Point? end = findEnd(grid);
    SpaghettiStack spaghetti_stack = new();
    SpaghettiStackNode start_node = spaghetti_stack.Add(start);
    Queue<SpaghettiStackNode> frontier = new();
    List<long> step_counts = new();

    frontier.Enqueue(start_node);
    while (frontier.Count > 0)
    {
        SpaghettiStackNode curr = frontier.Dequeue();
        if (curr.loc.Equals(end))
        {
            step_counts.Add(spaghetti_stack.CountSteps(curr));
        }
        else
        {
            foreach (Point p in getNeighbors(grid, curr.loc))
            {
                if (!spaghetti_stack.Visited(curr, p))
                {
                    frontier.Enqueue(spaghetti_stack.Add(p, curr));
                }
            }
        }
    }
    return step_counts.Max();
}

void part1(string file_name)
{
    long answer = solve(file_name, part1Neighbors);
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
    Console.WriteLine($"{timeSinceStart()} ms");
}

void part2(string file_name)
{
    long answer = solve(file_name, part2Neighbors);
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
    Console.WriteLine($"{timeSinceStart()} ms");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");

class SpaghettiStack
{

    List<SpaghettiStackNode> stack = new();

    public SpaghettiStack() { }

    public SpaghettiStackNode Add(Point loc)
    {
        SpaghettiStackNode new_node = new SpaghettiStackNode(loc, stack.Count, -1);
        stack.Add(new_node);
        return new_node;
    }

    public SpaghettiStackNode Add(Point loc, SpaghettiStackNode parent)
    {
        SpaghettiStackNode new_node = new SpaghettiStackNode(loc, stack.Count, parent.index);
        stack.Add(new_node);
        return new_node;
    }

    public HashSet<Point> Visited(SpaghettiStackNode node)
    {
        HashSet<Point> visited = new();
        for (int index = node.index; index != -1; index = stack[index].parent_index)
        {
            visited.Add(stack[index].loc);
        }

        return visited;
    }

    public bool Visited(SpaghettiStackNode node, Point loc)
    {
        for (int index = node.index; index != -1; index = stack[index].parent_index)
        {
            if (stack[index].loc.Equals(loc)) return true;
        }

        return false;
    }

    public long CountSteps(SpaghettiStackNode node)
    {
        long steps = -1;  // offset start location
        for(int index = node.index; index != -1; index = stack[index].parent_index)
        {
            ++steps;
        }
        return steps;
    }
}

static class Extensions
{
    public static ref char at(this char[][] arr, Point p)
    {
        return ref arr[p.Y][p.X];
    }
}

delegate IEnumerable<Point> GetNeighbors(char[][] grid, Point p);