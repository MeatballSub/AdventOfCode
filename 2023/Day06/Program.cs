using Library;
using static Library.Parsing;

void part1(string file_name)
{
    string[] lines = readFileLines(file_name);
    var times = lines[0].ExtractLongs().ToList();
    var distances = lines[1].ExtractLongs().ToList();

    List<long> ways = new();
    for(int i = 0; i < times.Count(); ++i)
    {
        long min = (long)(Math.Floor((-times[i] + Math.Sqrt((times[i] * times[i]) - (4 * distances[i]))) / -2) + 1);
        long max = (long)(Math.Ceiling((-times[i] - Math.Sqrt((times[i] * times[i]) - (4 * distances[i]))) / -2) - 1);
        ways.Add(max - min + 1);
    }
    Console.WriteLine($"Part1 - {file_name}: {ways.Product()}");
}

void part2(string file_name)
{
    string[] lines = readFileLines(file_name);
    var time = long.Parse(string.Join("", lines[0].Split(' ').Skip(1)));
    var distance = long.Parse(string.Join("", lines[1].Split(' ').Skip(1)));

    long min = (long)(Math.Floor((-time + Math.Sqrt((time * time) - (4 * distance))) / -2) + 1);
    long max = (long)(Math.Ceiling((-time - Math.Sqrt((time * time) - (4 * distance))) / -2) - 1);
    Console.WriteLine($"Part1 - {file_name}: {max - min + 1}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");