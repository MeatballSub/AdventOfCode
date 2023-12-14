using Library;
using System.Text;
using static Library.Geometry;
using static Library.Parsing;

Dictionary<int, Move> moveMap = new()
{
    { 0, p => p.Up() },
    { 1, p => p.Left() },
    { 2, p => p.Down() },
    { 3, p => p.Right() },
};

HashableList tiltOne(HashableList lines, int dir, int x, int y)
{
    Point location = new Point(x, y);
    if (lines[location] == 'O')
    {
        Point newLoc = moveMap[dir](location);

        var inBounds = (Point p) => (0 <= p.Y && p.Y < lines.Count && 0 <= p.X && p.X < lines[(int)p.Y].Length);

        while (inBounds(newLoc) && lines[newLoc] == '.')
        {
            newLoc = moveMap[dir](newLoc);
        }
        newLoc = moveMap[(dir + 2) % 4](newLoc);

        lines[location] = '.';
        lines[newLoc] = 'O';
    }
    return lines;
}

HashableList tiltLowToHigh(HashableList lines, int dir)
{
    for (int y = 0; y < lines.Count; y++)
    {
        for (int x = 0; x < lines[y].Length; x++)
        {
            lines = tiltOne(lines, dir, x, y);
        }
    }
    return lines;
}

HashableList tiltHighToLow(HashableList lines, int dir)
{    
    for (int y = lines.Count - 1; y >= 0; y--)
    {
        for (int x = lines[y].Length - 1; x >= 0; x--)
        {
            lines = tiltOne(lines, dir, x, y);
        }
    }
    return lines;
}

HashableList tilt(HashableList lines, int dir) => (dir < 2) ? tiltLowToHigh(lines, dir) : tiltHighToLow(lines, dir);

long getLoad(HashableList lines) => lines.Select((line, index) => line.ToString().Count(c => c == 'O') * (lines.Count - index)).Sum();

void part1(string file_name)
{
    HashableList lines = new HashableList(readFileLines(file_name).Select(_ => new StringBuilder(_)).ToList());
    long answer = getLoad(tilt(lines, 0));
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    int iterations = 1000000000;
    Dictionary<HashableList, int> state_index_map = new();
    HashableList lines = new (readFileLines(file_name).Select(_ => new StringBuilder(_)).ToList());
    state_index_map.Add(lines, 0);
    long first_index = 0;
    long first_repeat = 0;

    for(int i = 0; i < iterations; ++i)
    {
        for(int j = 0; j < 4; ++j)
        {
            lines = tilt(lines, j);
        }

        try
        {
            state_index_map.Add(new HashableList(lines), i + 1);
        }
        catch
        {
            first_index = state_index_map[lines];
            first_repeat = i+1;
            break;
        }
    }

    long offset = (iterations - first_repeat) % (first_repeat - first_index);
    long index = first_index + offset;

    lines = state_index_map.Where(_ => _.Value == index).First().Key;
    long answer = getLoad(lines);
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");

delegate Point Move(Point p);

class HashableList : List<StringBuilder>, IEquatable<HashableList>
{
    public HashableList(List<StringBuilder> list) => list.ForEach(_ => this.Add(new StringBuilder(_.ToString())));

    public bool Equals(HashableList? other) => this.Select(_ => _.ToString()).SequenceEqual(other.Select(_ => _.ToString()));

    public override int GetHashCode()
    {
        int code = 0;
        this.ForEach(_ => code = HashCode.Combine(code, _.ToString().GetHashCode()));
        return code;
    }

    public char this[Point p]
    {
        get => this[(int)p.Y][(int)p.X];
        set => this[(int)p.Y][(int)p.X] = value;
    }
}