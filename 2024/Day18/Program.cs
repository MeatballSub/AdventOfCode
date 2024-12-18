using static Library.Geometry;
using static Library.Parsing;
using static Library.Testing;
using static Library.Optimize;

Point START = new Point(0, 0);
Point GOAL = new Point(70, 70);

Point lineToPoint(string line)
{
    var coords = line.ExtractLongs();
    return new Point(coords.First(), coords.Last());
}

long part1(string file_name)
{
    var input = readFileLines(file_name).Select(lineToPoint).Take(1024);
    Graph g = new(input.ToHashSet());
    Search(g, START);
    return g.getBestCost(GOAL);
}

Point part2(string file_name)
{
    var input = readFileLines(file_name).Select(lineToPoint);

    int min_bound = 1025;
    int max_bound = input.Count();
    int mid_point = (min_bound + max_bound) / 2;

    while (min_bound != mid_point)
    {
        Graph g = new(input.Take(mid_point).ToHashSet());
        Search(g, START);
        if (0 == g.getBestCost(GOAL))
        {
            max_bound = mid_point;
        }
        else
        {
            min_bound = mid_point;
        }
        mid_point = (min_bound + max_bound) / 2;
    }
    return input.Take(max_bound).Last();
}

test(part1, "part1", "input.txt", 356);
test(part2, "part2", "input.txt", new Point(22,33));

class Graph : BaseGraph<Point, long>
{
    HashSet<Point> corruption;
    public Graph(HashSet<Point> corruption)
    {
        this.corruption = new(corruption);
    }

    public override long cost(Point vertex1, Point vertex2)
    {
        return 1;
    }

    public override List<Point> neighbors(Point vertex)
    {
        return vertex.orthogonalNeighbors().Where(n => inBounds(n) && !corrupted(n)).ToList();
    }

    private bool inBounds(Point p)
    {
        return 0 <= p.X && p.X < 71 && 0 <= p.Y && p.Y < 71;
    }

    private bool corrupted(Point p)
    {
        return corruption.Contains(p);
    }
}