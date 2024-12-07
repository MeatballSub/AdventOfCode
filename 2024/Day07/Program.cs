using Library;
using static Library.Parsing;

Dictionary<int, Func<long, long, long>> operations = new()
{
    { 0, (a,b) => a + b },
    { 1, (a,b) => a * b },
    { 2, (a,b) => long.Parse(a.ToString() + b.ToString()) },
};

(long test_value, IEnumerable<long> test_inputs) parse(string line)
{
    var line_parts = line.Split(':');
    var test_value = line_parts[0].ExtractLongs().First();
    var line_inputs = line_parts[1].ExtractLongs();
    return (test_value, line_inputs);
}

bool validLine(long test_value, long accumulator, IEnumerable<long> test_inputs, int num_ops)
{
    if (test_inputs.Count() == 0) return test_value == accumulator;

    long second = test_inputs.First();

    for (int i = 0; i < num_ops; ++i)
    {
        var result = operations[i](accumulator, second);
        if (result <= test_value)
        {
            if (validLine(test_value, result, test_inputs.Skip(1), num_ops)) return true;
        }
    }

    return false;
}

long solve(string file_name, int num_ops)
{
    long solution = 0;
    var input = readFileLines(file_name).Select(l => parse(l));

    foreach (var (test_value, test_inputs) in input)
    {
        if(validLine(test_value, test_inputs.First(), test_inputs.Skip(1), num_ops))
        {
            solution += test_value;
        }
    }

    return solution;
}

void part1(string file_name)
{
    long solution = solve(file_name, 2);
    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    long solution = solve(file_name, 3);
    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");