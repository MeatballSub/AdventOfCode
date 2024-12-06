using Library;
using static Library.Geometry;
using static Library.Parsing;

Dictionary<char, char> nextFacing = new Dictionary<char, char>()
{
    { '^', '>' },
    { '>', 'v' },
    { 'v', '<' },
    { '<', '^' },
};

Dictionary<char, Func<Point, Point>> moveGuard = new()
{
    { '^', p => p.Up() },
    { '>', p => p.Right() },
    { 'v', p => p.Down() },
    { '<', p => p.Left() },
};

Point findGuard(char[][] map)
{
    var guard_symbols = "^>v<";
    for (int y = 0; y < map.Length; y++)
    {
        for(int x = 0; x < map[y].Length; x++)
        {
            if(guard_symbols.Contains(map[y][x])) return new Point(x, y);
        }
    }
    throw new Exception("I thought this was unreachable, didn't find guard");
}

(bool is_loop, HashSet<(Point position, char facing)> path) simulate(char[][] map)
{
    Point guard_loc = findGuard(map);
    char facing = map.at(guard_loc);
    HashSet<(Point position, char facing)> path = new();

    while (guard_loc.boundsCheck(map))
    {
        if (!path.Add((guard_loc, facing))) return (true, path);

        Point new_guard_loc = moveGuard[facing](guard_loc);

        if (!new_guard_loc.boundsCheck(map)) break;

        if (map.at(new_guard_loc) == '#')
        {
            facing = nextFacing[facing];
        }
        else
        {
            guard_loc = new_guard_loc;
        }
    }

    return (false, path);
}

void part1(string file_name)
{
    var solution = simulate(readFileAsGrid(file_name)).path.Select(p => p.position).ToHashSet().Count();
    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    var solution = 0;
    var map = readFileAsGrid(file_name);
    var path = simulate(map).path.Select(p => p.position).ToHashSet();
    Point original_guard_loc = findGuard(map);

    foreach(Point p in path)
    {
        if (!p.Equals(original_guard_loc))
        {
            map[p.Y][p.X] = '#';

            if (simulate(map).is_loop)
            {
                ++solution;
            }

            map[p.Y][p.X] = '.';
        }
    }

    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");

public static class Day6
{
    public static char at(this char[][] arr, Point p) => arr[p.Y][p.X];
    public static bool boundsCheck(this Point p, char[][] arr) => p.X >= 0 && p.Y >= 0 && p.Y < arr.Length && p.X < arr[p.Y].Length;
}