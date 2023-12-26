using System.Text.RegularExpressions;
using static Library.Parsing;

Dictionary<Key, long> cache = new();

Key parse(string line)
{
    string[] split_line = line.Split(' ');
    string conditions = split_line[0];
    List<long> damgaged_groups = split_line[1].Split(',').Select(long.Parse).ToList();
    return new Key(conditions, damgaged_groups);
}

bool isPossiblyValid(Key key)
{
    string locked = key.conditions.Split('?')[0];
    var current = locked.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(_ => (long)_.Length);
    var zipped = key.damaged_groups.Take(current.Count()).Zip(current);

    if (locked.EndsWith('#'))
    {
        var last_zipped = zipped.Last();
        return last_zipped.First >= last_zipped.Second && zipped.SkipLast(1).All(_ => _.First == _.Second);
    }
    return zipped.All(_ => _.First == _.Second);
}

bool isValid(Key key) => key.conditions.Replace('?', '.').Split('.', StringSplitOptions.RemoveEmptyEntries).Select(_ => (long)_.Length).SequenceEqual(key.damaged_groups);

Key selectAndShorten(Key key, string replace)
{
    Regex regex = new Regex(@"\?");
    string front = key.conditions.Split('?')[0].TrimEnd('#');
    string conditions = regex.Replace(key.conditions, replace, 1);
    conditions = conditions.Substring(front.Length);

    int skip = front.Split('.', StringSplitOptions.RemoveEmptyEntries).Count();
    List<long> damaged_groups = key.damaged_groups.Skip(skip).ToList();

    return new Key(conditions, damaged_groups);
}

long solve(Key key)
{
    if (cache.ContainsKey(key)) return cache[key];

    int available = key.conditions.Count(c => c == '?');
    long needed = key.damaged_groups.Sum() - key.conditions.Count(c => c == '#');
    long answer = 0;

    if(needed == available)
    {
        Key newKey = new Key(key.conditions.Replace('?', '#'), key.damaged_groups);
        answer = isValid(newKey) ? 1 : 0;
    }
    else if (needed == 0)
    {
        answer = isValid(key) ? 1 : 0;
    }
    else if (0 < needed && needed < available && isPossiblyValid(key))
    {
        Key newDamagedKey = selectAndShorten(key, "#");
        Key newOperationalKey = selectAndShorten(key, ".");
        answer = solve(newDamagedKey) + solve(newOperationalKey);
    }

    cache.Add(key, answer);
    return answer;
}

string unfold(string line)
{
    string[] input = line.Split(' ');
    string conditions = string.Join('?', input[0], input[0], input[0], input[0], input[0]);
    string damaged_groups = string.Join(',', input[1], input[1], input[1], input[1], input[1]);
    return string.Join(' ', conditions, damaged_groups);
}

long solveLine(string line) => solve(new Key(parse(line)));

void part1(string file_name)
{
    long answer = readFileLines(file_name).Select(solveLine).Sum();
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    long answer = readFileLines(file_name).Select(unfold).Select(solveLine).Sum();
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
//part1("input.txt");
//part2("sample.txt");
//part2("input.txt");


class Key : IEquatable<Key>
{
    public Key(string cond, List<long> groups)
    {
        conditions = cond;
        damaged_groups = new List<long>(groups);
    }

    public Key(Key key) : this(key.conditions, new List<long>(key.damaged_groups))
    {
    }

    public string conditions;
    public List<long> damaged_groups;
    public bool Equals(Key? other)
    {
        return (conditions == other.conditions) && (damaged_groups.SequenceEqual(other.damaged_groups));
    }

    public override int GetHashCode()
    {
        int code = conditions.GetHashCode();
        damaged_groups.ForEach(g => HashCode.Combine(code, g.GetHashCode()));
        return code;
    }
}