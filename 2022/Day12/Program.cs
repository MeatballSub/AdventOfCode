using Library;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

void run(string file_name)
{
    Graph part1_graph = new(file_name);
    Graph part2_graph = new(file_name);

    Graph.ValidateNeighbor canClimb = (curr, next) =>
    {
        return (part1_graph.map[next.Y][(int)next.X] - part1_graph.map[curr.Y][(int)curr.X] < 2);
    };

    Graph.ValidateNeighbor canDescend = (next, curr) =>
    {
        return (part2_graph.map[next.Y][(int)next.X] - part2_graph.map[curr.Y][(int)curr.X] < 2);
    };

    part1_graph.goalTest = v => v.Equals(part1_graph.End);
    part1_graph.validateNeighbor = canClimb;
    part2_graph.goalTest = v => part2_graph.map[v.Y][(int)v.X] == 'a';
    part2_graph.validateNeighbor = canDescend;

    try
    {
        Search(part1_graph, part1_graph.Start);
    }
    catch (GoalFoundException ex)
    {
        Console.WriteLine($"Part1: {part1_graph.getBestCost(ex.Value)}");
    }

    try
    {
        Search(part2_graph, part2_graph.End);
    }
    catch (GoalFoundException ex)
    {
        Console.WriteLine($"Part2: {part2_graph.getBestCost(ex.Value)}");
    }
}

run("sample.txt");
run("input.txt");

class GoalFoundException(Point value) : Exception
{
    public Point Value { get; init; } = value;
}

class Graph : BaseGraph<Point, long>
{
    public delegate bool GoalTest(Point location);
    public delegate bool ValidateNeighbor(Point curr, Point neighbor);

    public Point Start { get; set; }
    public Point End { get; set; }
    public string[] map;
    int width;
    int height;
    public GoalTest goalTest;
    public ValidateNeighbor validateNeighbor;

    public Graph(string file_name)
    {
        map = readFileLines(file_name);
        height = map.Length;
        width = map[0].Length;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (map[y][x] == 'S')
                {
                    Start = new Point(x, y);
                    map[y] = map[y].Replace('S', 'a');
                }
                else if (map[y][x] == 'E')
                {
                    End = new Point(x, y);
                    map[y] = map[y].Replace('E', 'z');
                }
            }
        }
    }

    private bool inBounds(Point vertex)
    {
        return new List<long> { 0, vertex.X, width - 1 }.IsOrderedAsc() && new List<long> { 0, vertex.Y, height - 1 }.IsOrderedAsc();
    }

    public override long cost(Point vertex1, Point vertex2)
    {
        return manhattanDistance(vertex1, vertex2);
    }

    public override List<Point> neighbors(Point vertex)
    {
        return vertex.orthogonalNeighbors().Where(n => inBounds(n) && validateNeighbor(vertex, n)).ToList();
    }

    public override void visit(Point vertex)
    {
        if (goalTest(vertex))
        {
            throw new GoalFoundException(vertex);
        }
    }
}