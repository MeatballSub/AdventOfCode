using Library;
using static Library.Parsing;

void run(string file_name)
{
    IEnumerable<long> sums = readFile(file_name)
        .Split($"{Environment.NewLine}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries)
        .Select(_ => _.ExtractLongs().Sum())
        .OrderDescending();

    Console.WriteLine($"Part1: {sums.First()}");
    Console.WriteLine($"Part2: {sums.Take(3).Sum()}");
}

run("sample.txt");
run("input.txt");