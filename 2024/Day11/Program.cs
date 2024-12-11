using static Library.Parsing;

Dictionary<(long val, int blinks), long> memo_cache = new();

long blink(long stone_val, int blinks)
{
    var key = (stone_val, blinks);
    long answer = 0;

    if (memo_cache.TryGetValue(key, out answer)) return answer;

    if (blinks == 0) answer = 1;
    else if (stone_val == 0) answer = blink(1, blinks - 1);
    else
    {
        string val_str = stone_val.ToString();
        if (val_str.Length % 2 == 0)
        {
            int half_length = val_str.Length / 2;
            answer = blink(long.Parse(val_str.Substring(0, half_length)), blinks - 1) + blink(long.Parse(val_str.Substring(half_length)), blinks - 1);
        }
        else answer = blink(stone_val * 2024, blinks - 1);
    }

    memo_cache.Add((stone_val, blinks), answer);
    return answer;
}

void part1(string file_name)
{
    long solution = readFile(file_name).ExtractLongs().Select(stone => blink(stone, 25)).Sum();
    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    long solution = readFile(file_name).ExtractLongs().Select(stone => blink(stone, 75)).Sum();
    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

//part2("sample.txt");  // part 2 didn't even tell us what the answer was for the sample
part2("input.txt");