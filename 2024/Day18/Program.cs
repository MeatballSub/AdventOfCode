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
    Graph g = new(input.Take(1024).ToHashSet());
    Search(g, START);
    List<Point> best_path = g.getPathTo(GOAL);

    foreach(var new_point in input.Skip(1024))
    {
        g.AddCorruption(new_point);

        if (best_path.Contains(new_point))
        {
            Search(g, START);

            if (0 == g.getBestCost(GOAL))
            {
                return new_point;
            }

            best_path = g.getPathTo(GOAL);
        }
    }

    throw new Exception("No blocking point found!");
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

    public void AddCorruption(Point p)
    {
        corruption.Add(p);
        best_costs.Clear();
        predecessors.Clear();
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