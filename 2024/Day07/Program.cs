using Library;
using static Library.Parsing;

Func<long, long, long>[] operations = 
{
    (a,b) => a + b,
    (a,b) => a * b,
    (a,b) => long.Parse(a.ToString() + b.ToString()),
};

long test_value;
long[] test_inputs;

void parse(string line)
{
    var values = line.ExtractLongs();
    test_value = values.First();
    test_inputs = values.Skip(1).ToArray();
}

bool validLine(long test_value, long accumulator, int input_index, int num_ops)
{
    if (input_index == test_inputs.Count()) return test_value == accumulator;

    for (int i = 0; i < num_ops; ++i)
    {
        var result = operations[i](accumulator, test_inputs[input_index]);
        if (result <= test_value)
        {
            if (validLine(test_value, result, input_index + 1, num_ops)) return true;
        }
    }

    return false;
}

long solve(string file_name, int num_ops)
{
    long solution = 0;
    var input = readFileLines(file_name);

    foreach (var line in input)
    {
        parse(line);
        if(validLine(test_value, test_inputs[0], 1, num_ops))
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