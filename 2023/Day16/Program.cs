using Library;
using System.Runtime.Intrinsics.Arm;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

using Beam = (Library.Geometry.Point loc, int dir);
int UP = 0;
int LEFT = 1;
int DOWN = 2;
int RIGHT = 3;

Dictionary<int, Move> moves = new()
{
    { UP, p => p.Up() },
    { LEFT, p => p.Left() },
    { DOWN, p => p.Down() },
    { RIGHT, p => p.Right() }
};

List<Beam> simBeam(Beam beam, string[] contraption)
{
    if (contraption[(int)beam.loc.Y][(int)beam.loc.X] == '.')
    {
        return new List<Beam>() { new Beam(moves[beam.dir](beam.loc), beam.dir) };
    }
    else if(contraption[(int)beam.loc.Y][(int)beam.loc.X] == '/')  // U -> R, L -> D, D -> L, R -> U
    {
        beam.dir = 3 - beam.dir;
        return new List<Beam>() { new Beam(moves[beam.dir](beam.loc), beam.dir) };
    }
    else if (contraption[(int)beam.loc.Y][(int)beam.loc.X] == '\\') // U -> L, L -> U, D -> R, R -> D
    {
        beam.dir += ((beam.dir % 2) == 0 ? 1 : -1);
        return new List<Beam>() { new Beam(moves[beam.dir](beam.loc), beam.dir) };
    }
    else if (contraption[(int)beam.loc.Y][(int)beam.loc.X] == '|') // U -> U, L -> {U, D}, D -> D, R -> {U, D}
    {
        if(beam.dir % 2 == 1)
        {
            return new List<Beam>() { new Beam(moves[0](beam.loc), 0), new Beam(moves[2](beam.loc), 2) };
        }
        else
        {
            return new List<Beam>() { new Beam(moves[beam.dir](beam.loc), beam.dir) };
        }
    }
    else // if (contraption[(int)beam.loc.Y][(int)beam.loc.X] == '-') // U -> {L, R}, L -> L, D -> {L, R}, R -> R
    {
        if (beam.dir % 2 == 0)
        {
            return new List<Beam>() { new Beam(moves[1](beam.loc), 1), new Beam(moves[3](beam.loc), 3) };
        }
        else
        {
            return new List<Beam>() { new Beam(moves[beam.dir](beam.loc), beam.dir) };
        }
    }
}

long solve(string file_name, Point start_point, int start_dir)
{
    string[] lines = readFileLines(file_name);

    List<List<List<int>>> energized = new();

    Enumerable.Range(0, lines.Length).ToList().ForEach(y =>
    {
        energized.Add(new List<List<int>>());
        Enumerable.Range(0, lines[y].Length).ToList().ForEach(x => energized[y].Add(new List<int>()));
    }
    );

    List<Beam> beams = new() { (start_point, start_dir) };

    var inBounds = (Point p) => 0 <= p.Y && p.Y < lines.Length && 0 <= p.X && p.X < lines[(int)p.Y].Length;

    long answer = 0;
    long count = 0;
    while (true)
    {
        List<Beam> newBeams = new();
        foreach (Beam beam in beams)
        {
            if (inBounds(beam.loc))
            {
                if (!energized[(int)beam.loc.Y][(int)beam.loc.X].Contains(beam.dir))
                {
                    energized[(int)beam.loc.Y][(int)beam.loc.X].Add(beam.dir);
                    newBeams.AddRange(simBeam(beam, lines));
                }
            }
        }
        beams = newBeams;
        long value = energized.Select(l => l.Count(_ => _.Count > 0)).Sum();
        if (value > answer)
        {
            count = 0;
            answer = value;
        }
        else
        {
            ++count;
            if (count > 999)
            {
                break;
            }
        }
    }

    return answer;
}

void part1(string file_name)
{
    long answer = solve(file_name, new Point(0,0), RIGHT);
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    string[] lines = readFileLines(file_name);
    long answer = 0;
    for (int i=0; i < lines.Length; i++)
    {
        long sub = solve(file_name, new Point(0, i), RIGHT);
        if(sub > answer)
        {
            answer = sub;
        }
        Console.WriteLine($"   Part 2 - {file_name}: {answer}");
    }
    for (int i = 0; i < lines.Length; i++)
    {
        long sub = solve(file_name, new Point(lines[i].Length - 1, i), LEFT);
        if (sub > answer)
        {
            answer = sub;
        }
        Console.WriteLine($"  Part 2 - {file_name}: {answer}");
    }
    for (int i = 0; i < lines[0].Length; i++)
    {
        long sub = solve(file_name, new Point(i, 0), DOWN);
        if (sub > answer)
        {
            answer = sub;
        }
        Console.WriteLine($" Part 2 - {file_name}: {answer}");
    }
    for (int i = 0; i < lines[0].Length; i++)
    {
        long sub = solve(file_name, new Point(i, lines.Length - 1), UP);
        if (sub > answer)
        {
            answer = sub;
        }
        Console.WriteLine($"Part 2 - {file_name}: {answer}");
    }
}

//part1("sample.txt");
//part1("input.txt");
//part2("sample.txt");
part2("input.txt");

delegate Point Move(Point p);