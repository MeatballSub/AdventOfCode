using Library;
using static Library.Geometry;
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

    while (beams.Count > 0)
    {
        List<Beam> newBeams = new();
        foreach (Beam beam in beams)
        {
            if (inBounds(beam.loc))
            {
                List<int> loc_energized = energized[(int)beam.loc.Y][(int)beam.loc.X];
                if (!loc_energized.Contains(beam.dir)) 
                {
                    loc_energized.Add(beam.dir);
                    newBeams.AddRange(simBeam(beam, lines));
                }
            }
        }
        beams = newBeams;
    }

    return energized.Select(l => l.Count(c => c.Count > 0)).Sum();
}

void part1(string file_name)
{
    long answer = solve(file_name, new Point(0,0), RIGHT);
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    string[] lines = readFileLines(file_name);
    List<long> scores = new();
    for (int i=0; i < lines.Length; i++)
    {
        scores.Add(solve(file_name, new Point(0, i), RIGHT));
        scores.Add(solve(file_name, new Point(lines[i].Length - 1, i), LEFT));
    }
    for (int i = 0; i < lines[0].Length; i++)
    {
        scores.Add(solve(file_name, new Point(i, 0), DOWN));
        scores.Add(solve(file_name, new Point(i, lines.Length - 1), UP));
    }
    long answer = scores.Max();
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");

delegate Point Move(Point p);