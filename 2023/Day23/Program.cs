using Library;
using System.Numerics;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

//using VertexType = (Library.Geometry.Point loc, Library.Geometry.Point prev);

void part1(string file_name)
{
    string[] lines = readFileLines(file_name);
    Graph g = new(lines);
    //g.goalTest = v => v.loc.Y == g.height - 1;
    VertexType start = new (new Point(lines[0].IndexOf('.'), 0), null);
    VertexType goal = new(new Point(lines[g.height - 1].IndexOf('.'), g.height - 1), null);
    long answer = 0;
    Search(g, start);
    g.showPath(goal);
    answer = g.getBestCost(goal);
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

part1("sample.txt");
//part1("input.txt");

class GoalFoundException(VertexType value) : Exception
{
    public VertexType Value { get; init; } = value;
}

//class LongestIsBetterCost: IComparable<LongestIsBetterCost>,
//                           IAdditiveIdentity<LongestIsBetterCost, LongestIsBetterCost>,
//                           IAdditionOperators<LongestIsBetterCost, LongestIsBetterCost, LongestIsBetterCost>
//{
//    public long steps = 0;
//    public LongestIsBetterCost(long s) => steps = s;

//    public static LongestIsBetterCost AdditiveIdentity => new(142*142);

//    public int CompareTo(LongestIsBetterCost? other)
//    {
//        return (other.steps > steps) ? 1 : (other.steps == steps) ? 0 : -1;
//    }

//    public static LongestIsBetterCost operator +(LongestIsBetterCost left, LongestIsBetterCost right)
//    {
//        return new(left.steps + right.steps);
//    }
//}

class VertexType:IEquatable<VertexType>
{
    public Point loc;
    public Point prev;

    public VertexType(Point l, Point p)
    {
        loc = l;
        prev = p;
    }

    public bool Equals(VertexType? other)
    {
        return other.loc.Equals(loc);
    }

    public override int GetHashCode()
    {
        return loc.GetHashCode();
    }
}

class Graph : BaseGraph<VertexType, long>
{
    //public delegate bool GoalTest(VertexType location);
    //public delegate List<VertexType> GetNeighbors(VertexType location);

    //public GoalTest goalTest;
    //public GetNeighbors getNeighbors;

    public static Dictionary<int, Move> moves = new()
    {
        { 0, p => p.Up() },
        { 1, p => p.Left() },
        { 2, p => p.Down() },
        { 3, p => p.Right() }
    };

    public string[] grid;
    public long height = -1;
    public long width = -1;

    public Graph(string[] lines)
    {
        grid = lines;
        height = lines.Count();
        width = lines[0].Length;
    }

    public override long cost(VertexType vertex1, VertexType vertex2) => -1;

    //public override long heuristic(VertexType vertex) => 208 + getBestCost(new VertexType(vertex.prev, null));

    public override List<VertexType> neighbors(VertexType vertex)
    {
        List<VertexType> result = new ();
        //if (grid.at(vertex.loc) == '.')
        {
            var inBounds = (Point p) => 0 <= p.Y && p.Y < height && 0 <= p.X && p.X < width;
            for (int i = 0; i < 4; ++i)
            {
                Point new_loc = moves[i](vertex.loc);
                if (inBounds(new_loc) && grid.at(new_loc) != '#' && !new_loc.Equals(vertex.prev))
                {
                    result.Add(new(new_loc, vertex.loc));
                }
            }
        }
        //else
        //{
        //    Point new_loc = grid.at(vertex.loc) switch
        //    {
        //        '^' => moves[0](vertex.loc),
        //        '<' => moves[1](vertex.loc),
        //        'v' => moves[2](vertex.loc),
        //        '>' => moves[3](vertex.loc),
        //    };
        //    if (!new_loc.Equals(vertex.prev))
        //    {
        //        result.Add(new(new_loc, vertex.loc));
        //    }
        //}
        return result;
    }

    //public override void visit(VertexType vertex)
    //{
    //    if (goalTest(vertex))
    //    {
    //        throw new GoalFoundException(vertex);
    //    }
    //}

    public void showPath(VertexType p)
    {
        char[][] char_grid = grid.Select(_ => _.ToCharArray()).ToArray();
        while (p.loc.Y != 0)
        {
            char_grid.at(p.loc) = 'O';
            p = predecessors[p];
        }
        for (int y = 0; y < char_grid.Count(); y++)
        {
            for(int x = 0; x < char_grid[y].Length; x++)
            {
                Console.Write(char_grid[y][x]);
            }
            Console.WriteLine();
        }
    }
};

public static class extensions
{
    public static char at(this string[] arr, Point p)
    {
        return arr[(int)p.Y][(int)p.X];
    }
    public static ref char at(this char[][] arr, Point p)
    {
        return ref arr[(int)p.Y][(int)p.X];
    }
}

delegate Point Move(Point p);