using Library;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

using Rule = (char var, char op, long value, string result);
Dictionary<string, Workflow> system=new();

bool matchesRule(Rule rule, Part part)
{
    return rule.op switch
    {
        '<' => part.getPartProp(rule.var) < rule.value,
        '>' => part.getPartProp(rule.var) > rule.value,
        _ => throw new ArgumentOutOfRangeException(),
    };
}

string processPart(Part part)
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
    string[] input = readFile(file_name).Split($"{Environment.NewLine}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries);
    List<Workflow> workflows = input[0].SplitLines().Select(s => new Workflow(s)).ToList();
    List<Part> parts = input[1].SplitLines().Select(p => new Part(p)).ToList();
    workflows.ForEach(wf => system.Add(wf.Name, wf));
    long answer = parts.Where(p => processPart(p) == "A").Select(p => p.Value).Sum();
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

part1("sample.txt");
system.Clear();
part1("input.txt");

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