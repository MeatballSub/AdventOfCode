using Library;
using static Library.Parsing;

(Dictionary<long, List<long>> precedes_rules, List<List<long>> successes, List<List<long>> failures) parse(string file_name)
{
    Dictionary<long, List<long>> precedes_rules = new();
    List<List<long>> successes = new();
    List<List<long>> failures = new();

    var input = File.ReadAllText(file_name).Split(Environment.NewLine);

    int line_index = 0;
    for (; line_index < input.Length; line_index++)
    {
        if (input[line_index] == "") break;

        var pages = input[line_index].ExtractLongs();
        var first = pages.First();
        var second = pages.Last();

        if (!precedes_rules.ContainsKey(first))
        {
            precedes_rules[first] = new();
        }
        precedes_rules[first].Add(second);
    }

    ++line_index;

    for (; line_index < input.Length; line_index++)
    {
        var pages = input[line_index].ExtractLongs().ToList();
        bool success = true;
        for (int i = pages.Count - 1; i >= 0; --i)
        {
            if (!pages.SkipLast(pages.Count - i).All(p => !precedes_rules.ContainsKey(pages[i]) || !precedes_rules[pages[i]].Contains(p)))
            {
                success = false;
                break;
            }
        }
        if(success)
        {
            successes.Add(pages);
        }
        else
        {
            failures.Add(pages);
        }
    }

    return (precedes_rules, successes, failures);
}

void part1(string file_name)
{
    var problem = parse(file_name);
    var solution = problem.successes.Select(pages => pages[pages.Count / 2]).Sum();

    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    var problem = parse(file_name);
    var solution = problem.failures.Select(pages => pages.Order(Comparer<long>.Create((long a, long b) =>
    {
        if (problem.precedes_rules.ContainsKey(a) && problem.precedes_rules[a].Contains(b))
        {
            return -1;
        }

        if (problem.precedes_rules.ContainsKey(b) && problem.precedes_rules[b].Contains(a))
        {
            return 1;
        }
        return 0;
    }))).Select(pages => pages.ElementAt(pages.Count() / 2)).Sum();


    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");