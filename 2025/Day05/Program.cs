using static Library.Parsing;
using static Library.Testing;

// TODO: add this range stuff to Library
bool overlap((long begin, long end) a, (long begin, long end) b)
{
    return a.begin <= b.end && a.end >= b.begin;
}

(long, long) mergeRange((long begin, long end) a, (long begin, long end) b)
{
    var begin = Math.Min(a.begin, b.begin);
    var end = Math.Max(a.end, b.end);
    return (begin, end);
}

List<(long, long)> mergeRanges(IEnumerable<(long begin, long end)> ranges)
{
    List<(long, long)> merged = ranges.OrderBy(r => r.begin).ToList();
    for (int i = 0; i < merged.Count - 1;)
    {
        if (overlap(merged[i], merged[i + 1]))
        {
            merged[i] = mergeRange(merged[i], merged[i + 1]);
            merged.RemoveAt(i + 1);
        }
        else
        {
            ++i;
        }
    }
    return merged;
}

(IEnumerable<(long, long)> ranges, IEnumerable<long> ingredients) parse(string file_name)
{
    var input = SplitBlankLine(file_name);
    var ranges = input[0].SplitLines().Select(r => r.Split('-')).Select(r => (long.Parse(r[0]), long.Parse(r[1])));
    ranges = mergeRanges(ranges);
    var ingredients = input[1].SplitLines().Select(long.Parse);
    return (ranges, ingredients);
}

long part1(string file_name)
{
    var solution = 0L;
    var (ranges, ingredients) = parse(file_name);

    foreach (var ingredient in ingredients)
    {
        foreach (var (begin, end) in ranges)
        {
            if (begin <= ingredient && ingredient <= end)
            {
                ++solution;
                break;
            }
        }
    }

    return solution;
}

long part2(string file_name)
{
    long solution = 0;
    var (ranges, _) = parse(file_name);

    foreach (var (begin, end) in ranges)
    {
        solution += end - begin + 1;
    }

    return solution;
}

test(part1, "part1", "sample.txt", 3);
test(part1, "part1", "input.txt", 643);

test(part2, "part2", "sample.txt", 14);
test(part2, "part2", "input.txt", 342018167474526);
