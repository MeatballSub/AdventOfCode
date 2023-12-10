using Library;
using System.Text;
using static Library.Geometry;
using static Library.Parsing;

string translate(char c, int i)
{
    List<string> values;
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
    else if (c == '.')
    {
        values = new List<string>
        {
          "...",
          "...",
          "...",
        };
    }
    else //if (c == 'S')
    {
        // this is hard coded for my input, figure it out later
        values = new List<string>
        {
          "...",
          ".S-",
          ".|I",
        };
    }
    return values[i];
}

List<string> translateLine(string str) =>
    Enumerable.Range(0, 3).Select(n => str.Select(_ => new StringBuilder(translate(_, n))).Aggregate((a, b) => a.Append(b)).ToString()).ToList();

(string[] lines, long height, long width, Dictionary<Point, long> steps) findLoop(string file_name, Preprocess preprocess)
{
    string[] lines = preprocess(readFileLines(file_name));
    long height = lines.Count();
    long width = lines[0].Length;
    Dictionary<Point, long> steps = new();

    Point start = lines.Select((l, y) => (l, y)).Where(_ => _.l.Contains('S')).Select(_ => new Point(_.l.IndexOf('S'), _.y)).First();

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
    return (lines, height, width, steps);
}

List<StringBuilder> Fill(List<StringBuilder> lines, Point start)
{
    if (lines[(int)start.Y][(int)start.X] != '.') return lines;
    lines[(int)start.Y][(int)start.X] = 'I';
    start.orthogonalNeighbors().ToList().ForEach(neighbor => lines = Fill(lines, neighbor));
    return lines;
}

void part1(string file_name)
{
    var loop = findLoop(file_name, _ => _);
    long answer = loop.steps.Max(_ => _.Value);
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    var loop = findLoop(file_name, _ => _.SelectMany(translateLine).ToArray());
    Point fill_start = loop.lines.Select((l, y) => (l, y)).Where(_ => _.l.Contains('I')).Select(_ => new Point(_.l.IndexOf('I'), _.y)).First();

    List<StringBuilder> filled = Enumerable.Range(0, loop.lines.Count()).Select(_ => new StringBuilder(new string('.', loop.lines[0].Length))).ToList();
    loop.steps.Keys.ToList().ForEach(key => filled[(int)key.Y][(int)key.X] = loop.lines[(int)key.Y][(int)key.X]);

    filled = Fill(filled, fill_start);

    long answer = 0;
    for (int y = 1; y < loop.height; y += 3)
    {
        for (int x = 1; x < loop.width; x += 3)
        {
            if (filled[y][x] == 'I')
            {
                ++answer;
            }
        }
    }

    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("input.txt");
part2("input.txt");

delegate string[] Preprocess(string[] lines);