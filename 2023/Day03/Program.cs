using Library;
using System.Text.RegularExpressions;
using static Library.Parsing;

using NumberLocation = (long x, long y, long length, long value);
using SymbolLocation = (long x, long y, char symbol);

bool isAdjacent(NumberLocation number, SymbolLocation symbol) =>
    (Math.Abs(number.y - symbol.y) <= 1 && number.x - 1 <= symbol.x && symbol.x <= number.x + number.length);

bool numberHasAdjancentSymbol(NumberLocation number, List<SymbolLocation> symbols) =>
    symbols.Any(_ => isAdjacent(number, _));

List<NumberLocation> numbersAdjacentToSymbol(SymbolLocation symbol, List<NumberLocation> numbers) =>
    numbers.Where(_ => isAdjacent(_, symbol)).ToList();

(List<NumberLocation> numbers, List<SymbolLocation> symbols) parse(string file_name)
{
    string[] lines = readFileLines(file_name);
    List<NumberLocation> numbers = new();
    List<SymbolLocation> symbols = new();

    for (int i = 0; i < lines.Length; ++i)
    {
        numbers.AddRange(Regex.Matches(lines[i], @"\d+").Select(_ => new NumberLocation(_.Index, i, _.Length, long.Parse(_.Value))));
        symbols.AddRange(Regex.Matches(lines[i], @"[^0-9\.]").Select(_ => new SymbolLocation(_.Index, i, lines[i][_.Index])));
    }

    return (numbers, symbols);
}

void part1(string file_name)
{
    (var numbers, var symbols) = parse(file_name);
    long sum = numbers.Where(_ => numberHasAdjancentSymbol(_, symbols))
                      .Sum(_ => _.value);
    Console.WriteLine($"Part 1 - {file_name}: {sum}");
}

void part2(string file_name)
{
    (var numbers, var symbols) = parse(file_name);
    long sum = symbols.Where(_ => _.symbol == '*')
                      .Select(_ => numbersAdjacentToSymbol(_, numbers))
                      .Where(_ => _.Count == 2)
                      .Sum(_ => _.Select(_ => _.value).Product());
    Console.WriteLine($"Part 2 - {file_name}: {sum}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");