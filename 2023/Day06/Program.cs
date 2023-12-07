using Library;
using static Library.Parsing;

long getWays(long time, long distance)
{
    long min = (long)(Math.Floor((-time + Math.Sqrt((time * time) - (4 * distance))) / -2) + 1);
    long max = (long)(Math.Ceiling((-time - Math.Sqrt((time * time) - (4 * distance))) / -2) - 1);
    return max - min + 1;
}

void part1(string file_name)
{
    long ans = readFileLines(file_name).Chunk(2).Select(_ => _[0].ExtractLongs().Zip(_[1].ExtractLongs(), (t, d) => getWays(t, d)).Product()).First();
    Console.WriteLine($"Part1 - {file_name}: {ans}");
}

void part2(string file_name)
{
    long ans = readFileLines(file_name).Select(_ => long.Parse(_.Split(':')[1].Replace(" ", ""))).Chunk(2).Select(_ => getWays(_[0], _[1])).First();
    Console.WriteLine($"Part2 - {file_name}: {ans}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");