using Library;
using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

long start_time = Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond;
long timeSinceStart() => (Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond) - start_time;

PuzzleState? solve(PuzzleState start)
{
    Graph g = new();
    try
    {
        Search<PuzzleState, long>(g, start);
    }
    catch (GoalFoundException)
    {
        return g.best_state;
    }
    return null;
}

List<PuzzleState> solveBlueprints(IEnumerable<string> blueprints, int time) =>
    blueprints.Select(_ => new PuzzleState(time, _))
              .Select(solve)
              .ToList();

void part1(string file_name)
{
    long sum = solveBlueprints(readFileLines(file_name), 24).Select(_ => _.Score() * _.blueprint).Sum();
    Console.WriteLine(sum);
    Console.WriteLine($"time: {timeSinceStart()}");
}

void part2(string file_name)
{
    long product = solveBlueprints(readFileLines(file_name).Take(3), 32).Select(_ => _.Score()).Product();
    Console.WriteLine(product);
    Console.WriteLine($"time: {timeSinceStart()}");
}

part1("sample.txt"); // ~  2 seconds
part1("input.txt");        // ~ 12 seconds
part2("sample.txt"); // ~ 99 seconds
part2("input.txt");        // ~ 65 seconds

class PuzzleState : IEquatable<PuzzleState>
{
    private const int GEODE = 3;

    public int minutes_left { get; set; }
    public int blueprint { get; init; }
    public List<long> resources { get; set; } = new() { 0, 0, 0, 0 };
    public List<long> robots { get; set; } = new() { 1, 0, 0, 0 };
    List<List<long>> costs { get; init; } = new();
    List<long> max_costs { get; init; } = new();

    public PuzzleState(int minutes_left, string blueprint_desc)
    {
        this.minutes_left = minutes_left;
        var matches = Regex.Matches(blueprint_desc, @"\d+").Select(_ => int.Parse(_.Value)).ToList();
        blueprint = matches[0];
        costs.Add(new List<long> { matches[1], 0, 0, 0 });
        costs.Add(new List<long> { matches[2], 0, 0, 0 });
        costs.Add(new List<long> { matches[3], matches[4], 0, 0 });
        costs.Add(new List<long> { matches[5], 0, matches[6], 0 });
        max_costs = Enumerable.Range(0, costs.Count).Select(t => costs.Select(_ => _[t]).Max()).ToList();
    }

    public PuzzleState(PuzzleState other)
    {
        this.minutes_left = other.minutes_left;
        this.blueprint = other.blueprint;
        this.resources = other.resources.ToList();
        this.robots = other.robots.ToList();
        this.costs = other.costs;
        this.max_costs = other.max_costs;
    }

    public long Score() => resources[GEODE] + minutes_left * robots[GEODE];

    // pretend we can build a geode robot every minute
    private long BestPossibleScore() => Score() + ((minutes_left * (minutes_left - 1)) / 2);

    public bool IsBetterThan(PuzzleState other) => Score() > other.Score();

    public bool IsPotentiallyBetterThan(PuzzleState other) => BestPossibleScore() > other.Score();

    private List<long> GetTotalResources() => resources.Select((v, i) => v + robots[i] * minutes_left).ToList();

    private bool HaveEnoughToBuild(int type) => GetTotalResources().Select((v, i) => v >= costs[type][i]).All(_ => _);

    private bool EnoughOf(int type) => type != GEODE && robots[type] >= max_costs[type];

    private void WaitForBuild()
    {
        var IntDivRoundUp = (long numer, long denom) => (numer + denom - 1) / denom;

        int minutes_needed = 1 + resources.Select((v, i) => v < 0 ? (int)IntDivRoundUp(-v, robots[i]) : 0).Max();
        minutes_left -= minutes_needed;
        resources = resources.Select((v, i) => v + robots[i] * minutes_needed).ToList();
    }

    private void PayForRobot(List<long> cost) => resources = resources.Select((v, i) => v - cost[i]).ToList();

    private PuzzleState? TryBuild(int type)
    {
        if (!HaveEnoughToBuild(type) || EnoughOf(type))
        {
            return null;
        }

        PuzzleState puzzle_state = new PuzzleState(this);
        puzzle_state.PayForRobot(costs[type]);
        puzzle_state.WaitForBuild();
        puzzle_state.robots[type]++;
        return puzzle_state;
    }

    public List<PuzzleState> Neighbors() => Enumerable.Range(0, robots.Count).Select(TryBuild).Where(_ => _ != null).ToList();

    public bool Equals(PuzzleState? other) => other != null && minutes_left == other.minutes_left && resources.SequenceEqual(other.resources) && robots.SequenceEqual(other.robots);

    public override int GetHashCode()
    {
        HashCode code = new();
        code.Add(minutes_left);
        resources.ForEach(code.Add);
        robots.ForEach(code.Add);
        return code.ToHashCode();
    }

    public long Priority() => BestPossibleScore();
}

class GoalFoundException : Exception
{
}

class Graph : BaseGraph<PuzzleState, long>
{
    public PuzzleState? best_state=null;
    public Graph() { }

    public override long cost(PuzzleState vertex1, PuzzleState vertex2) => 0;

    public override List<PuzzleState> neighbors(PuzzleState vertex) => vertex.Neighbors();

    public override long heuristic(PuzzleState vertex)
    {
        return -vertex.Priority();
    }

    public override void visit(PuzzleState vertex)
    {
        if(best_state == null || vertex.IsBetterThan(best_state))
        {
            best_state = vertex;
        }
        if(!vertex.IsPotentiallyBetterThan(best_state))
        {
            throw new GoalFoundException();
        }
    }

    public override long getBestCost(PuzzleState vertex) => 0;
    public override void setBestCost(PuzzleState vertex, long bestCost) { }
    public override bool containsBestCostFor(PuzzleState vertex) => false;
    public override void setPredecessor(PuzzleState vertex, PuzzleState predecessor) { }

}

