using Library;
using static Library.Parsing;

int getMatches(string line) => line.winningNumbers().Intersect(line.numbersYouHave()).Count();

long points(int matches) => 1 << (matches - 1);

void part1(string file_name)
{
    long sum = readFileLines(file_name).Select(getMatches).Where(_ => _ > 0).Sum(points);
    Console.WriteLine($"Part 1 - {file_name}: {sum}");
}

void part2(string file_name)
{
    string[] lines = readFileLines(file_name);
    List<int> counts = Enumerable.Repeat(1, lines.Length).ToList();
    
    for(int i = 0; i < lines.Length; ++i)
    {
        for(int j = 1; j <= getMatches(lines[i]); ++j)
        {
            counts[i + j] += counts[i];
        }
    }

    Console.WriteLine($"Part 2 - {file_name}: {counts.Sum()}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");

static class Extensions
{
    public static IEnumerable<long> winningNumbers(this string str)
    {
        return str.Split(':')[1].Split('|')[0].ExtractLongs();
    }

    public static IEnumerable<long> numbersYouHave(this string str)
    {
        return str.Split('|')[1].ExtractLongs();
    }
}