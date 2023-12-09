using Library;
using static Library.MathStuff;
using static Library.Parsing;

void part1(string file_name)
{
    long answer = readFileLines(file_name).Select(_ => _.ExtractLongs().ToList().Extrapolate()).Sum();
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    long answer = readFileLines(file_name).Select(_ => _.ExtractLongs().Reverse().ToList().Extrapolate()).Sum();
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");

static class extensions
{
    public static long Extrapolate(this List<long> nums)
    {
        int n = nums.Count();
        return Enumerable.Range(0, n).Select(k => ((n - k) % 2 == 0 ? -1 : 1) * GetBinCoeff(n, k) * nums[k]).Sum();
    }
}