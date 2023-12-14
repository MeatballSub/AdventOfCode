using Library;
using System.Text;
using static Library.Parsing;

int hammingDistance(string a, string b) => a.Zip(b).Where(_ => _.First != _.Second).Count();

long solve(string pattern, int smudge_count)
{
    Dictionary<(int, int), int> reflections = new();

    string[] lines = pattern.SplitLines();
    for (int i = 0; i < lines.Length; ++i)
    {
        foreach (var reflection in reflections.ToList())
        {
            int reflection_index = reflection.Key.Item1 + reflection.Key.Item2 - i;
            if (reflection_index >= 0)
            {
                int dist = hammingDistance(lines[i], lines[reflection_index]);
                if (dist >= smudge_count)
                {
                    if (reflection.Value + dist > smudge_count) reflections.Remove(reflection.Key);
                    else reflections[reflection.Key] += dist;
                }
            }
        }
        if (i > 0)
        {
            int dist = hammingDistance(lines[i], lines[i - 1]);
            if (dist <= smudge_count) reflections.Add((i - 1, i), dist);
        }
    }
    return reflections.Where(_ => _.Value == smudge_count).Select(_ => _.Key.Item2).Sum();
}

string transpose(string pattern)
{
    string[] lines = pattern.SplitLines();
    List<StringBuilder> transposed_lines = lines.First().Select(_ => new StringBuilder()).ToList();
    for (int y = 0; y < lines.Length; ++y)
    {
        for (int x = 0; x < lines[y].Length; ++x)
        {
            transposed_lines[x].Append(lines[y][x]);
        }
    }
    return string.Join(Environment.NewLine, transposed_lines.Select(_ => _.ToString()));
}

long solve_both_dirs(string pattern, int smudge_count = 0)
{
    long h_count = solve(pattern, smudge_count);
    long v_count = solve(transpose(pattern), smudge_count);
    return v_count + 100 * h_count;
}

void part1(string file_name)
{
    string[] patterns = readFile(file_name).Split($"{Environment.NewLine}{Environment.NewLine}");
    long answer = patterns.Select(_ => solve_both_dirs(_, 0)).Sum();
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    string[] patterns = readFile(file_name).Split($"{Environment.NewLine}{Environment.NewLine}");
    long answer = patterns.Select(_ => solve_both_dirs(_, 1)).Sum();
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");