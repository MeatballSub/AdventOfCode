using Library;
using static Library.Parsing;
using static Library.Testing;

(List<Line> polygon_segments, List<(Geometry.Point a, Geometry.Point b, long area)> rectangles) parse(string file_name)
{
    var input = readFileLines(file_name);
    var tiles = input.Select(l => l.Split(',')).Select(p => new Geometry.Point(long.Parse(p[0]), long.Parse(p[1]))).ToList();
    List<Line> polygon_segments = new();
    for (int i = 0; i < tiles.Count; ++i)
    {
        var prev = tiles[(i + tiles.Count - 1) % tiles.Count];
        var curr = tiles[i];
        polygon_segments.Add(new Line(prev, curr));
    }

    List<(Geometry.Point a, Geometry.Point b, long area)> rectangles = new();
    for (int i = 0; i < tiles.Count; ++i)
    {
        for (int j = i + 1; j < tiles.Count; ++j)
        {
            var diff = tiles[i] - tiles[j];
            var area = (Math.Abs(diff.X) + 1) * (Math.Abs(diff.Y) + 1);
            rectangles.Add((tiles[i], tiles[j], area));
        }
    }

    rectangles = rectangles.OrderByDescending(r => r.area).ToList();

    return (polygon_segments, rectangles);
}

bool point_in_polygon(Geometry.Point p, List<Line> polygon_segments)
{
    var intersects = (Line seg) => (seg.a.Y > p.Y) != (seg.b.Y > p.Y) &&
                                   (p.X < (seg.b.X - seg.a.X) * (p.Y - seg.a.Y) / (seg.b.Y - seg.a.Y) + seg.a.X);

    var on_poly = (Line seg) => Math.Min(seg.a.Y, seg.b.Y) <= p.Y &&
                                p.Y <= Math.Max(seg.a.Y, seg.b.Y) &&
                                Math.Min(seg.a.X, seg.b.X) <= p.X &&
                                p.X <= Math.Max(seg.a.X, seg.b.X);

    return polygon_segments.Where(intersects).Count() % 2 == 1 || polygon_segments.Any(on_poly);
}

bool corners_inscribed(Geometry.Point a, Geometry.Point b, List<Line> polygon_segments)
{
    return point_in_polygon(new Geometry.Point(a.X, b.Y), polygon_segments) &&
           point_in_polygon(new Geometry.Point(b.X, a.Y), polygon_segments);
}

// like a perimeter check, but only checking at potential problem points
bool poi_inscribed(Geometry.Point a, Geometry.Point b, List<Line> polygon_segments)
{
    var polygon_verticals = polygon_segments.Where(seg => seg.a.X == seg.b.X);
    var polygon_horizontals = polygon_segments.Where(seg => seg.a.Y == seg.b.Y);

    var (min_x, max_x, min_y, max_y) = (Math.Min(a.X, b.X), Math.Max(a.X, b.X), Math.Min(a.Y, b.Y), Math.Max(a.Y, b.Y));

    var poi_v = polygon_verticals.SelectMany(l => new List<Geometry.Point>() { new Geometry.Point(l.a.X, a.Y), new Geometry.Point(l.a.X, b.Y) })
                                 .SelectMany(p => new List<Geometry.Point>() { p.Left(), p.Right() })
                                 .Where(p => min_x <= p.X && p.X <= max_x);

    var poi_h = polygon_horizontals.SelectMany(l => new List<Geometry.Point>() { new Geometry.Point(a.X, l.a.Y), new Geometry.Point(b.X, l.a.Y) })
                                   .SelectMany(p => new List<Geometry.Point>() { p.Up(), p.Down() })
                                   .Where(p => min_y <= p.Y && p.Y <= max_y);

    var poi = poi_v.Union(poi_h).ToHashSet();

    return poi.All(p => point_in_polygon(p, polygon_segments));
}

bool is_inscribed(Geometry.Point a, Geometry.Point b, List<Line> polygon_segments)
{
    return corners_inscribed(a, b, polygon_segments) && poi_inscribed(a, b, polygon_segments);
}

long part1(string file_name)
{
    var (_, rectangles) = parse(file_name);
    return rectangles.First().area;
}

long part2(string file_name)
{
    var solution = 0L;
    var (polygon_segments, rectangles) = parse(file_name);
    return rectangles.First(r => is_inscribed(r.a, r.b, polygon_segments)).area;
}

test(part1, "part1", "sample.txt", 50);
test(part1, "part1", "input.txt", 4745816424);

test(part2, "part2", "sample.txt", 24);
test(part2, "part2", "input.txt", 1351617690);

public record Line(Geometry.Point a, Geometry.Point b);