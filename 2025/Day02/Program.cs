using static Library.Parsing;
using static Library.Testing;

(string, string) parseRange(string range)
{
    var split = range.Split('-');
    return (split[0], split[1]);
}

long solve(string file_name, int max_num_repeats = int.MaxValue)
{
    var invalid_ids = new HashSet<long>();
    var ranges = readFileLines(file_name)[0].Split(',').Select(parseRange);
    foreach (var (begin, end) in ranges)
    {
        for (int repeats = 2; repeats <= end.Length && repeats <= max_num_repeats; ++repeats)
        {
            int repeat_length = begin.Length / repeats;

            string repeat_string = (repeat_length > 0) ? begin.Substring(0, repeat_length) : "1";
            string full_string = string.Concat(Enumerable.Repeat(repeat_string, repeats));

            long repeat_string_val = long.Parse(repeat_string);
            long full_string_val = long.Parse(full_string);

            while (full_string_val <= long.Parse(end))
            {
                if (full_string_val >= long.Parse(begin))
                {
                    invalid_ids.Add(full_string_val);
                }
                ++repeat_string_val;
                full_string = string.Concat(Enumerable.Repeat(repeat_string_val.ToString(), repeats));
                full_string_val = long.Parse(full_string);
            }
        }
    }

    return invalid_ids.Sum();
}

long part1(string file_name)
{
    return solve(file_name, 2);
}

long part2(string file_name)
{
    return solve(file_name);
}

test(part1, "part1", "sample.txt", 1227775554);
test(part1, "part1", "input.txt", 31839939622);

test(part2, "part2", "sample.txt", 4174379265);
test(part2, "part2", "input.txt", 41662374059);
