using static Library.Parsing;
using static Library.Geometry;
using System.Text;
using System.Numerics;

Dictionary<char, Func<Point, Point>> move_functions = new()
{
    { '^', p => p.Up() },
    { '>', p => p.Right() },
    { 'v', p => p.Down() },
    { '<', p => p.Left() },
};

(char[][] map, string moves) parse(string file_name)
{
    var parts = SplitBlankLine(file_name);
    var map = parts[0].SplitLines().Select(l => l.ToCharArray()).ToArray();
    var moves = parts[1].Replace(Environment.NewLine, String.Empty);

    return (map, moves);
}

void findRobot(ref char[][] map, out Point? robot_position)
{
    robot_position = null;

    for (int y = 0; y < map.Length; ++y)
    {
        for (int x = 0; x < map[y].Length; ++x)
        {
            if (map[y][x] == '@')
            {
                robot_position = new Point(x, y);
                map[y][x] = '.';
                return;
            }
        }
    }
}

char[][] expandMap(ref char[][] map)
{
    List<string> map_buffer = new();
    for (int y = 0; y < map.Length; ++y)
    {
        StringBuilder sb = new();
        for (int x = 0; x < map[y].Length; ++x)
        {
            if (map[y][x] == '#')
            {
                sb.Append("##");
            }
            else if (map[y][x] == 'O')
            {
                sb.Append("[]");
            }
            else if (map[y][x] == '.')
            {
                sb.Append("..");
            }
            else if (map[y][x] == '@')
            {
                sb.Append("@.");
            }
            else
            {
                throw (new Exception("Unknown map char"));
            }
        }
        map_buffer.Add(sb.ToString());
    }

    return map_buffer.Select(l => l.ToCharArray()).ToArray();
}

List<Point> getNeighbors(ref char[][] map, char move, Point curr)
{
    List<Point> neighbors = new();
    var curr_value = map.at(curr);
    if (curr_value == '#') return neighbors;

    var neighbor = move_functions[move](curr);
    var neighbor_value = map.at(neighbor);

    if (move == 'v' || move == '^')
    {
        if (neighbor_value == '[')
        {
            neighbors.Add(neighbor.Right());
        }
        else if (neighbor_value == ']')
        {
            neighbors.Add(neighbor.Left());
        }
    }

    if (neighbor_value != '.')
    {
        neighbors.Add(neighbor);
    }

    return neighbors;
}

(bool valid, HashSet<Point> obj) buildObject(ref char[][] map, char move, Point robot_position)
{
    bool valid = true;
    HashSet<Point> obj = new();
    Queue<Point> queue = new();
    queue.Enqueue(robot_position);
    while (queue.TryDequeue(out var curr))
    {
        obj.Add(curr);
        if (map.at(curr) == '#')
        {
            valid = false;
            break;
        }

        foreach (var neighbor in getNeighbors(ref map, move, curr))
        {
            queue.Enqueue(neighbor);
        }
    }
    return (valid, obj);
}

void moveObject(ref char[][]map, char move, HashSet<Point> obj)
{
    List<(Point, char)> move_values = new();
    foreach (Point p in obj)
    {
        move_values.Add((p, map.at(p)));
    }
    foreach (var (p, v) in move_values)
    {
        map[p.Y][p.X] = '.';
    }
    foreach (var (p, v) in move_values)
    {
        var np = move_functions[move](p);
        map[np.Y][np.X] = v;
    }
}

void sim(ref char[][] map, string moves, Point robot_start_position)
{
    Point robot_position = robot_start_position;
    foreach (char move in moves)
    {
        var (valid_move, obj_to_move) = buildObject(ref map, move, robot_position);
        if(valid_move)
        {
            moveObject(ref map, move, obj_to_move);
            robot_position = move_functions[move](robot_position);
        }
    }
}

long score(ref char[][] map, char score_symobl)
{
    var solution = 0L;

    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == score_symobl)
            {
                solution += 100 * y + x;
            }
        }
    }

    return solution;
}

long part1(string file_name)
{
    var (map, moves) = parse(file_name);

    findRobot(ref map, out var robot_position);
    if (robot_position == null) throw new Exception("Couldn't find robot");

    sim(ref map, moves, robot_position);
    return score(ref map, 'O');
}

long part2(string file_name)
{
    var (original_map, moves) = parse(file_name);
    var map = expandMap(ref original_map);

    findRobot(ref map, out var robot_position);
    if (robot_position == null) throw new Exception("Couldn't find robot");

    sim(ref map, moves, robot_position);
    return score(ref map, '[');
}

void test<T>(Func<string, T> test_func, string func_name, string file_name, T expected) where T: IEqualityOperators<T,T,bool>
{
    var old_color = Console.ForegroundColor;
    var actual = test_func(file_name);
    if (actual == expected)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{func_name} - {file_name}: {actual}");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{func_name} - {file_name}[ actual: {actual} expected: {expected} ]");
    }
    Console.ForegroundColor = old_color;
}

test(part1, "part1", "sample.txt", 10092);
test(part1, "part1", "sample2.txt", 2028);
test(part1, "part1", "input.txt", 1499739);

test(part2, "part2", "sample.txt", 9021);
test(part2, "part2", "input.txt", 1522215);