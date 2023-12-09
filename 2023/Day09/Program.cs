using Library;
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
    private static long GetBinCoeff(long N, long K)
    {
        long r = 1;
        long d;
        if (K > N) return 0;
        for (d = 1; d <= K; d++)
        {
            r *= N--;
            r /= d;
        }
        return r;
    }
    public static long Extrapolate(this List<long> nums)
    {
        int n = nums.Count();
        return Enumerable.Range(0, n).Select(k => ((n - k) % 2 == 0 ? -1 : 1) * GetBinCoeff(n, k) * nums[k]).Sum();
    }
}