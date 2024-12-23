using System.Collections.Concurrent;
using static Library.Parsing;
using static Library.Testing;

(HashSet<string> computers, ConcurrentDictionary<string, List<string>> connections) parse(string file_name)
{
    HashSet<string> computers = new();
    ConcurrentDictionary<string, List<string>> connections = new();
    var lines = readFileLines(file_name).Select(l => l.Split('-', StringSplitOptions.RemoveEmptyEntries));
    foreach(var line in lines)
    {
        var c1 = line.First();
        var c2 = line.Last();
        var c1_list = new List<string>() { c1 };
        var c2_list = new List<string>() { c2 };
        connections.AddOrUpdate(c1, c2_list, (k, v) => v.Append(c2).ToList());
        connections.AddOrUpdate(c2, c1_list, (k, v) => v.Append(c1).ToList());
        computers.Add(c1);
        computers.Add(c2);
    }
    return (computers, connections);
}

HashSet<string> BronKerbosch(HashSet<string> R, HashSet<string> P, HashSet<string> X, ConcurrentDictionary<string, List<string>> connections)
{
    if (P.Count == 0 && X.Count == 0)
    {
        return R;
    }

    HashSet<string> max = new();
    foreach (var v in P)
    {
        var candidate = BronKerbosch(
            R.Union(new List<string>() { v }).ToHashSet(),
            P.Intersect(connections[v]).ToHashSet(),
            X.Intersect(connections[v]).ToHashSet(),
            connections);

        if (candidate.Count > max.Count) max = candidate;

        P.Remove(v);
        X.Add(v);
    }

    return max;
}

long part1(string file_name)
{
    var (computers, connections) = parse(file_name);
    var t_computers = computers.Where(c => c.StartsWith('t'));

    HashSet<(string, string, string)> three_clique = new();

    foreach(var computer in t_computers)
    {
        for(int i = 0; i < connections[computer].Count; ++i)
        {
            var c1 = connections[computer].ElementAt(i);
            for(int j = i + 1; j < connections[computer].Count; ++j)
            {
                var c2 = connections[computer].ElementAt(j);
                if (connections[c1].Contains(c2))
                {
                    List<string> clique = new() { computer, c1, c2 };
                    clique.Sort();
                    three_clique.Add((clique[0], clique[1], clique[2]));
                }
            }
        }
    }

    return three_clique.Count;
}

string part2(string file_name)
{
    var (computers, connections) = parse(file_name);

    HashSet<string> R = new();
    HashSet<string> P = new(computers);
    HashSet<string> X = new();

    var LAN_party = BronKerbosch(R, P, X, connections);

    return string.Join(',', LAN_party.Order());
}

test(part1, "part1", "sample.txt", 7);
test(part1, "part1", "input.txt", 1306);

test(part2, "part2", "sample.txt", "co,de,ka,ta");
test(part2, "part2", "input.txt", "bd,dk,ir,ko,lk,nn,ob,pt,te,tl,uh,wj,yl");