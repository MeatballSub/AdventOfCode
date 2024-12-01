using Library;
using static Library.Parsing;

(IEnumerable<long>, IEnumerable<long>) parse(string file_name)
{
    var lists_values = readFileLines(file_name).Select(l => l.ExtractLongs());
    var left = lists_values.Select(v => v.First());
    var right = lists_values.Select(v => v.Last());
    return (left, right);
}

long solve_part1(string file_name)
{
    var (left, right) = parse(file_name);

    return left.Order().Zip(right.Order()).Select(pair => Math.Abs(pair.First - pair.Second)).Sum();
}

long solve_part2(string file_name)
{
    var (left, right) = parse(file_name);
    var counts = right.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
    return left.Where(v => counts.ContainsKey(v)).Sum(v => v * counts[v]);
}

void part1(string file_name)
{
    long solution = solve_part1(file_name);
    Console.WriteLine($"Part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    long solution = solve_part2(file_name);
    Console.WriteLine($"Part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");