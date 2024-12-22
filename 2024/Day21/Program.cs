using static Library.Geometry;
using static Library.Parsing;
using static Library.Testing;
using static Library.Optimize;
using static Library.GraphExtensions;

char[][] numpad_map =
{
    "789".ToCharArray(),
    "456".ToCharArray(),
    "123".ToCharArray(),
    " 0A".ToCharArray()
};

char[][] dirpad_map = {
    " ^A".ToCharArray(),
    "<v>".ToCharArray(),
};

Dictionary<Func<Point, Point>, char> direction_to_char = new()
{
    { p => p.Left(), '<' },
    { p => p.Up(), '^' },
    { p => p.Right(), '>' },
    { p => p.Down(), 'v' },
};

Dictionary<(string path, int level), long> dirpad_encoding_cache = new();

List<string> buildPaths(List<List<Point>> point_paths)
{
    var convert_dir_to_char = ((Point prev, Point next) pair) => direction_to_char.First(kvp => pair.next.Equals(kvp.Key(pair.prev))).Value;
    return point_paths.Select(point_path => new String(point_path.Zip(point_path.Skip(1)).Select(convert_dir_to_char).ToArray()) + 'A').ToList();
}

List<string> getPaths(ref char[][] map, Point start, Point end)
{
    var point_paths = new Graph(map).GetAllShortestPaths(start, end);
    return buildPaths(point_paths);
}

Point findButton(ref char[][] map, char button)
{
    var result = map.find(button);
    if (result.found) return result.location;
    throw new Exception($"Couldn't find button: '{button}'");
}

long minDirpadEncoding(string path, int level)
{
    var key = (path, level);
    if (dirpad_encoding_cache.TryGetValue(key, out var length)) return length;

    if (level == 0) return path.Length;

    var minEncoding = ((Point prev, Point next) pair) => getPaths(ref dirpad_map, pair.prev, pair.next).Select(p => minDirpadEncoding(p, level - 1)).Min();
    var points = path.Select(c => findButton(ref dirpad_map, c)).Prepend(findButton(ref dirpad_map, 'A'));
    length = points.Zip(points.Skip(1)).Select(minEncoding).Sum();

    dirpad_encoding_cache.Add(key, length);
    return length;
}

long minNumpadEncoding(string sequence, int level)
{
    var minEncoding = ((Point prev, Point next) pair) => getPaths(ref numpad_map, pair.prev, pair.next).Select(p => minDirpadEncoding(p, level)).Min();
    var points = sequence.Select(c => findButton(ref numpad_map, c)).Prepend(findButton(ref numpad_map, 'A'));
    return points.Zip(points.Skip(1)).Select(minEncoding).Sum();
}

long solve((string file_name, int dir_pad_count) args)
{
    var get_length = (string l) => minNumpadEncoding(l, args.dir_pad_count);
    var get_numeric = (string l) => int.Parse(l.Substring(0, l.Length - 1));
    var get_complexity = (string l) => get_length(l) * get_numeric(l);

    return readFileLines(args.file_name).Select(get_complexity).Sum();
}

test(solve, "part1", ("sample.txt", 2), 126384);
test(solve, "part1", ("input.txt", 2), 278568);

test(solve, "part2", ("sample.txt", 25), 154115708116294);
test(solve, "part2", ("input.txt", 25), 341460772681012);

class Graph : BaseGraph<Point, long>
{
    private char[][] map;
    public Graph(char[][] map) => this.map = map;

    public override long cost(Point vertex1, Point vertex2) => 1;

    public override List<Point> neighbors(Point vertex) => vertex.orthogonalNeighbors().Where(n => n.boundsCheck(map) && map.at(n) != ' ').ToList();
}