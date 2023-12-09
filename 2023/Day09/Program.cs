using Library;
using static Library.MathStuff;
using static Library.Parsing;

IEnumerable<IEnumerable<long>> parse(string file_name) => readFileLines(file_name).ExtractSignedLongs();

void part1(string file_name)
{
    long answer = parse(file_name).Solve();
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    long answer = parse(file_name).Select(_ => _.Reverse()).Solve();
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");

static class extensions
{
    static long Extrapolate(IEnumerable<long> nums)
    {
        var nums_list = nums.ToList();
        int n = nums.Count();
        return Enumerable.Range(0, n).Select(k => ((n - k) % 2 == 0 ? -1 : 1) * GetBinCoeff(n, k) * nums_list[k]).Sum();
    }

    public static long Solve(this IEnumerable<IEnumerable<long>> input) => input.Select(Extrapolate).Sum();
}