using static Library.Parsing;
using static Library.Testing;

long solve(string file_name, int digits)
{
    var solution = 0L;
    var input = readFileLines(file_name);
    foreach (var bank in input)
    {
        var max_indices = new Stack<int>();
        for (int digits_remaining = digits; digits_remaining > 0; --digits_remaining)
        {
            int index = bank.Length - digits_remaining;
            int max_index = index;
            for (; index > ((max_indices.Count > 0) ? max_indices.Peek() : -1); --index)
            {
                if (bank[index] >= bank[max_index])
                {
                    max_index = index;
                }
            }
            max_indices.Push(max_index);
        }

        long multiplier = 1;
        for (int digit_index = 0; max_indices.TryPop(out digit_index);)
        {
            solution += multiplier * (bank[digit_index] - '0');
            multiplier *= 10;
        }
    }

    return solution;
}

long part1(string file_name)
{
    return solve(file_name, 2);
}

long part2(string file_name)
{
    return solve(file_name, 12);
}

test(part1, "part1", "sample.txt", 357);
test(part1, "part1", "input.txt", 17155);

test(part2, "part2", "sample.txt", 3121910778619);
test(part2, "part2", "input.txt", 169685670469164);
