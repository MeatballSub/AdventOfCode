using static Library.Parsing;
using static Library.Geometry;
using System.Text;
using System.Net;

Dictionary<char, Func<Point, Point>> move_functions = new()
{
    { '^', p => p.Up() },
    { '>', p => p.Right() },
    { 'v', p => p.Down() },
    { '<', p => p.Left() },
};

Dictionary<char, Func<Point, Point>> swap_functions = new()
{
    { '^', p => p.Down() },
    { '>', p => p.Left() },
    { 'v', p => p.Up() },
    { '<', p => p.Right() },
};

(char[][] map, string moves, Point robot_position) parse1(string file_name)
{
    var parts = SplitBlankLine(file_name);
    var map = parts[0].SplitLines().Select(l => l.ToCharArray()).ToArray();
    var moves = parts[1].Replace("\n", String.Empty).Replace("\r", String.Empty);
    Point robot_position = new Point(-1,-1);

    for(int y = 0; y < map.Length; ++y)
    {
        for(int x = 0; x < map[y].Length; ++x)
        {
            if (map[y][x] == '@')
            {
                robot_position = new Point(x, y);
                map[y][x] = '.';
                break;
            }
        }
    }

    return (map, moves, robot_position);
}

(char[][] map, string moves, Point robot_position) parse2(string file_name)
{
    var parts = SplitBlankLine(file_name);
    var original_map = parts[0].SplitLines().Select(l => l.ToCharArray()).ToArray();
    var moves = parts[1].Replace("\n", String.Empty).Replace("\r", String.Empty);
    Point robot_position = new Point(-1, -1);

    // Double map
    List<string> map_buffer = new();
    for(int y = 0; y < original_map.Length; ++y)
    {
        StringBuilder sb = new();
        for (int x = 0; x < original_map[y].Length; ++x)
        {
            if(original_map[y][x] == '#')
            {
                sb.Append("##");
            }
            else if (original_map[y][x] == 'O')
            {
                sb.Append("[]");
            }
            else if(original_map[y][x] == '.')
            {
                sb.Append("..");
            }
            else if(original_map[y][x] == '@')
            {
                sb.Append("..");
                robot_position = new Point(x * 2, y);
            }
            else
            {
                throw (new Exception("Unknown map char"));
            }
        }
        map_buffer.Add(sb.ToString());
    }

    var double_map = map_buffer.Select(l => l.ToCharArray()).ToArray();

    return (double_map, moves, robot_position);
}

void sim1(ref char[][] map, string moves, Point robot_start_position)
{
    Point robot_position = robot_start_position;
    foreach(char move in moves)
    {
        Point end_point = move_functions[move](robot_position);
        while (map.at(end_point) != '#' && map.at(end_point) != '.')
        {
            end_point = move_functions[move](end_point);
        }
        if (map.at(end_point) == '.')
        {
            while (!end_point.Equals(robot_position))
            {
                Point swap_point = swap_functions[move](end_point);
                char temp = map.at(swap_point);
                map[swap_point.Y][swap_point.X] = map.at(end_point);
                map[end_point.Y][end_point.X] = temp;
                end_point = swap_point;
            }
            robot_position = move_functions[move](robot_position);
        }
    }
}

bool keepTrying(ref char[][]map, HashSet<Point> check_points)
{
    foreach(Point p in check_points)
    {
        if (map.at(p) == '#') return false;
    }

    foreach(Point p in check_points)
    {
        if (map.at(p) != '.')
        {
            return true;
        }
    }
    return false;
}

(bool, HashSet<Point>) isPossible(ref char[][]map, char move, Point robot_position)
{
    Point attempt_move_point = move_functions[move](robot_position);
    List<Point> end_points = new();
    HashSet<Point> new_end_points = new() { attempt_move_point };
    char map_value = map.at(attempt_move_point);
    if(move == 'v' || move == '^')
    {
        if (map_value == ']')
        {
            new_end_points.Add(attempt_move_point.Left());
        }
        else if (map_value == '[')
        {
            new_end_points.Add(attempt_move_point.Right());
        }
    }

    while (keepTrying(ref map, new_end_points))
    {
        end_points.AddRange(new_end_points);
        new_end_points = new_end_points.Select(p => move_functions[move](p)).ToHashSet();
        foreach(Point p in new_end_points.ToList())
        {
            map_value = map.at(p);
            if(map_value == '.')
            {
                new_end_points.Remove(p);
            }
            else if (move == 'v' || move == '^')
            {
                if (map_value == ']')
                {
                    new_end_points.Add(p.Left());
                }
                else if (map_value == '[')
                {
                    new_end_points.Add(p.Right());
                }
            }
        }
    }

    foreach(Point p in new_end_points)
    {
        if (map.at(p) == '#' || map.at(p) != '.') return (false, new HashSet<Point>());
    }
    return (true, end_points.ToHashSet());
}

void sim2(ref char[][] map, string moves, Point robot_start_position)
{
    Point robot_position = robot_start_position;

    //Console.WriteLine();
    //for (int y = 0; y < map.Length; y++)
    //{
    //    for (int x = 0; x < map[y].Length; x++)
    //    {
    //        if (y == robot_position.Y && x == robot_position.X)
    //        {
    //            Console.Write('@');
    //        }
    //        else
    //        {
    //            Console.Write(map[y][x]);
    //        }
    //    }
    //    Console.WriteLine();
    //}

    foreach (char move in moves)
    {
        //Console.WriteLine();
        //Console.WriteLine(move);
        var (is_possible, move_points) = isPossible(ref map, move, robot_position);
        if (is_possible)
        {
            List<(Point, char)> move_values = new();
            foreach(Point p in move_points)
            {
                move_values.Add((p, map.at(p)));
            }
            foreach(var (p, v) in move_values)
            {
                map[p.Y][p.X] = '.';
            }
            foreach (var (p, v) in move_values)
            {
                var np = move_functions[move](p);
                map[np.Y][np.X] = v;
            }
            robot_position = move_functions[move](robot_position);
        }

        //Console.WriteLine();
        //for (int y = 0; y < map.Length; y++)
        //{
        //    for (int x = 0; x < map[y].Length; x++)
        //    {
        //        if (y == robot_position.Y && x == robot_position.X)
        //        {
        //            Console.Write('@');
        //        }
        //        else
        //        {
        //            Console.Write(map[y][x]);
        //        }
        //    }
        //    Console.WriteLine();
        //}
        //Console.WriteLine();
        //Console.WriteLine();
    }
}

void part1(string file_name)
{
    var solution = 0;
    var (map, moves, robot_position) = parse1(file_name);
    sim1(ref map, moves, robot_position);
    for(int y = 0; y < map.Length; y++)
    {
        for(int x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == 'O')
            {
                solution += 100 * y + x;
            }
        }
    }

    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    var solution = 0;
    var (map, moves, robot_position) = parse2(file_name);
    sim2(ref map, moves, robot_position);
    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == '[')
            {
                solution += 100 * y + x;
            }
        }
    }

    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

Console.Write("Expecting: 10092 | ");
part1("sample.txt");
Console.Write("Expecting: 2028 | ");
part1("sample2.txt");
Console.Write("Expecting: 1499739 | ");
part1("input.txt");

Console.Write("Expecting: 9021 | ");
part2("sample.txt");
Console.Write("Expecting: 1522215 | ");
part2("input.txt");