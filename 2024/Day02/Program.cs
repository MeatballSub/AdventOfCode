using Library;
using static Library.Parsing;

bool IsOrderedStrict(IEnumerable<long> enumerable) => enumerable.IsOrderedStrictAsc() || enumerable.IsOrderedStrictDesc();
long MaxDiff(IEnumerable<long> enumerable) => enumerable.Zip(enumerable.Skip(1), (a, b) => Math.Abs(a - b)).Max();
bool IsSafe(IEnumerable<long> enumerable) => IsOrderedStrict(enumerable) && MaxDiff(enumerable) < 4;
IEnumerable<IEnumerable<long>>  excludeOne(IEnumerable<long> report) => report.Select((_, i) => report.Where((v, index) => index != i));

void part1(string file_name)
{
    var safe_reports = readFileLines(file_name).Select(l => l.ExtractLongs()).Where(IsSafe);
    Console.WriteLine($"part 1 - {file_name}: {safe_reports.Count()}");
}

void part2(string file_name)
{
    var safe_reports = readFileLines(file_name).Select(l => l.ExtractLongs()).Where(r => IsSafe(r) || excludeOne(r).Any(IsSafe));
    Console.WriteLine($"part 2 - {file_name}: {safe_reports.Count()}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");