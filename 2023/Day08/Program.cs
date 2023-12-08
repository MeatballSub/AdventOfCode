using Library;
using System.Text.RegularExpressions;
using static Library.Parsing;

using Entry = (string ele, string left, string right);
using ProblemDesc = (string instructions, System.Collections.Generic.List<(string ele, string left, string right)> nodes);

long GCD(long a, long b)
{
    while (b != 0)
    {
        var temp = b;
        b = a % b;
        a = temp;
    }

    return a;
}

long LCM(IEnumerable<long> values) => values.Aggregate((a, b) => a / GCD(a, b) * b);


long countSteps(ProblemDesc prob, string current, EndCondition endCondition)
{
    long steps = 0;
    while (!endCondition(current))
    {
        Entry curr_entry = prob.nodes.Where(_ => _.ele == current).First();
        current = (prob.instructions[(int)(steps % prob.instructions.Length)] == 'L') ? curr_entry.left : curr_entry.right;
        ++steps;
    }
    return steps;
}

ProblemDesc parse(string file_name)
{
    string[] lines = readFile(file_name).Split($"{Environment.NewLine}{Environment.NewLine}");
    string instructions = lines[0];
    List<Entry> nodes = lines[1].SplitLines().Select(_ => Regex.Match(_, @"(?<ele>\w+) = \((?<left>\w+), (?<right>\w+)\)")).Select(_ => (_.Groups["ele"].Value, _.Groups["left"].Value, _.Groups["right"].Value)).ToList();
    return (instructions, nodes);
}

void part1(string file_name)
{
    ProblemDesc prob = parse(file_name);
    string current = "AAA";
    EndCondition endCondition = (_ => _ == "ZZZ");
    long answer = countSteps(prob, current, endCondition);
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    ProblemDesc prob = parse(file_name);
    List<string> current = prob.nodes.Where(_ => _.ele.EndsWith('A')).Select(_ => _.ele).ToList();
    EndCondition endCondition = (_ => _.EndsWith('Z'));
    List<long> steps = current.Select(_ => countSteps(prob, _, endCondition)).ToList();
    long answer = LCM(steps);
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample1.txt");
part1("sample2.txt");
part1("input.txt");
part2("sample3.txt");
part2("input.txt");

delegate bool EndCondition(string str);