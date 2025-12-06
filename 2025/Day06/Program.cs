using Library;
using static Library.Parsing;
using static Library.Testing;

List<List<string>> transposeList(string[] input)
{
    List<List<string>> transposed = new();
    var lists = input.Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToList();
    for (int i = 0; i < lists[0].Count(); ++i)
    {
        List<string> row = new();
        foreach (var list in lists)
        {
            row.Add(list[i]);
        }
        transposed.Add(row);
    }

    return transposed;
}

List<List<string>> transposeChars(string[] input)
{
    List<List<string>> problems = new();
    List<string> ops = input.Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

    var grid = input.SkipLast(1).Select(l => l.ToCharArray()).ToArray();
    char[][] transposed = new char[grid[0].Length][];
    for (int y = 0; y < transposed.Length; ++y)
    {
        transposed[y] = new char[grid.Length];
    }

    for (int y = 0; y < grid.Length; ++y)
    {
        for (int x = 0; x < grid[y].Length; ++x)
        {
            transposed[x][y] = grid[y][x];
        }
    }

    {
        List<string> problem = new();
        int index = 0;
        foreach (var line in transposed)
        {
            var str = new string(line);
            // Console.WriteLine(str);
            if (string.IsNullOrWhiteSpace(str))
            {
                problem.Add(ops[index]);
                problems.Add(problem);
                ++index;
                problem = new();
            }
            else
            {
                problem.Add(str);
            }
        }
        problem.Add(ops[index]);
        problems.Add(problem);
    }

    return problems;
}

long solveProblem(List<string> problem)
{
    if (problem.Last() == "+")
    {
        return problem.SkipLast(1).Select(long.Parse).Sum();
    }
    else
    {
        return problem.SkipLast(1).Select(long.Parse).Product();
    }
}

long part1(string file_name)
{
    var input = transposeList(readFileLines(file_name));
    return input.Select(solveProblem).Sum();
}

long part2(string file_name)
{
    var input = transposeChars(readFileLines(file_name));
    return input.Select(solveProblem).Sum();
}

test(part1, "part1", "sample.txt", 4277556);
test(part1, "part1", "input.txt", 6295830249262);

test(part2, "part2", "sample.txt", 3263827);
test(part2, "part2", "input.txt", 9194682052782);
