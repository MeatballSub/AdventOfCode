using static Library.Geometry;
using static Library.Parsing;
using static Library.LinqExtensions;

IEnumerable<(Point position, Point velocity)> parse(string file_name)
{
    return readFileLines(file_name).Select(l => l.ExtractSignedLongs()).Select(r =>  (new Point(r.First(), r.Skip(1).First()), new Point(r.Skip(2).First(), r.Last())));
}

Point moveRobot((Point position, Point velocity) robot, long width, long height, long times = 1)
{
    Point total_velocity = new Point(times * robot.velocity.X, times * robot.velocity.Y);
    Point newPos = robot.position + total_velocity;
    newPos.X = (newPos.X % width + width) % width;
    newPos.Y = (newPos.Y % height + height) % height;
    return newPos;
}

void part1(string file_name, long width, long height)
{
    var solution = 0L;
    var robots = parse(file_name);
    Dictionary<(bool, bool), long> quadrants = new()
    {
        { (false, false), 0 },
        { (false, true), 0 },
        { (true, false), 0 },
        { (true, true), 0 },
    };

    var final_positions = robots.Select(r => moveRobot(r, width, height, 100));

    foreach(var position in final_positions)
    {
        if (position.X != (width / 2) && position.Y != (height / 2))
        {
            bool x_half = position.X > (width / 2);
            bool y_half = position.Y > (height / 2);
            quadrants[(x_half, y_half)]++;
        }
    }

    solution = quadrants.Values.Product();

    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name, long width, long height)
{
    bool slow_version = false;

    var robots = parse(file_name);

    for(long i = 1; ; ++i)
    {
        robots = robots.Select(r => (moveRobot(r, width, height), r.velocity));

        if (slow_version || (i == 6532))  // I had to do the slow version to figure out the answer
        {
            var bitmap = new System.Drawing.Bitmap((int)width, (int)height);
            foreach (var robot in robots)
            {
                bitmap.SetPixel((int)robot.position.X, (int)robot.position.Y, System.Drawing.Color.Red);
            }
            bitmap.Save($"{i}.bmp");
            if (!slow_version)
            {
                break;
            }
        }
    }
}

part1("sample.txt", 11, 7);
part1("input.txt", 101, 103);

part2("input.txt", 101, 103);