using static Library.Parsing;
using static Library.Testing;

(List<List<int>> locks, List<List<int>> keys) parse(string file_name)
{
    List<List<int>> keys = new();
    List<List<int>> locks = new();

    var input = SplitBlankLine(file_name);

    foreach(var map in input)
    {
        var lines = map.SplitLines();
        bool is_lock = (lines[0].StartsWith('#'));
        IEnumerable<string> count_this = is_lock ? lines.Skip(1) : lines.Reverse().Skip(1);
        ref List<List<int>> storage = ref (is_lock ? ref locks : ref keys);

        List<int> values = new() { 0, 0, 0, 0, 0 };
        foreach (var line in count_this)
        {
            for (int i = 0; i < line.Length; ++i)
            {
                if (line[i] == '#') ++values[i];
            }
        }

        storage.Add(values);
    }

    return (locks, keys);
}

long part1(string file_name)
{
    var solution = 0L;
    var (locks, keys) = parse(file_name);

    foreach(var the_lock in locks)
    {
        foreach(var key in keys)
        {
            if (the_lock.Zip(key).Select(lock_and_key => lock_and_key.First + lock_and_key.Second).All(v => v < 6)) ++solution;
        }
    }

    return solution;
}

test(part1, "part1", "sample.txt", 3);
test(part1, "part1", "input.txt", 3365);