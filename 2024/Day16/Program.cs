using static Library.Geometry;
using static Library.Parsing;
using System.Numerics;

Dictionary<string, (long min, int count)> solutions = new();

Dictionary<Facing, Func<Point, Point>> move_functions = new()
{
    { Facing.Up, p => p.Up() },
    { Facing.Right, p => p.Right() },
    { Facing.Down, p => p.Down() },
    { Facing.Left, p => p.Left() },
};

void getStartEnd(ref char[][]map, out Point? start, out Point? end)
{
    start = null;
    end = null;
    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == 'S') start = new Point(x, y);
            if (map[y][x] == 'E') end = new Point(x, y);
            if (start != null && end != null) return;
        }
    }
}

(char[][] map, Point start, Point end) parse(string file_name)
{
    var map = readFileAsGrid(file_name);
    getStartEnd(ref map, out var start, out var end);

    if (start == null) throw new Exception("Couldn't find start");
    if (end == null) throw new Exception("Couldn't find end");

    return (map, start, end);
}

List<Vertex> getNeighbors(Vertex vertex, ref char[][] map)
{
    List<Vertex> neighbors = new();

    foreach (Facing facing in Enum.GetValues(typeof(Facing)))
    {
        var new_location = move_functions[facing](vertex.location);
        if (new_location.boundsCheck(map) && map.at(new_location) != '#')
        {
            neighbors.Add(new Vertex() { facing = facing, location = new_location });
        }
    }

    return neighbors;
}

long getCost(Vertex vertex1, Vertex vertex2)
{
    if (vertex1.facing == vertex2.facing) return 1;

    long cost = Math.Abs(vertex1.facing - vertex2.facing) % 2;
    return (cost == 0) ? 2001 : 1001;
}

(long min, int count) solve(string file_name)
{
    if (solutions.TryGetValue(file_name, out var solution)) return solution;

    SpaghettiStack spaghetti_stack = new();
    Dictionary<Vertex, long> visited = new();
    HashSet<Point> locations = new();
    PriorityQueue<int, long> frontier = new();
    long? min = null;

    var input = parse(file_name);

    var validNeighbors = (int curr) =>
    {
        var curr_vertex = spaghetti_stack.Get(curr).vertex;
        return getNeighbors(curr_vertex, ref input.map).Where(n => !spaghetti_stack.Contains(curr, n.location));
    };

    var start_vertex = new Vertex()
    {
        facing = Facing.Right,
        location = input.start
    };

    int start_index = spaghetti_stack.Push(start_vertex, -1);

    frontier.Enqueue(start_index, 0);

    while (frontier.TryDequeue(out var curr, out var curr_cost))
    {
        if (min != null && min < curr_cost) break;

        var curr_vertex = spaghetti_stack.Get(curr).vertex;

        if (curr_vertex.location.Equals(input.end))
        {
            min = curr_cost;
            locations.UnionWith(spaghetti_stack.GetPath(curr));
            continue;
        }

        if (visited.TryGetValue(curr_vertex, out var visited_cost) && visited_cost < curr_cost) continue;

        visited[curr_vertex] = curr_cost;

        foreach (var neighbor in validNeighbors(curr))
        {
            int neighbor_index = spaghetti_stack.Push(neighbor, curr);
            long neighbor_cost = curr_cost + getCost(curr_vertex, neighbor);
            frontier.Enqueue(neighbor_index, neighbor_cost);
        }
    }

    solution = (min.GetValueOrDefault(-1), locations.Count);
    solutions.Add(file_name, solution);
    return solution;
}

long part1(string file_name)
{
    return solve(file_name).min;
}

int part2(string file_name)
{
    return solve(file_name).count;
}

void test<T>(Func<string, T> test_func, string func_name, string file_name, T expected) where T: IEqualityOperators<T,T, bool>
{
    var old_color = Console.ForegroundColor;
    var actual = test_func(file_name);
    if(actual == expected)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{func_name} - {file_name}: {actual}");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{func_name} - {file_name}[ actual: {actual} expected: {expected} ]");
    }
    Console.ForegroundColor = old_color;
}

test(part1, "part 1", "sample.txt", 7036);
test(part1, "part 1", "sample2.txt", 11048);
test(part1, "part 1", "input.txt", 143580);
test(part2, "part 2", "sample.txt", 45);
test(part2, "part 2", "sample2.txt", 64);
test(part2, "part 2", "input.txt", 645);


enum Facing
{
    Up,
    Right,
    Down,
    Left
}

struct Vertex: IEquatable<Vertex>
{
    public Point location;
    public Facing facing;

    public bool Equals(Vertex other)
    {
        return (location.Equals(other.location) && facing == other.facing);
    }
}

class SpaghettiStack
{
    private List<(Vertex vertex, int parent)> stack = new();

    public int Push(Vertex vertex, int parent)
    {
        int index = stack.Count;
        stack.Add((vertex, parent));
        return index;
    }

    public (Vertex vertex, int parent) Get(int index)
    {
        return stack[index];
    }

    public bool Contains(int index, Point p)
    {
        return GetPath(index).Contains(p);
    }

    public List<Point> GetPath(int index)
    {
        List<Point> path = new();
        while (index >= 0)
        {
            path.Add(stack[index].vertex.location);
            index = stack[index].parent;
        }
        return path;
    }
}