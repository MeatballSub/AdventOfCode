using System.Text.RegularExpressions;
using static Library.Parsing;

void part1(string file_name)
{
    var input = readFile(file_name);
    var matches = Regex.Matches(input, @"mul\((?<a>\d{1,3}),(?<b>\d{1,3})\)");

    long solution = 0;
    foreach (Match match in matches)
    {
        solution += long.Parse(match.Groups["a"].Value) * long.Parse(match.Groups["b"].Value);
    }
    
    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    var input = readFile(file_name);
    var matches = Regex.Matches(input, @"mul\((?<a>\d{1,3}),(?<b>\d{1,3})\)|do\(\)|don't\(\)");
    long solution = 0;
    bool enabled = true;

    foreach (Match match in matches)
    {
        if (match.ToString() == "do()")
        {
            enabled = true;
        }
        else if(match.ToString() == "don't()")
        {
            enabled = false;
        }
        else if(enabled)
        {
            solution += long.Parse(match.Groups["a"].Value) * long.Parse(match.Groups["b"].Value);
        }
    }


    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");