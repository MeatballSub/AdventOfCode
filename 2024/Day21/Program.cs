using static Library.Geometry;
using static Library.Parsing;
using static Library.Testing;
using static Library.Optimize;
using static Library.GraphExtensions;
using System.Linq;
using System.Reflection.Emit;

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
Dictionary<(string path, int level), long> dirpad_encoding_cache_iter = new();

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

/*
 * Challenged myself to make this an iterative solve rather than recursion
 */
long minDirpadEncodingIter(string numpad_path, int dirpad_count)
{
    Stack<(string path, int level)> stack = new([(numpad_path, dirpad_count)]);

    while (stack.TryPop(out var curr))
    {
        if (dirpad_encoding_cache_iter.ContainsKey(curr)) continue;

        if (curr.level == 0)
        {
            dirpad_encoding_cache_iter[curr] = curr.path.Length;
            continue;
        }

        var to_paths = ((Point prev, Point next) pair) => getPaths(ref dirpad_map, pair.prev, pair.next);
        var get_unsolved = (List<string> paths) => paths.Select(p => (p, curr.level - 1)).Where(key => !dirpad_encoding_cache_iter.ContainsKey(key));
        var get_min_encoding = (List<string> paths) => paths.Select(p => dirpad_encoding_cache_iter[(p, curr.level - 1)]).Min();

        var dirpad_points = "A".Concat(curr.path).Select(c => findButton(ref dirpad_map, c));
        var dirpad_path_list = dirpad_points.Zip(dirpad_points.Skip(1)).Select(to_paths);

        var unsolved_sub_problems = dirpad_path_list.SelectMany(get_unsolved);

        if (unsolved_sub_problems.Count() > 0)
        {
            foreach (var problem in unsolved_sub_problems.Prepend(curr))
            {
                stack.Push(problem);
            }
        }
        else
        {
            dirpad_encoding_cache_iter[curr] = dirpad_path_list.Select(get_min_encoding).Sum();
        }
    }

    return dirpad_encoding_cache_iter[(numpad_path, dirpad_count)];
}

long minNumpadEncodingIter(string sequence, int dirpad_count)
{
    var minEncoding = ((Point prev, Point next) pair) => getPaths(ref numpad_map, pair.prev, pair.next).Select(p => minDirpadEncodingIter(p, dirpad_count)).Min();
    var points = "A".Concat(sequence).Select(c => findButton(ref numpad_map, c));
    return points.Zip(points.Skip(1)).Select(minEncoding).Sum();
}

long solveIter((string file_name, int dir_pad_count) args)
{
    var get_length = (string l) => minNumpadEncodingIter(l, args.dir_pad_count);
    var get_numeric = (string l) => int.Parse(l.Substring(0, l.Length - 1));
    var get_complexity = (string l) => get_length(l) * get_numeric(l);

    return readFileLines(args.file_name).Select(get_complexity).Sum();
}

//test(solve, "part1", ("sample.txt", 2), 126384);
//test(solve, "part1", ("input.txt", 2), 278568);

//test(solve, "part2", ("sample.txt", 25), 154115708116294);
//test(solve, "part2", ("input.txt", 25), 341460772681012);

test(solveIter, "part1", ("sample.txt", 2), 126384);
test(solveIter, "part1", ("input.txt", 2), 278568);

test(solveIter, "part2", ("sample.txt", 25), 154115708116294);
test(solveIter, "part2", ("input.txt", 25), 341460772681012);

class Graph : BaseGraph<Point, long>
{
    private char[][] map;
    public Graph(char[][] map) => this.map = map;

    public override long cost(Point vertex1, Point vertex2) => 1;

    public override List<Point> neighbors(Point vertex) => vertex.orthogonalNeighbors().Where(n => n.boundsCheck(map) && map.at(n) != ' ').ToList();
}