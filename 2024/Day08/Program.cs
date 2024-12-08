using static Library.Geometry;
using static Library.Parsing;

(char [][] map, Dictionary<char, List<Point>> antennas) parse(string file_name)
{
    var map = readFileAsGrid(file_name);
    Dictionary<char, List<Point>> antennas = new();
    for (int y = 0; y < map.Length; ++y)
    {
        for (int x = 0; x < map[y].Length; ++x)
        {
            char c = map[y][x];
            if (c != '.')
            {
                if (antennas.ContainsKey(c))
                {
                    antennas[c].Add(new Point(x, y));
                }
                else
                {
                    antennas[c] = new() { new Point(x, y) };
                }
            }
        }
    }
    return (map, antennas);
}

IEnumerable<(Point a, Point b)> allPairs(List<Point> antennas)
{
    return antennas.SelectMany((x, i) => antennas.Skip(i + 1), (x, y) => (x, y));
}

Point getDelta((Point a, Point b) pair)
{
    long run = pair.a.X - pair.b.X;
    long rise = pair.a.Y - pair.b.Y;
    return new Point(run, rise);
}

void part1(string file_name)
{
    long solution = 0;
    var (map, antennas) = parse(file_name);

    HashSet<Point> antinodes = new();

    foreach(var point_list in antennas.Values)
    {
        foreach (var pair in allPairs(point_list))
        {
            Point move = getDelta(pair);

            Point opt1 = pair.a + move;
            if (opt1.boundsCheck(map))
            {
                antinodes.Add(opt1);
            }

            Point opt2 = pair.b - move;
            if (opt2.boundsCheck(map))
            {
                antinodes.Add(opt2);
            }
        }
    }
    solution = antinodes.Count;

    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    long solution = 0;
    var (map, antennas) = parse(file_name);

    HashSet<Point> antinodes = new();

    foreach (var point_list in antennas.Values)
    {
        foreach (var pair in allPairs(point_list))
        {
            Point move = getDelta(pair);

            Point opt1 = pair.a;
            while (opt1.boundsCheck(map))
            {
                antinodes.Add(opt1);
                opt1 += move;
            }

            Point opt2 = pair.b;
            while (opt2.boundsCheck(map))
            {
                antinodes.Add(opt2);
                opt2 -= move;
            }
        }
    }
    solution = antinodes.Count;

    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");

public static class Day8
{
    public static bool boundsCheck(this Point p, char[][] arr) => p.X >= 0 && p.Y >= 0 && p.Y < arr.Length && p.X < arr[p.Y].Length;
}