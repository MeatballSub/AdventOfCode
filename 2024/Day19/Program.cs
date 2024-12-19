using static Library.Parsing;
using static Library.Testing;

(string[] towels, IEnumerable<string> patterns) parse(string file_name)
{
    var input = readFileLines(file_name);
    var towels = input[0].Split(", ", StringSplitOptions.RemoveEmptyEntries);
    var patterns = input.Skip(1);
    return (towels, patterns);
}

Dictionary<(string, string[]), long> possible_cache = new();

long countPossibilities(string pattern, string[]towels)
{
    long count = 0;
    if (pattern == "")
    {
        return 1;
    }

    var key = (pattern, towels);
    if (possible_cache.ContainsKey(key))
    {
        return possible_cache[key];
    }

    foreach(var towel in towels)
    {
        if(pattern.StartsWith(towel))
        {
            count += countPossibilities(pattern.Substring(towel.Length), towels);
        }
    }

    possible_cache[key] = count;
    return count;
}

long part1(string file_name)
{
    var (towles, patterns) = parse(file_name);
    return patterns.Where(p => 0 != countPossibilities(p, towles)).Count();
}

long part2(string file_name)
{
    var (towles, patterns) = parse(file_name);
    return patterns.Sum(p => countPossibilities(p, towles));
}

test(part1, "part1", "sample.txt", 6);
test(part1, "part1", "input.txt", 302);

test(part2, "part2", "sample.txt", 16);
test(part2, "part2", "input.txt", 771745460576799);