using Library;
using System.Text;
using static Library.Geometry;
using static Library.Parsing;

bool trace = false;

string translate(char c, int i, char start_pipe)
{
    List<string> values;
    bool is_start_pipe = (c == 'S');
    if (is_start_pipe) c = start_pipe;
    if (c == '|')
    {
        values = new List<string>
        {
          ".|.",
          ".|.",
          ".|.",
        };
    }
    else if (c == '-')
    {
        values = new List<string>
        {
          "...",
          "---",
          "...",
        };
    }
    else if (c == 'L')
    {
        values = new List<string>
        {
          ".|.",
          ".L-",
          "...",
        };
    }
    else if (c == 'J')
    {
        values = new List<string>
        {
          ".|.",
          "-J.",
          "...",
        };
    }
    else if (c == '7')
    {
        values = new List<string>
        {
          "...",
          "-7.",
          ".|.",
        };
    }
    else if (c == 'F')
    {
        values = new List<string>
        {
          "...",
          ".F-",
          ".|.",
        };
    }
    else // if (c == '.')
    {
        values = new List<string>
        {
          "...",
          "...",
          "...",
        };
    }

    if (is_start_pipe)
    {
        StringBuilder sb = new StringBuilder(values[1]);
        sb[1] = 'S';
        values[1] = sb.ToString();
    }
    return values[i];
}

List<string> translateLine(string str, char start_pipe) =>
    Enumerable.Range(0, 3).Select(n => str.Select(_ => new StringBuilder(translate(_, n, start_pipe))).Aggregate((a, b) => a.Append(b)).ToString()).ToList();

char getStartPipe(string[] input, Point start)
{
    Dictionary<Point, string> valid_neighbor_values = new()
        {
            { start.Up(), "|7F" },
            { start.Down(), "|LJ" },
            { start.Left(), "-LF" },
            { start.Right(), "-7J" },
        };

    Dictionary<Point, string> valid_values = new()
        {
            { start.Up(), "|LJ" },
            { start.Down(), "|7F" },
            { start.Left(), "-J7" },
            { start.Right(), "-LF" },
        };

    Func<Point, bool> inBounds = p => (0 <= p.X) && (p.X < input[0].Length) && (0 <= p.Y) && (p.Y < input.Count());

    string possibilities = "|-LJ7F";

    foreach (var neighbor in valid_neighbor_values)
    {
        if (inBounds(neighbor.Key) && neighbor.Value.Contains(input[neighbor.Key.Y][(int)neighbor.Key.X]))
        {
            possibilities = string.Join("",possibilities.Intersect(valid_values[neighbor.Key]));
        }
    }

    return possibilities[0];
}

(string[] lines, long height, long width, Dictionary<Point, long> steps) findLoop(string file_name, Upsample upsample)
{
    string[] input = readFileLines(file_name);
    Point start = input.Select((l, y) => (l, y)).Where(_ => _.l.Contains('S')).Select(_ => new Point(_.l.IndexOf('S'), _.y)).First();
    if(trace)
    {
        Console.WriteLine("Input:");
        for(int y = 0; y < input.Count(); ++y)
        {
            for (int x = 0; x < input[y].Length; ++x)
            {
                Console.Write(input[y][x]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    char start_pipe = getStartPipe(input, start);
    string[] lines = upsample(input, start_pipe);
    if (trace)
    {
        Console.WriteLine("Upsampled:");
        for (int y = 0; y < lines.Count(); ++y)
        {
            for (int x = 0; x < lines[y].Length; ++x)
            {
                Console.Write(lines[y][x]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    long height = lines.Count();
    long width = lines[0].Length;
    Dictionary<Point, long> steps = new();

    start = lines.Select((l, y) => (l, y)).Where(_ => _.l.Contains('S')).Select(_ => new Point(_.l.IndexOf('S'), _.y)).First();

    Queue<Point> frontier = new();
    frontier.Enqueue(start);
    steps[start] = 0;
    while (frontier.Count() > 0)
    {
        Point curr = frontier.Dequeue();
        Dictionary<Point, string> valid_values = new()
        {
            { curr.Up(), "|7FS" },
            { curr.Down(), "|LJS" },
            { curr.Left(), "-LFS" },
            { curr.Right(), "-7JS" },
        };

        Func<Point, bool> inBounds = p => (0 <= p.X) && (p.X < width) && (0 <= p.Y) && (p.Y < height);

        foreach (var neighbor in valid_values)
        {
            if (inBounds(neighbor.Key) && neighbor.Value.Contains(lines[neighbor.Key.Y][(int)neighbor.Key.X]))
            {
                if (!steps.ContainsKey(neighbor.Key))
                {
                    frontier.Enqueue(neighbor.Key);
                    steps[neighbor.Key] = steps[curr] + 1;
                }
            }
        }
    }
    if (trace)
    {
        Console.WriteLine("The loop:");
        List<StringBuilder> loop = Enumerable.Range(0, lines.Count()).Select(_ => new StringBuilder(new string('.', lines[0].Length))).ToList();
        steps.Keys.ToList().ForEach(key => loop[(int)key.Y][(int)key.X] = lines[(int)key.Y][(int)key.X]);
        for (int y = 0; y < loop.Count(); ++y)
        {
            for (int x = 0; x < loop[y].Length; ++x)
            {
                Console.Write(loop[y][x]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    return (lines, height, width, steps);
}

List<StringBuilder> Fill(List<StringBuilder> lines, Point start)
{
    Func<Point, bool> inBounds = p => (0 <= p.X) && (p.X < lines[0].Length) && (0 <= p.Y) && (p.Y < lines.Count());
    Queue<Point> frontier = new();
    frontier.Enqueue(start);

    while(frontier.Count > 0)
    {
        Point p = frontier.Dequeue();
        if (lines[(int)p.Y][(int)p.X] != '.') continue;
        lines[(int)p.Y][(int)p.X] = 'O';
        p.orthogonalNeighbors().ToList().Where(neighbor => inBounds(neighbor)).ToList().ForEach(neighbor => frontier.Enqueue(neighbor));
    }
    return lines;
}

void part1(string file_name)
{
    var loop = findLoop(file_name, (l, s) => l);
    long answer = loop.steps.Max(_ => _.Value);
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    var loop = findLoop(file_name, (l,s) => l.SelectMany(_ => translateLine(_, s)).ToArray());
    Point fill_start = new Point(0,0);

    List<StringBuilder> filled = Enumerable.Range(0, loop.lines.Count()).Select(_ => new StringBuilder(new string('.', loop.lines[0].Length))).ToList();
    loop.steps.Keys.ToList().ForEach(key => filled[(int)key.Y][(int)key.X] = loop.lines[(int)key.Y][(int)key.X]);

    filled = Fill(filled, fill_start);

    if (trace)
    {
        Console.WriteLine("Post fill:");
        for (int y = 0; y < filled.Count(); ++y)
        {
            for (int x = 0; x < filled[y].Length; ++x)
            {
                Console.Write(filled[y][x]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();

        Console.WriteLine("Downsampled:");
        for (int y = 1; y < filled.Count(); y += 3)
        {
            for (int x = 1; x < filled[y].Length; x += 3)
            {
                Console.Write(filled[y][x]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    long answer = 0;
    for (int y = 1; y < loop.height; y += 3)
    {
        for (int x = 1; x < loop.width; x += 3)
        {
            if (filled[y][x] == '.')
            {
                ++answer;
            }
        }
    }

    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");

delegate string[] Upsample(string[] lines, char start_pipe);