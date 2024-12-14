using static Library.Geometry;
using static Library.Parsing;
using static Library.LinqExtensions;
using System.Runtime.CompilerServices;

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

void part3(string file_name, long width, long height)
{
    var robots_start = parse(file_name).ToList();
    var robots = parse(file_name).ToList();
    List<double> x_dev_values = new();
    List<double> y_dev_values = new();
    double x_dev_sum = 0;
    double y_dev_sum = 0;
    double x_dev_avg = 0;
    double y_dev_avg = 0;

    for (long i = 1; i < 100; ++i)
    {
        robots = robots.AsParallel().Select(r => (moveRobot(r, width, height), r.velocity)).ToList();
        var x_dev = robots.Select(r => r.position.X).StdDev();
        var y_dev = robots.Select(r => r.position.Y).StdDev();

        x_dev_values.Add(x_dev);
        y_dev_values.Add(y_dev);
    }

    x_dev_sum = x_dev_values.Sum();
    y_dev_sum = y_dev_values.Sum();
    x_dev_avg = x_dev_values.Average();
    y_dev_avg = y_dev_values.Average();

    robots = new(robots_start);
    for (long i = 1; ; ++i)
    {
        robots = robots.AsParallel().Select(r => (moveRobot(r, width, height), r.velocity)).ToList();
        var x_dev = robots.Select(r => r.position.X).StdDev();
        var y_dev = robots.Select(r => r.position.Y).StdDev();

        double running_x_dev = Math.Sqrt(x_dev_values.Select(v => v - x_dev_avg).Select(v => v * v).Sum() / x_dev_values.Count);
        double running_y_dev = Math.Sqrt(y_dev_values.Select(v => v - y_dev_avg).Select(v => v * v).Sum() / y_dev_values.Count);

        // outlier?
        if(x_dev < x_dev_avg - running_x_dev * 2 || x_dev > x_dev_avg + running_x_dev * 2)
        {
            if (y_dev < y_dev_avg - running_y_dev * 2 || y_dev > y_dev_avg + running_y_dev * 2)
            {
                Console.WriteLine($"part 3 - {file_name}: {i}");
                var bitmap = new System.Drawing.Bitmap((int)width, (int)height);
                foreach (var robot in robots)
                {
                    bitmap.SetPixel((int)robot.position.X, (int)robot.position.Y, System.Drawing.Color.Red);
                }
                bitmap.Save($"{i}.bmp");
                break;
            }
        }

        x_dev_values.Add(x_dev);
        y_dev_values.Add(y_dev);
        x_dev_sum += x_dev;
        y_dev_sum += y_dev;
        x_dev_avg = x_dev_sum / i;
        y_dev_avg = y_dev_sum / i;
    }
}

part1("sample.txt", 11, 7);
part1("input.txt", 101, 103);

//part2("input.txt", 101, 103);

part3("input.txt", 101, 103);

static class Day14
{
    public static double StdDev(this IEnumerable<long> values)
    {
        var avg = values.Average();
        return Math.Sqrt(values.Select(v => v - avg).Select(v => v * v).Sum() / (double)values.Count());
    }
}