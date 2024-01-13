using Library;
using System.Runtime.InteropServices;
using System.Text;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

Point FindStart(char[][] grid)
{
    for(int y = 0 ; y < grid.Length; y++)
    {
        for(int x = 0;  x < grid[y].Length; x++)
        {
            if (grid[y][x] == 'S')
            {
                return new Point(x, y);
            }
        }
    }
    throw new IndexOutOfRangeException();
}

long solve1(char[][] grid, Point start, long steps)
{
    long count = 0;
    HashSet<Point> visited = new();
    HashSet<Point> reachable = new();
    Queue<(Point, long)> frontier = new();
    List<Point> edge_counts = new();
    frontier.Enqueue((start, 0));

    while(frontier.Count > 0)
    {
        (Point p, long taken) = frontier.Dequeue();
        if (grid.modAt(p) == '#' || visited.Contains(p) || taken > steps) continue;
        visited.Add(p);
        if(taken % 2 == steps % 2)
        {
            reachable.Add(p);
            ++count;
        }
        p.orthogonalNeighbors().ToList().ForEach(neighbor => frontier.Enqueue((neighbor, taken + 1)));
    }

    //for (int y = -grid.Length; y < 2 * grid.Length; y++)
    //{
    //    for (int x = -grid.Length; x < 2 * grid.Length; x++)
    //    {
    //        if (reachable.Contains(new Point(x, y)))
    //        {
    //            Console.Write('O');
    //        }
    //        else
    //        {
    //            Console.Write(grid.modAt(new Point(x,y)));
    //        }
    //    }
    //    Console.WriteLine();
    //}

    return count;
}

long countGrid(char [][] grid, HashSet<Point> reached, Point min)
{
    Point max = new Point(min.X + grid.Length, min.Y + grid.Length);
    HashSet<Point> subset = reached.Where(p => min.X <= p.X && p.X < max.X && min.Y <= p.Y && p.Y < max.Y).ToHashSet();

    //char[][] reachedGrid = new char[grid.Length][];
    //for(int y = 0; y < grid.Length; ++y)
    //{
    //    reachedGrid[y] = new char[grid.Length];
    //    for(int x = 0; x < grid.Length; ++x)
    //    {
    //        reachedGrid[y][x] = grid[y][x];
    //    }
    //}

    //foreach(Point p in subset)
    //{
    //    reachedGrid.modAt(p) = 'O';
    //}

    //Console.WriteLine();
    //for (int y = 0; y < reachedGrid.Length; ++y)
    //{
    //    for (int x = 0; x < reachedGrid.Length; ++x)
    //    {
    //        Console.Write(reachedGrid[y][x]);
    //    }
    //    Console.WriteLine();
    //}
    //Console.WriteLine();

    return subset.Count;
}

