using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;
using static Library.Testing;

(char[][] map, Point start, Point end) parse(string file_name)
{
    var map = readFileAsGrid(file_name);
    Point? start = null;
    Point? end = null;

    for(int y = 0; y < map.Length; y++)
    {
        for(int x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == 'S')
            {
                start = new Point(x, y);
            }
            if (map[y][x] == 'E')
            {
                end = new Point(x, y);
            }
            if (start != null && end != null) return (map, start, end);
        }
    }
    throw new Exception("Couldn't find start and end");
}

long solve((string file_name, long max_dist, long savings_needed) args)
{
    var solution = 0L;
    var (map, start, end) = parse(args.file_name);

    Graph g = new(map);

    Search(g, end);

    for (int y = 0; y < map.Length; ++y)
    {
        for (int x = 0; x < map[y].Length; ++x)
        {
            var point1 = new Point(x, y);
            if (map.at(point1) != '#')
            {
                var best1 = g.getBestCost(point1);
                for (int y2 = y; y2 < map.Length; ++y2)
                {
                    for (int x2 = (y2 == y) ? x : 0; x2 < map[y2].Length; ++x2)
                    {
                        var point2 = new Point(x2, y2);
                        if (map.at(point2) != '#')
                        {
                            var dist = manhattanDistance(point1, point2);
                            if (dist <= args.max_dist)
                            {
                                var best2 = g.getBestCost(point2);
                                if (Math.Abs(best1 - best2) - dist >= args.savings_needed)
                                {
                                    ++solution;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    return solution;
}

test(solve, "part1", ("sample.txt", 2L, 2L), 44);
test(solve, "part1", ("sample.txt", 2L, 4L), 30);
test(solve, "part1", ("sample.txt", 2L, 6L), 16);
test(solve, "part1", ("sample.txt", 2L, 8L), 14);
test(solve, "part1", ("sample.txt", 2L, 10L), 10);
test(solve, "part1", ("sample.txt", 2L, 12L), 8);
test(solve, "part1", ("sample.txt", 2L, 20L), 5);
test(solve, "part1", ("sample.txt", 2L, 36L), 4);
test(solve, "part1", ("sample.txt", 2L, 38L), 3);
test(solve, "part1", ("sample.txt", 2L, 40L), 2);
test(solve, "part1", ("sample.txt", 2L, 64L), 1);

test(solve, "part1", ("input.txt", 2L, 100L), 1346);

test(solve, "part2", ("sample.txt",20L,50L), 285);
test(solve, "part2", ("sample.txt", 20L, 52L), 253);
test(solve, "part2", ("sample.txt", 20L, 54L), 222);
test(solve, "part2", ("sample.txt", 20L, 56L), 193);
test(solve, "part2", ("sample.txt", 20L, 58L), 154);
test(solve, "part2", ("sample.txt", 20L, 60L), 129);
test(solve, "part2", ("sample.txt", 20L, 62L), 106);
test(solve, "part2", ("sample.txt", 20L, 64L), 86);
test(solve, "part2", ("sample.txt", 20L, 66L), 67);
test(solve, "part2", ("sample.txt", 20L, 68L), 55);
test(solve, "part2", ("sample.txt", 20L, 70L), 41);
test(solve, "part2", ("sample.txt", 20L, 72L), 29);
test(solve, "part2", ("sample.txt", 20L, 74L), 7);
test(solve, "part2", ("sample.txt", 20L, 76L), 3);

test(solve, "part2", ("input.txt", 20L, 100L), 985482);

class Graph : BaseGraph<Point, long>
{
    private char[][] map;
    public Graph(char[][] map)
    {
        this.map = map;
    }

    public override long cost(Point vertex1, Point vertex2)
    {
        return 1;
    }

    public override List<Point> neighbors(Point vertex)
    {
        return vertex.orthogonalNeighbors().Where(n => n.boundsCheck(map) && map.at(n) != '#').ToList();
    }
}