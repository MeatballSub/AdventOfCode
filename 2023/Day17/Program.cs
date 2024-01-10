using Library;
using System.IO;
using System;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

using VertexType = (Library.Geometry.Point p, int dir, int count);

bool isValidDir(Graph g, VertexType vertex, int new_dir, int min_count, int max_count)
{
    var inBounds = (Point p) => 0 <= p.Y && p.Y < g.height && 0 <= p.X && p.X < g.width;

    int count = 1 + ((new_dir == vertex.dir) ? vertex.count : 0);
    Point newLoc = Graph.moves[new_dir](vertex.p);

    bool reverse_dir = vertex.dir != 4 && (new_dir == (vertex.dir + 2) % 4);
    bool too_far_without_turn = (count > max_count);
    bool not_far_enough = (vertex.dir != new_dir && vertex.dir != 4 && vertex.count < min_count);
    bool out_of_bounds = !inBounds(newLoc);

    return !(reverse_dir || too_far_without_turn || not_far_enough || out_of_bounds);
}

List<VertexType> neighbors(VertexType vertex, Graph g, int min_count, int max_count)
{

    List<VertexType> result = new();

    for (int i = 0; i < 4; ++i)
    {
        int count = 1 + ((i == vertex.dir) ? vertex.count : 0);
        Point newLoc = Graph.moves[i](vertex.p);

        if(isValidDir(g, vertex, i, min_count, max_count))
        {
            result.Add((newLoc, i, count));
        }
    }

    return result;
}

void drawGrid(char[][]grid, List<VertexType> path)
{
    for (int y = 0; y < grid.Length; y++)
    {
        for (int x = 0; x < grid[y].Length; x++)
        {
            Console.BackgroundColor = path.Select(n => n.p).Contains(new Point(x, y)) ? ConsoleColor.Red : ConsoleColor.Black;
            Console.Write(grid[y][x]);
            Console.BackgroundColor = ConsoleColor.Black;
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

void showPath(string[] lines, VertexType goal, Graph g)
{
    char[] move_symbols = "^<v>".ToCharArray();
    char[][] grid = lines.Select(l => l.ToCharArray()).ToArray();
    List<VertexType> path = g.getPathTo(goal).Skip(1).ToList();
    drawGrid(grid, path);

    foreach (var vertex in g.getPathTo(goal))
    {
        if (vertex.dir < move_symbols.Length)
        {
            grid.at(vertex.p) = move_symbols[vertex.dir];
        }
    }

    drawGrid(grid, path);
}

long solve(string file_name, Graph.GoalTest goal_test, int min_count, int max_count)
{
    string[] lines = readFileLines(file_name);
    Graph g = new(lines);
    g.goalTest = v => v.p.Equals(new Point(g.width - 1, g.height - 1)) && goal_test(v);
    g.getNeighbors = v => neighbors(v, g, min_count, max_count);
    long answer = 0;
    try
    {
        Search(g, new VertexType(new Point(0, 0), 4, 0));
    }
    catch (GoalFoundException e)
    {
        showPath(lines, e.Value, g);
        answer = g.getBestCost(e.Value);
    }
    return answer;
}

void part1(string file_name)
{
    Graph.GoalTest goal_test = v => true;
    long answer = solve(file_name, goal_test, 0, 3);
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    Graph.GoalTest goal_test = v => v.count > 3;
    long answer = solve(file_name, goal_test, 4, 10);
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("sample2.txt");
part2("input.txt");

class GoalFoundException(VertexType value) : Exception
{
    public VertexType Value { get; init; } = value;
}

class Graph : BaseGraph<VertexType, long>
{
    public delegate bool GoalTest(VertexType location);
    public delegate List<VertexType> GetNeighbors(VertexType location);

    public GoalTest goalTest;
    public GetNeighbors getNeighbors;

    public static Dictionary<int, Move> moves = new()
    {
        { 0, p => p.Up() },
        { 1, p => p.Left() },
        { 2, p => p.Down() },
        { 3, p => p.Right() }
    };

    public static Dictionary<int, char> dirToChar = new()
    {
        { 0, 'N' },
        { 1, 'W' },
        { 2, 'S' },
        { 3, 'E' },
        { 4, ' ' },
    };

    public List<List<long>> blocks = new();
    public long height = -1;
    public long width = -1;

    public Graph(string[] lines)
    {
        blocks = lines.Select(l => l.Select(c => (long)c - '0').ToList()).ToList();
        height = blocks.Count;
        width = blocks[0].Count;
    }

    public override long cost(VertexType vertex1, VertexType vertex2) => blocks[(int)vertex2.p.Y][(int)vertex2.p.X];

    public override List<VertexType> neighbors(VertexType vertex) => getNeighbors(vertex);

    public void writeVertex(VertexType vertex)
    {
        string pathToArrive = new string(dirToChar[vertex.dir], vertex.count);
        pathToArrive = string.Join(',', pathToArrive.Select(c => $@"""{c}"""));
        Console.Write("{\"0\":" + vertex.p.Y + ",\"1\":" + vertex.p.X + ",\"weight\":" + cost(vertex, vertex) + ",\"pathToArrive\":[" + pathToArrive + "]}");
    }

    public override void visit(VertexType vertex)
    {
        //Console.Write("Visiting: ");
        //writeVertex(vertex);
        //Console.WriteLine();
        //Console.WriteLine("  Neighbors:");
        //foreach(var neighbor in neighbors(vertex))
        //{
        //    Console.Write("    ");
        //    writeVertex(neighbor);
        //    Console.WriteLine();
        //}
        if (goalTest(vertex))
        {
            throw new GoalFoundException(vertex);
        }
    }
};

static class Extensions
{
    public static ref char at(this char[][] arr, Point p)
    {
        return ref arr[p.Y][p.X];
    }
}

delegate Point Move(Point p);