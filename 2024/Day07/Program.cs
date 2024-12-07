using Library;
using static Library.Parsing;

long concat(long a, long b)
{
    long result = b;
    while(b > 0)
    {
        a *= 10;
        b /= 10;
    }
    return result + a;
}

long[] test_inputs;

bool validLine1(long accumulator, int input_index)
{
    if (accumulator > test_inputs[0]) return false;
    if (input_index == test_inputs.Length) return (accumulator == test_inputs[0]);
    var result = accumulator + test_inputs[input_index];
    if (validLine1(result, input_index + 1)) return true;
    result = accumulator * test_inputs[input_index];
    return validLine1(result, input_index + 1);
}

bool validLine2(long accumulator, int input_index)
{
    if (accumulator > test_inputs[0]) return false;
    if (input_index == test_inputs.Length) return (accumulator == test_inputs[0]);
    var result = accumulator + test_inputs[input_index];
    if (validLine2(result, input_index + 1)) return true;
    result = accumulator * test_inputs[input_index];
    if (validLine2(result, input_index + 1)) return true;
    result = concat(accumulator, test_inputs[input_index]);
    return validLine2(result, input_index + 1);
}

void part1(string file_name)
{
    long solution = 0;
    var input = readFileLines(file_name);

    foreach (var line in input)
    {
        test_inputs = line.ExtractLongs().ToArray();
        if (validLine1(test_inputs[1], 2)) solution += test_inputs[0];
    }

    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    long solution = 0;
    var input = readFileLines(file_name);

    foreach (var line in input)
    {
        test_inputs = line.ExtractLongs().ToArray();
        if (validLine2(test_inputs[1], 2)) solution += test_inputs[0];
    }

    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");