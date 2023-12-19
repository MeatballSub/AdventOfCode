using Library;
using System.Text.RegularExpressions;
using static Library.Geometry;
using static Library.Parsing;

using Instruction = (char dir, int meters, string color_code);
using Parser = System.Func<string, (char dir, int meters, string color_code)>;

Dictionary<char, Move> moves = new()
{
    { 'R', (p,d) => p.Right(d) },
    { 'D', (p,d) => p.Down(d) },
    { 'L', (p,d) => p.Left(d) },
    { 'U', (p,d) => p.Up(d) },
};

Dictionary<char, char> hexToDir = new()
{
    { '0', 'R' },
    { '1', 'D' },
    { '2', 'L' },
    { '3', 'U' },
};

Instruction parse1(string line)
{
    var match = Regex.Match(line, @"(?<dir>[ULDR]) (?<meters>\d+) \(#(?<color_code>[0-9a-f]+)\)");
    return new Instruction(match.get("dir")[0], int.Parse(match.get("meters")), match.get("color_code"));
}

Instruction parse2(string line)
{
    var match = Regex.Match(line, @"[ULDR] \d+ \(#(?<color_code>[0-9a-f]+)\)");
    string color_code = match.get("color_code");
    char dir = hexToDir[color_code[5]];
    int meters = int.Parse(color_code.Substring(0, 5), System.Globalization.NumberStyles.HexNumber);
    return new Instruction(dir, meters, color_code);
}

long shoelace(List<Point> trench)
{
    long sum1 = trench.SkipLast(1).Select((p, i) => trench[i].X * trench[(i + 1) % trench.Count].Y).Sum();
    long sum2 = trench.SkipLast(1).Select((p, i) => trench[i].Y * trench[(i + 1) % trench.Count].X).Sum();
    return Math.Abs(sum1 - sum2) / 2;
}

long perimeter(List<Point> trench) => trench.SkipLast(1).Select((p, i) => manhattanDistance(trench[i], trench[i + 1])).Sum();

long solve(string file_name, Parser parser)
{
    string[] lines = readFileLines(file_name);
    Point curr_location = new Point(0, 0);
    List<Point> trench = new List<Point>() { new Point(0, 0) };
    List<Instruction> instructions = lines.Select(parser).ToList();

    foreach (var instruction in instructions)
    {
        curr_location = moves[instruction.dir](curr_location, instruction.meters);
        trench.Add(curr_location);
    }

    return shoelace(trench) + perimeter(trench) / 2 + 1;
}

void part1(string file_name)
{
    long answer = solve(file_name, parse1);
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    long answer = solve(file_name, parse2);
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");

delegate Point Move(Point p, long d);