long solve2(char[][] grid, Point start, long steps)
{
    // taking advantage of input:
    // * start at center
    // * steps is an exact multiple of grid.Length + steps to center
    // * that exact multiple is even
    long evens = ((steps - start.X) / grid.Length) - 1;
    long odds = evens + 1;
    long min_steps = grid.Length * 4 + start.X;

    HashSet<Point> visited = new();
    HashSet<Point> reachable = new();
    Queue<(Point, long)> frontier = new();
    List<Point> edge_counts = new();
    frontier.Enqueue((start, 0));
        
    while (frontier.Count > 0)
    {
        (Point p, long taken) = frontier.Dequeue();
        if (grid.modAt(p) == '#' || visited.Contains(p) || taken > min_steps) continue;
        visited.Add(p);
        if (taken % 2 == min_steps % 2)
        {
            reachable.Add(p);
        }
        p.orthogonalNeighbors().ToList().ForEach(neighbor => frontier.Enqueue((neighbor, taken + 1)));
    }

    Dictionary<string, Point> gridTL = new()
    {
        { "even full", new Point(0, 0) },
        { "odd full", new Point(0, grid.Length) },
        { "NE outer", new Point(grid.Length * 2, -grid.Length * 3) },
        { "SE outer", new Point(grid.Length * 2, grid.Length * 3) },
        { "SW outer", new Point(-grid.Length * 2, grid.Length * 3) },
        { "NW outer", new Point(-grid.Length * 2, -grid.Length * 3) },
        { "NE inner", new Point(grid.Length * 2, -grid.Length * 2) },
        { "SE inner", new Point(grid.Length * 2, grid.Length * 2) },
        { "SW inner", new Point(-grid.Length * 2, grid.Length * 2) },
        { "NW inner", new Point(-grid.Length * 2, -grid.Length * 2) },
        { "N tip", new Point(0, -grid.Length * 4) },
        { "E tip", new Point(grid.Length * 4, 0) },
        { "S tip", new Point(0, grid.Length * 4) },
        { "W tip", new Point(-grid.Length * 4, 0) },
    };

    long full_count = evens * evens * countGrid(grid, reachable, gridTL["even full"]) +
                       odds *  odds * countGrid(grid, reachable, gridTL["odd full"]) +
                               odds * countGrid(grid, reachable, gridTL["NE outer"]) +
                               odds * countGrid(grid, reachable, gridTL["SE outer"]) +
                               odds * countGrid(grid, reachable, gridTL["SW outer"]) +
                               odds * countGrid(grid, reachable, gridTL["NW outer"]) +
                              evens * countGrid(grid, reachable, gridTL["NE inner"]) +
                              evens * countGrid(grid, reachable, gridTL["SE inner"]) +
                              evens * countGrid(grid, reachable, gridTL["SW inner"]) +
                              evens * countGrid(grid, reachable, gridTL["NW inner"]) +
                                      countGrid(grid, reachable, gridTL["N tip"]) +
                                      countGrid(grid, reachable, gridTL["E tip"]) +
                                      countGrid(grid, reachable, gridTL["S tip"]) +
                                      countGrid(grid, reachable, gridTL["W tip"]);

    //Console.WriteLine($"evens = {evens}, odds = {odds}");
    return full_count;
}

void part1(string file_name, long steps, long expected)
{
    Console.OutputEncoding = Encoding.UTF8;
    char[][] grid = readFileLines(file_name).Select(l => l.ToCharArray()).ToArray();
    Point start = FindStart(grid);
    long count = solve1(grid, start, steps);
    Console.WriteLine($"Part 1 - {file_name}: {count} {(count == expected ? "\u2705" : "\u274c")}");
}

void part2(string file_name, long steps, long expected)
{
    Console.OutputEncoding = Encoding.UTF8;
    char[][] grid = readFileLines(file_name).Select(l => l.ToCharArray()).ToArray();
    Point start = FindStart(grid);
    long count = solve2(grid, start, steps);
    Console.WriteLine($"Part 2 - {file_name}: {count} {(count == expected ? "\u2705" : "\u274c")}");
}

part1("sample.txt", 1, 2);
part1("sample.txt", 2, 4);
part1("sample.txt", 3, 6);
part1("sample.txt", 4, 9);
part1("sample.txt", 5, 13);
part1("sample.txt", 6, 16);
part1("input.txt", 64, 3699);
part2("input.txt", 26501365, 613391294577878);

static class Extensions
{
    public static ref char at(this char[][] arr, Point p) => ref arr[p.Y][p.X];
    //public static ref char modAt(this char[][] arr, Point p) => ref arr[p.Y % arr.Length][p.X % arr[p.Y % arr.Length].Length];

    public static ref char modAt(this char[][] arr, Point p)
    {
        long y = p.Y % arr.Length;
        if (y < 0) y += arr.Length;
        long x = p.X % arr[y].Length;
        if (x < 0) x += arr[y].Length;
        return ref arr[y][x];
    }

    public static bool inBounds(this char[][] arr, Point p) => 0 <= p.Y && p.Y < arr.Length && 0 <= p.X && p.X < arr[p.Y].Length;
}