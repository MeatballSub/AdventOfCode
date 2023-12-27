using Library;
using System.Numerics;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

// Stoer-Wagner
(string source, string sink) stMinCut(Graph g)
{
    PriorityQueue<string, long> frontier = new();
    
    foreach(string v in g.Vertices)
    {
        g.setBestCost(v, 0);
        frontier.Enqueue(v, 0);
    }

    string? source = null;
    string? sink = null;

    HashSet<string> visited = new();

    while(frontier.Count > 0)
    {
        string curr = frontier.Dequeue();
        if (visited.Contains(curr))
        {
            continue;
        }
        visited.Add(curr);
        source = sink;
        sink = curr;
        foreach (string neighbor in g.neighbors(curr))
        {
            if (!visited.Contains(neighbor))
            {
                long best_cost = g.getBestCost(neighbor) + g.cost(curr, neighbor);
                g.setBestCost(neighbor, best_cost);
                frontier.Enqueue(neighbor, -best_cost);
            }
        }
    }
    return (source, sink);
}

Graph MinCut(Graph g)
{
    if(g.Vertices.Count == 2)
    {
        g.sink = g.Vertices.First();
        return g;
    }

    Graph g1 = new Graph(g);
    (string source, string sink) = stMinCut(g1);
    g1.sink = sink;

    Graph g2 = new Graph(g1);
    g2.MergeVertices(source, sink);
    g2 = MinCut(g2);

    return (g1.Weight() <= g2.Weight()) ? g1 : g2;
}

Graph parse(string file_name)
{
    Graph g = new();
    string[] lines = readFileLines(file_name);
    foreach (string line in lines)
    {
        string[] component_connections = line.Split(':');
        string component = component_connections[0];
        string[] connections = component_connections[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (string connection in connections)
        {
            g.AddEdge(component, connection);
        }
    }
    return g;
}

// 561180 is too low
void part1(string file_name)
{
    Graph g = parse(file_name);
    long answer = g.Vertices.Count;
    g = MinCut(g);
    string[] sink_side = g.sink.Split(':');
    answer -= sink_side.Count();
    answer *= sink_side.Count();

    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    string[] lines = readFileLines(file_name);
    long answer = 0;
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
part1("input.txt");
//part2("sample.txt");
//part2("input.txt");

class Graph : BaseGraph<string, long>
{
    public HashSet<string> Vertices = new();
    Dictionary<string, Dictionary<string, long>> Edges = new();
    public string sink = "";

    public Graph() { }

    public Graph(Graph other)
    {
        Vertices = other.Vertices.ToHashSet();
        Edges = other.Edges.ToDictionary();
        sink = "";
        best_costs.Clear();
    }

    public void AddEdge(string v1, string v2)
    {
        if(!Edges.ContainsKey(v1))
        {
            Edges[v1] = new();
        }

        if (!Edges.ContainsKey(v2))
        {
            Edges[v2] = new();
        }

        Edges[v1][v2] = 1;
        Edges[v2][v1] = 1;
        Vertices.Add(v1);
        Vertices.Add(v2);
    }

    public void MergeVertices(string vertex1, string vertex2)
    {
        string new_name = vertex1 + ':' + vertex2;
        Vertices.Remove(vertex1);
        Vertices.Remove(vertex2);
        Vertices.Add(new_name);

        Edges[new_name] = new();
        foreach (string neighbor in neighbors(vertex1).Union(neighbors(vertex2)))
        {
            if (!neighbor.Equals(vertex1) && !neighbor.Equals(vertex2))
            {
                long weight = 0;

                if (Edges[vertex1].ContainsKey(neighbor))
                {
                    weight += Edges[vertex1][neighbor];
                }

                if (Edges[vertex2].ContainsKey(neighbor))
                {
                    weight += Edges[vertex2][neighbor];
                }

                Edges[neighbor].Remove(vertex1);
                Edges[neighbor].Remove(vertex2);
                Edges[new_name][neighbor] = weight;
                Edges[neighbor][new_name] = weight;
            }
        }
        Edges.Remove(vertex1);
        Edges.Remove(vertex2);
    }

    public long Weight() => Edges[sink].Select(_ => _.Value).Sum();

    private long Weight(HashSet<string> component1, HashSet<string> component2) =>
        Edges.Where(a => component1.Contains(a.Key)).Sum(a => a.Value.Where(b => component2.Contains(b.Key)).Sum(b => b.Value));

    public override long cost(string vertex1, string vertex2) => Edges[vertex1][vertex2];

    public override List<string> neighbors(string vertex) => (Edges.ContainsKey(vertex)) ? Edges[vertex].Select(_ => _.Key).ToList() : new();
}