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

void FitAndExtrapolate(List<Point> points, long X)
{

}

long solve(char[][] grid, Point start, long steps)
{
    long count = 0;
    long show_me = 0;
    HashSet<Point> visited = new();
    //HashSet<Point> reachable = new();
    Queue<(Point, long)> frontier = new();
    List<Point> edge_counts = new();
    frontier.Enqueue((start, 0));

    while(frontier.Count > 0)
    {
        (Point p, long taken) = frontier.Dequeue();
        if (grid.modAt(p) == '#' || visited.Contains(p) || taken > steps) continue;
        visited.Add(p);
        if(p.Y % grid.Length == -1 && p.Y < show_me)
        {
            show_me = p.Y;
            edge_counts.Add(new Point(taken - 1, count));
            if (edge_counts.Count > 1)
            {
                FitAndExtrapolate(edge_counts, steps);
            }
            Console.WriteLine($"taken: {taken - 1}, count: {count}");
        }
        if(taken % 2 == steps % 2)
        {
            //reachable.Add(p);
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

void part1(string file_name, long steps, long expected)
{
    Console.OutputEncoding = Encoding.UTF8;
    char[][] grid = readFileLines(file_name).Select(l => l.ToCharArray()).ToArray();
    Point start = FindStart(grid);
    long count = solve(grid, start, steps);
    Console.WriteLine($"Part 1 - {file_name}: {count} {(count == expected ? "\u2705" : "\u274c")}");
}

// 623131417303078 is too high
void part2(string file_name, long steps, long expected)
{
    Console.OutputEncoding = Encoding.UTF8;
    char[][] grid = readFileLines(file_name).Select(l => l.ToCharArray()).ToArray();
    Point start = FindStart(grid);
    long count = solve(grid, start, steps);
    Console.WriteLine($"Part 2 - {file_name}: {count} {(count == expected ? "\u2705" : "\u274c")}");
}

//part1("sample.txt", 1, 2);
//part1("sample.txt", 2, 4);
//part1("sample.txt", 3, 6);
//part1("sample.txt", 4, 9);
//part1("sample.txt", 5, 13);
//part1("sample.txt", 6, 16);
//part1("input.txt", 64, 3699);
//part2("sample.txt", 6, 16);
//part2("sample.txt", 10, 50);
//part2("sample.txt", 50, 1594);
//part2("sample.txt", 100, 6536);
//part2("sample.txt", 500, 167004);
//part2("sample.txt", 1000, 668697);
//part2("sample.txt", 5000, 16733044);
//part2("sample.txt", 15, 115);
part2("input.txt", 26501365, 0);

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