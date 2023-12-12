using static Library.Geometry;
using static Library.Parsing;

long solve(string file_name, long expansion_factor)
{
    List<string> lines = readFileLines(file_name).ToList();
    List<int> empty_rows = new();
    List<int> empty_cols = Enumerable.Range(0, lines[0].Length).ToList();

    for (int y = 0; y < lines.Count; y++)
    {
        if (!lines[y].Contains('#'))
        {
            empty_rows.Add(y);
        }
        for (int x = 0; x < lines[y].Length; x++)
        {
            if (lines[y][x] == '#')
            {
                empty_cols.Remove(x);
            }
        }
    }

    List<Point> galaxies = new();
    for (int y = 0; y < lines.Count; ++y)
    {
        long expand_y = empty_rows.Where(_ => _ < y).Count();
        for (int x = 0; x < lines[y].Length; x++)
        {
            if (lines[y][x] == '#')
            {
                long expand_x = empty_cols.Where(_ => _ < x).Count();
                galaxies.Add(new Point(x + (expansion_factor - 1) * expand_x, y + (expansion_factor - 1) * expand_y));
            }
        }
    }

    long sum = 0;
    for (int i = 0; i < galaxies.Count; ++i)
    {
        for (int j = i + 1; j < galaxies.Count; j++)
        {
            sum += manhattanDistance(galaxies[i], galaxies[j]);
        }
    }
    return sum;
}

Console.WriteLine($"Part 1 - sample: {solve("sample.txt", 2)}");
Console.WriteLine($"Part 1 - input: {solve("input.txt", 2)}");
Console.WriteLine($"Part 2 - sample: {solve("sample.txt", 10)}");
Console.WriteLine($"Part 2 - sample: {solve("sample.txt", 100)}");
Console.WriteLine($"Part 2 - input: {solve("input.txt", 1000000)}");