using System.Text.RegularExpressions;
using static Library.Parsing;

Regex parse_regex = new Regex(@"(?<label>.*?)(?<operation>[-=])(?<focal_length>\d*)");
long focalLength(Match lens) => long.Parse(lens.Groups["focal_length"].Value);
long HASH(string str) => str.Aggregate(0, (a, b) => ((a + b) * 17) % 256);
long focusingPower(Dictionary<long, List<Match>> boxes) =>
    boxes.Select(box => box.Value.Select((l, i) => (i + 1) * focalLength(l)).Sum(v => v * (box.Key + 1))).Sum();

void part1(string file_name)
{
    string[] steps = readFile(file_name).Split(',');
    long answer = steps.Select(HASH).Sum();
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    Dictionary<long, List<Match>> boxes = new();
    string[] steps = readFile(file_name).Split(',');
    foreach (var step in steps)
    {
        var match = parse_regex.Match(step);
        var lensMatches = (Match m) => m.Groups["label"].Value == match.Groups["label"].Value;
        long box = HASH(match.Groups["label"].Value);

        if (!boxes.ContainsKey(box)) boxes[box] = new List<Match>();

        int index = boxes[box].FindIndex(_ => lensMatches(_));

        if (match.Groups["operation"].Value == "-")
        {
            boxes[box] = boxes[box].Where(_ => !lensMatches(_)).ToList();
        }
        else if (index >= 0)
        {            
            boxes[box][index] = match;
        }
        else
        {
            boxes[box].Add(match);
        }
    }

    Console.WriteLine($"Part 2 - {file_name}: {focusingPower(boxes)}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");