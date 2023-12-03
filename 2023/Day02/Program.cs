using System.Text.RegularExpressions;
using static Library.Parsing;

using Game = (long game, long red, long green, long blue);

IEnumerable<Game> parse(string file_name) =>
    readFileLines(file_name).Select(_ => (_.game(), _.color("red"), _.color("green"), _.color("blue")));

bool validGame(Game game) => game.red <= 12 && game.green <= 13 && game.blue <= 14;

long power(Game game) => game.red * game.green * game.blue;

void part1(string file_name)
{
    long sum = parse(file_name).Where(validGame).Sum(_ => _.game);
    Console.WriteLine($"Part1 - {file_name}: {sum}");
}

void part2(string file_name)
{
    long sum = parse(file_name).Sum(power);
    Console.WriteLine($"Part2 - {file_name}: {sum}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");

static class extensions
{
    public static long game(this string line) => long.Parse(Regex.Match(line, @"^Game (?<game_num>\d+)").Groups["game_num"].Value);

    public static long color(this string line, string color_name) =>
        Regex.Matches(line, $@"(?<num>\d+) {color_name}").Select(_ => _.Groups["num"].Value).Select(long.Parse).Max();
}
