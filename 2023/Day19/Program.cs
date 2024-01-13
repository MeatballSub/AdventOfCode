using Library;
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

using Rule = (char var, char op, long value, string result);

bool matchesRule(Rule rule, Part part)
{
    return rule.op switch
    {
        '<' => part.getPartProp(rule.var) < rule.value,
        '>' => part.getPartProp(rule.var) > rule.value,
        _ => throw new ArgumentOutOfRangeException(),
    };
}

string processPart(Part part, Dictionary<string, Workflow> system)
{
    string curr = "in";
    while(system.ContainsKey(curr))
    {
        Workflow wf = system[curr];
        curr = wf.Rules.Where(r => matchesRule(r, part)).Select(r => r.result).FirstOrDefault(wf.DefaultRule);
    }
    return curr;
}

void part1(string file_name)
{
    Dictionary<string, Workflow> system = new();

    string[] input = readFile(file_name).Split($"{Environment.NewLine}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries);
    List<Workflow> workflows = input[0].SplitLines().Select(s => new Workflow(s)).ToList();
    List<Part> parts = input[1].SplitLines().Select(p => new Part(p)).ToList();
    workflows.ForEach(wf => system.Add(wf.Name, wf));
    long answer = parts.Where(p => processPart(p, system) == "A").Select(p => p.Value).Sum();
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    Dictionary<string, Workflow> system = new();

    string[] input = readFile(file_name).Split($"{Environment.NewLine}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries);
    List<Workflow> workflows = input[0].SplitLines().Select(s => new Workflow(s)).ToList();
    workflows.ForEach(wf => system.Add(wf.Name, wf));
    long answer = 0;

    Queue<(string name, InputRange range)> frontier = new();
    frontier.Enqueue(new ("in", new((0, 4001), (0, 4001), (0, 4001), (0, 4001))));

    while(frontier.Count > 0)
    {
        (string name, InputRange range) process = frontier.Dequeue();
        if (process.name == "R") continue;

        if (process.name == "A")
        {
            answer += (process.range.ranges['x'].max - process.range.ranges['x'].min - 1) *
                      (process.range.ranges['m'].max - process.range.ranges['m'].min - 1) *
                      (process.range.ranges['a'].max - process.range.ranges['a'].min - 1) *
                      (process.range.ranges['s'].max - process.range.ranges['s'].min - 1);
            continue;
        }

        Workflow wf = system[process.name];
        InputRange range = process.range;
        foreach(Rule rule in wf.Rules)
        {
            var (match, compliment) = range.processRule(rule);
            frontier.Enqueue(new (rule.result, match));
            range = compliment;
        }
        frontier.Enqueue(new(wf.DefaultRule, range));
    }

    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");


class InputRange
{
    public Dictionary<char, (long min, long max)> ranges = new();

    public InputRange((long min, long max) x_range, (long min, long max) m_range, (long min, long max) a_range, (long min, long max) s_range)
    {
        ranges['x'] = new(x_range.min, x_range.max);
        ranges['m'] = new(m_range.min, m_range.max);
        ranges['a'] = new(a_range.min, a_range.max);
        ranges['s'] = new(s_range.min, s_range.max);
    }

    public InputRange(InputRange other) : this(other.ranges['x'], other.ranges['m'], other.ranges['a'], other.ranges['s']) { }

    public (InputRange match, InputRange compliment) processRule(Rule rule)
    {
        InputRange range_match = new(this);
        InputRange range_compliment = new(this);

        switch (rule.op)
        {
            case '<':
                range_match.ranges[rule.var] = (range_match.ranges[rule.var].min, rule.value);
                range_compliment.ranges[rule.var] = (rule.value - 1, range_compliment.ranges[rule.var].max);
                break;
            case '>':
                range_match.ranges[rule.var] = (rule.value, range_match.ranges[rule.var].max);
                range_compliment.ranges[rule.var] = (range_compliment.ranges[rule.var].min, rule.value + 1);
                break;
            default: throw new ArgumentOutOfRangeException();
        }

        return (range_match, range_compliment);
    }
}

class Workflow
{
    public string Name { get; set; }
    public List<Rule> Rules { get; set; }
    public string DefaultRule {  get; set; }

    private Rule parseRule(string str)
    {
        string pattern = @"(?<var>\w)(?<op>[<>])(?<value>\d+):(?<result>\w+)";
        var rule_match = Regex.Match(str, pattern);
        return new Rule(rule_match.get("var")[0], rule_match.get("op")[0], long.Parse(rule_match.get("value")), rule_match.get("result"));
    }

    public Workflow(string str)
    {
        string pattern = @"(?<workflow>\w+){(?<rules>.+)}";
        var workflow_match = Regex.Match(str, pattern);
        string[] rules_strings = workflow_match.get("rules").Split(',');

        Name = workflow_match.get("workflow");
        Rules = rules_strings.SkipLast(1).Select(parseRule).ToList();
        DefaultRule = rules_strings.Last();
    }

    public override string ToString()
    {
        StringBuilder sb = new($"Name: {Name}, DefaultRule: {DefaultRule}{Environment.NewLine}");
        Rules.ForEach(r => { sb.AppendLine($"  {r}"); });
        return sb.ToString();
    }
};

class Part
{
    public long X { get; set; }
    public long M { get; set; }
    public long A { get; set; }
    public long S { get; set; }
    public long Value { get { return X + M + A + S; } }

    public Part(string str)
    {
        List<long> values = str.ExtractLongs().ToList();
        X = values[0];
        M = values[1];
        A = values[2];
        S = values[3];
    }
    public long getPartProp(char prop)
    {
        return prop switch
        {
            'x' => X,
            'm' => M,
            'a' => A,
            's' => S,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}