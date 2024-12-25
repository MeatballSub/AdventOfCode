using static Library.Parsing;
using static Library.Testing;

(List<List<int>> locks, List<List<int>> keys) parse(string file_name)
{
    List<List<int>> keys = new();
    List<List<int>> locks = new();

    foreach(var map in SplitBlankLine(file_name))
    {
        var lines = map.SplitLines();
        ref List<List<int>> storage = ref (lines[0].StartsWith('#') ? ref locks : ref keys);

        List<int> values = new() { 0, 0, 0, 0, 0 };
        foreach (var line in lines.Skip(1).SkipLast(1))
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