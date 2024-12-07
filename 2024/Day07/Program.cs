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

long solve(string file_name, int num_ops)
{
    long solution = 0;

    var input = readFileLines(file_name);
    var processed_lines = 0;
    var ten_percent = input.Length / 10;
    foreach (var line in input)
    {
        var (test_value, test_inputs) = parse(line);
        for(int i = 0; i < Math.Pow(num_ops, test_inputs.Count() - 1); ++i)
        {
            long check_result = test_inputs.First();
            int op_choice = i;
            foreach (var line_input in test_inputs.Skip(1))
            {
                var operation_key = op_choice % num_ops;
                check_result = operations[operation_key](check_result, line_input);
                op_choice /= num_ops;
            }
            if (check_result == test_value)
            {
                solution += test_value;
                break;
            }
        }
        // it's slow on part 2 okay, I need to know it's progressing
        if (ten_percent != 0)
        {
            ++processed_lines;
            if (processed_lines % ten_percent == 0)
            {
                Console.WriteLine($"{processed_lines * 100 / input.Length}%");
            }
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