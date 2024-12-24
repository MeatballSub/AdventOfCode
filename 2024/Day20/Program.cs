using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;
using static Library.Testing;
using static Library.GraphExtensions;

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
    Graph g2 = new(map);

    g.Search(end);
    g2.Search(start);

    int width = map[0].Length;
    int height = map.Length;
    var map_bitmap = new System.Drawing.Bitmap(width, height);
    for (int y = 0; y < map.Length; ++y)
    {
        for (int x = 0; x < map[y].Length; ++x)
        {
            if (map[y][x] == '#')
            {
                map_bitmap.SetPixel(x, y, System.Drawing.Color.Gray);
            }
            if (map[y][x] == 'S')
            {
                map_bitmap.SetPixel(x, y, System.Drawing.Color.Green);
            }
            if (map[y][x] == 'E')
            {
                map_bitmap.SetPixel(x, y, System.Drawing.Color.Red);
            }
        }
    }

    Directory.CreateDirectory("images");

    for (int y = 0; y < map.Length; ++y)
    {
        for (int x = 0; x < map[y].Length; ++x)
        {
            var point1 = new Point(x, y);
            if (map.at(point1) != '#')
            {
                var best1 = g.getBestCost(point1);
                for (int y2 = y; y2 < Math.Min(map.Length, y + args.max_dist + 1); ++y2)
                {
                    for (int x2 = (y2 == y) ? x : (int)Math.Max(0, x - args.max_dist - 1); x2 < Math.Min(map[y2].Length, x + args.max_dist + 1); ++x2)
                    {
                        var point2 = new Point(x2, y2);
                        if (map.at(point2) != '#')
                        {
                            var best2 = g.getBestCost(point2);
                            var dist = manhattanDistance(point1, point2);
                            if (dist <= args.max_dist)
                            {
                                if (Math.Abs(best1 - best2) - dist >= args.savings_needed)
                                {
                                    long step = 0;
                                    var bitmap = new System.Drawing.Bitmap(map_bitmap);
                                    var start_path = ((best1 > best2) ? g2.getPathTo(point1) : g2.getPathTo(point2)).Skip(1).SkipLast(1);
                                    var end_path = ((best1 > best2) ? g.getPathTo(point2) : g.getPathTo(point1)).Skip(1).SkipLast(1);
                                    foreach(var p in start_path)
                                    {
                                        bitmap.SetPixel((int)p.X, (int)p.Y, System.Drawing.Color.CornflowerBlue);
                                        bitmap.Save($"images/{x}_{y}_{x2}_{y2}_{step++}.bmp");
                                    }
                                    int y_start = (int)((point1.Y < point2.Y) ? point1.Y : point2.Y);
                                    int y_end = (int)((point1.Y < point2.Y) ? point2.Y : point1.Y);
                                    for (int y3 = y_start; y3 <= y_end; ++y3)
                                    {
                                        if ((point1.X != start.X || y3 != start.Y) && (point1.X != end.X || y3 != end.Y))
                                        {
                                            bitmap.SetPixel((int)point1.X, y3, System.Drawing.Color.Orange);
                                            bitmap.Save($"images/{x}_{y}_{x2}_{y2}_{step++}.bmp");
                                        }
                                    }

                                    int x_start = (int)((point1.X < point2.X) ? point1.X : point2.X);
                                    int x_end = (int)((point1.X < point2.X) ? point2.X : point1.X);
                                    for (int x3 = x_start; x3 <= x_end; ++x3)
                                    {
                                        if ((x3 != start.X || y_end != start.Y) && (x3 != end.X || y_end != end.Y))
                                        {
                                            bitmap.SetPixel(x3, y_end, System.Drawing.Color.Orange);
                                            bitmap.Save($"images/{x}_{y}_{x2}_{y2}_{step++}.bmp");
                                        }
                                    }

                                    foreach (var p in end_path.Reverse())
                                    {
                                        bitmap.SetPixel((int)p.X, (int)p.Y, System.Drawing.Color.CornflowerBlue);
                                        bitmap.Save($"images/{x}_{y}_{x2}_{y2}_{step++}.bmp");
                                    }

                                    //bitmap.SetPixel((int)point1.X, (int)point1.Y, System.Drawing.Color.Yellow);
                                    //bitmap.SetPixel((int)point2.X, (int)point2.Y, System.Drawing.Color.Yellow);
                                    //bitmap.Save($"images/{x}_{y}_{x2}_{y2}.bmp");

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
//test(solve, "part1", ("sample.txt", 2L, 4L), 30);
//test(solve, "part1", ("sample.txt", 2L, 6L), 16);
//test(solve, "part1", ("sample.txt", 2L, 8L), 14);
//test(solve, "part1", ("sample.txt", 2L, 10L), 10);
//test(solve, "part1", ("sample.txt", 2L, 12L), 8);
//test(solve, "part1", ("sample.txt", 2L, 20L), 5);
//test(solve, "part1", ("sample.txt", 2L, 36L), 4);
//test(solve, "part1", ("sample.txt", 2L, 38L), 3);
//test(solve, "part1", ("sample.txt", 2L, 40L), 2);
//test(solve, "part1", ("sample.txt", 2L, 64L), 1);

//test(solve, "part1", ("input.txt", 2L, 100L), 1346);

//test(solve, "part2", ("sample.txt",20L,50L), 285);
//test(solve, "part2", ("sample.txt", 20L, 52L), 253);
//test(solve, "part2", ("sample.txt", 20L, 54L), 222);
//test(solve, "part2", ("sample.txt", 20L, 56L), 193);
//test(solve, "part2", ("sample.txt", 20L, 58L), 154);
//test(solve, "part2", ("sample.txt", 20L, 60L), 129);
//test(solve, "part2", ("sample.txt", 20L, 62L), 106);
//test(solve, "part2", ("sample.txt", 20L, 64L), 86);
//test(solve, "part2", ("sample.txt", 20L, 66L), 67);
//test(solve, "part2", ("sample.txt", 20L, 68L), 55);
//test(solve, "part2", ("sample.txt", 20L, 70L), 41);
//test(solve, "part2", ("sample.txt", 20L, 72L), 29);
//test(solve, "part2", ("sample.txt", 20L, 74L), 7);
//test(solve, "part2", ("sample.txt", 20L, 76L), 3);

//test(solve, "part2", ("input.txt", 20L, 100L), 985482);

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