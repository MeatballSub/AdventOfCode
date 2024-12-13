using static Library.Parsing;
using static Library.Geometry;

List<(Point a, Point b, Point prize)> parse(string file_name)
{
    List<(Point a, Point b, Point prize)> machines = new();
    var file_lines = readFileLines(file_name);
    for(int i = 0; i < file_lines.Length; i+= 3)
    {
        var a_line = file_lines[i].ExtractLongs();
        Point a_move = new Point(a_line.First(), a_line.Last());

        var b_line = file_lines[i + 1].ExtractLongs();
        Point b_move = new Point(b_line.First(), b_line.Last());

        var prize_line = file_lines[i + 2].ExtractLongs();
        Point prize_location = new Point(prize_line.First(), prize_line.Last());

        machines.Add((a_move, b_move, prize_location));
    }
    return machines;
}

long solve(List<(Point a, Point b, Point prize)> machines)
{
    List<(long a_presses, long b_presses)> prize_solutions = new();

    foreach (var machine in machines)
    {
        var B_numerator = (machine.prize.Y * machine.a.X - machine.prize.X * machine.a.Y);
        var B_denominator = (machine.a.X * machine.b.Y - machine.a.Y * machine.b.X);

        if (B_numerator % B_denominator == 0)
        {
            var B = B_numerator / B_denominator;

            var A_numerator = (machine.prize.X - B * machine.b.X);
            var A_denominator = machine.a.X;
            if (A_numerator % A_denominator == 0)
            {
                var A = A_numerator / A_denominator;

                prize_solutions.Add((A, B));
            }
        }
    }

    return prize_solutions.Select(solution => 3 * solution.a_presses + solution.b_presses).Sum();
}

(Point a, Point b, Point prize) farAwayPrize((Point a, Point b, Point prize) machine)
{
    Point far_move = new Point(10000000000000, 10000000000000);
    return (machine.a, machine.b, machine.prize + far_move);
}

void part1(string file_name)
{
    var machines = parse(file_name);
    var solution = solve(machines);
    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    var machines = parse(file_name).Select(farAwayPrize).ToList();
    var solution = solve(machines);

    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");