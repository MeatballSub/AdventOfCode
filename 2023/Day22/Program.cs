using Library;
using System.Buffers;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

Dictionary<Brick, List<Brick>> resting_on_me = new();
Dictionary<Brick, List<Brick>> resting_on = new();

void Show(List<Brick> bricks)
{

}

// 390 is too high
void part1(string file_name)
{
    Queue<Brick> bricks = new(readFileLines(file_name).Select(l => new Brick(l)).OrderBy(b => b.MinZ()));
    List<Brick> dropped_bricks = new();
    while(bricks.Count > 0)
    {
        Brick curr = bricks.Dequeue();
        //Console.WriteLine($"Dropping: {curr}");

        var sorted = dropped_bricks.OrderByDescending(b => b.MaxZ()).ToList();
        bool dropped = false;
        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i].DoesIntersect(curr))
            {
                if (!dropped)
                {
                    curr.Drop(sorted[i].MaxZ());
                    resting_on[curr] = new List<Brick>();
                    resting_on_me[curr] = new List<Brick>();
                    resting_on[curr].Add(sorted[i]);
                    resting_on_me[sorted[i]].Add(curr);
                    //Console.WriteLine($"  Resting on: {sorted[i]}");
                    dropped = true;
                }
                else if(curr.MinZ() - 1 == sorted[i].MaxZ())
                {
                    resting_on[curr].Add(sorted[i]);
                    resting_on_me[sorted[i]].Add(curr);
                    //Console.WriteLine($"  Resting on: {sorted[i]}");
                }
            }
        }
        if(!dropped)
        {
            curr.Drop(0);
            resting_on[curr] = new List<Brick>();
            resting_on_me[curr] = new List<Brick>();
        }

        //Console.WriteLine($"  Resting at: {curr}");

        dropped_bricks.Add(curr);

        Show(dropped_bricks);
    }
    long answer = 0;
    foreach (var brick in resting_on_me)
    {
        if(brick.Value.All(b => resting_on[b].Count > 1))
        {
            answer++;
        }
    }
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

long countFall(Brick brick)
{
    List<Brick> disintegrated = new() { brick };
    bool moreFound = true;
    while(moreFound)
    {
        moreFound = false;
        var notDisintegrated = (Brick b) => !disintegrated.Contains(b);
        var restingOnDisintegrated = (Brick b) => resting_on[b].All(_ => disintegrated.Contains(_));
        var filtered = resting_on.Where(_ => notDisintegrated(_.Key) && restingOnDisintegrated(_.Key) && resting_on[_.Key].Count > 0).ToList();
        moreFound = filtered.Count() > 0;
        disintegrated.AddRange(filtered.Select(_ => _.Key));
    }
    return disintegrated.Count() - 1;
}

void part2(string file_name)
{
    Queue<Brick> bricks = new(readFileLines(file_name).Select(l => new Brick(l)).OrderBy(b => b.MinZ()));
    List<Brick> dropped_bricks = new();
    while (bricks.Count > 0)
    {
        Brick curr = bricks.Dequeue();
        //Console.WriteLine($"Dropping: {curr}");

        var sorted = dropped_bricks.OrderByDescending(b => b.MaxZ()).ToList();
        bool dropped = false;
        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i].DoesIntersect(curr))
            {
                if (!dropped)
                {
                    curr.Drop(sorted[i].MaxZ());
                    resting_on[curr] = new List<Brick>();
                    resting_on_me[curr] = new List<Brick>();
                    resting_on[curr].Add(sorted[i]);
                    resting_on_me[sorted[i]].Add(curr);
                    //Console.WriteLine($"  Resting on: {sorted[i]}");
                    dropped = true;
                }
                else if (curr.MinZ() - 1 == sorted[i].MaxZ())
                {
                    resting_on[curr].Add(sorted[i]);
                    resting_on_me[sorted[i]].Add(curr);
                    //Console.WriteLine($"  Resting on: {sorted[i]}");
                }
            }
        }
        if (!dropped)
        {
            curr.Drop(0);
            resting_on[curr] = new List<Brick>();
            resting_on_me[curr] = new List<Brick>();
        }

        //Console.WriteLine($"  Resting at: {curr}");

        dropped_bricks.Add(curr);

        Show(dropped_bricks);
    }
    long answer = 0;
    foreach (var brick in resting_on_me)
    {
        answer += countFall(brick.Key);
    }
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
resting_on.Clear();
resting_on_me.Clear();
part1("input.txt");
resting_on.Clear();
resting_on_me.Clear();
part2("sample.txt");
resting_on.Clear();
resting_on_me.Clear();
part2("input.txt");

class Brick : IEquatable<Brick>
{
    Point3d a;
    Point3d b;
    public Brick(string str)
    {
        List<long> values = str.ExtractLongs().ToList();
        a = new(values[0], values[1], values[2]);
        b = new(values[3], values[4], values[5]);
    }

    public long MinZ() => Math.Min(a.Z, b.Z);

    public long MaxZ() => Math.Max(a.Z, b.Z);

    public void Drop(long support_z)
    {
        long drop_val = - support_z - 1 + ((a.Z < b.Z) ? a.Z : b.Z);
        a.Z -= drop_val;
        b.Z -= drop_val;
    }

    private bool OnSegment(Point3d a, Point3d b, Point3d c)
    {
        return b.X <= Math.Max(a.X, c.X) &&
               b.X >= Math.Min(a.X, c.X) &&
               b.Y <= Math.Max(a.Y, c.Y) &&
               b.Y >= Math.Min(a.Y, c.Y);
    }

    private int Orientation(Point3d a, Point3d b, Point3d c)
    {
        long val = (b.Y - a.Y) * (c.X - b.X) - (b.X - a.X) * (c.Y - b.Y);

        // 0 = colinear
        // 1 = clockwise oriented
        // 2 - counter clockwise oriented
        return (val == 0) ? 0 : (val > 0) ? 1 : 2;
    }

    public bool DoesIntersect(Brick brick)
    {
        // Find the four orientations needed for general and 
        // special cases 
        int o1 = Orientation(a, b, brick.a);
        int o2 = Orientation(a, b, brick.b);
        int o3 = Orientation(brick.a, brick.b, a);
        int o4 = Orientation(brick.a, brick.b, b);

        // General case 
        if (o1 != o2 && o3 != o4)
            return true;

        // Special Cases 
        // p1, q1 and p2 are collinear and p2 lies on segment p1q1 
        if (o1 == 0 && OnSegment(a, brick.a, b)) return true;

        // p1, q1 and q2 are collinear and q2 lies on segment p1q1 
        if (o2 == 0 && OnSegment(a, brick.b, b)) return true;

        // p2, q2 and p1 are collinear and p1 lies on segment p2q2 
        if (o3 == 0 && OnSegment(brick.a, a, brick.b)) return true;

        // p2, q2 and q1 are collinear and q1 lies on segment p2q2 
        if (o4 == 0 && OnSegment(brick.a, b, brick.b)) return true;

        return false; // Doesn't fall in any of the above cases 
    }

    public bool Equals(Brick? other) => (a == other.a && b == other.b);

    public override int GetHashCode() => HashCode.Combine(a.GetHashCode(), b.GetHashCode());
    public override string ToString()
    {
        return $"{a}~{b}";
    }
}