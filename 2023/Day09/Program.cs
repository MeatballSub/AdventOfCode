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
        long n = nums.Count();
        long value = 0;
        for (int k = 0; k < nums.Count(); ++k)
        {
            long sign = (n - k) % 2 == 0 ? -1 : 1;
            long coef = GetBinCoeff(n, k);
            value += nums[k] * coef * sign;
        }
        return value;
    }
}