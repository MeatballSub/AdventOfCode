using System.Numerics;
using static Library.Parsing;

long part1(string file_name)
{
    var solution = 0L;
    var input = readFileLines(file_name);

    return solution;
}

long part2(string file_name)
{
    var solution = 0L;
    var input = readFileLines(file_name);

    return solution;
}

void test<T>(Func<string, T> test_func, string func_name, string file_name, T expected) where T : IEqualityOperators<T, T, bool>
{
    var old_color = Console.ForegroundColor;
    var actual = test_func(file_name);
    if (actual == expected)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{func_name} - {file_name}: {actual}");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{func_name} - {file_name}[ actual: {actual} expected: {expected} ]");
    }
    Console.ForegroundColor = old_color;
}

test(part1, "part1", "sample.txt", 0);
test(part1, "part1", "input.txt", 0);

test(part2, "part2", "sample.txt", 0);
test(part2, "part2", "input.txt", 0);