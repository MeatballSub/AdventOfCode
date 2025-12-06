using Library;
using static Library.Parsing;
using static Library.Testing;

IEnumerable<Geometry.Point> validNeighbors(Geometry.Point loc, Dictionary<Geometry.Point, int> counts) =>
    loc.allNeighbors().ToHashSet().Where(counts.ContainsKey);

Dictionary<Geometry.Point, int> initCounts(char[][] grid)
{
    Dictionary<Geometry.Point, int> counts = new();
    for (int y = 0; y < grid.Length; ++y)
    {
        for (int x = 0; x < grid[y].Length; ++x)
        {
            Geometry.Point index = new(x, y);
            counts.Add(index, 0);
        }
    }
    grid.find_all('@').SelectMany(i => validNeighbors(i, counts)).ToList().ForEach(neighbor => ++counts[neighbor]);
    return counts;
}

IEnumerable<KeyValuePair<Geometry.Point, int>> getAccessible(char[][] grid, Dictionary<Geometry.Point, int> counts) =>
    counts.Where(kvp => kvp.Value < 4 && grid.at(kvp.Key) == '@');

long part1(string file_name)
{
    var input = readFileAsGrid(file_name);
    var counts = initCounts(input);
    return getAccessible(input, counts).Count();
}

long part2(string file_name)
{
    var input = readFileAsGrid(file_name);
    var counts = initCounts(input);
    var removeRoll = (KeyValuePair<Geometry.Point, int> kvp) =>
    {
        input[kvp.Key.Y][kvp.Key.X] = 'x';
        validNeighbors(kvp.Key, counts).ToList().ForEach(neighbor => --counts[neighbor]);
    };

    var solution = 0L;

    IEnumerable<KeyValuePair<Geometry.Point, int>> accessible;
    do
    {
        accessible = getAccessible(input, counts);
        solution += accessible.Count();
        accessible.ToList().ForEach(removeRoll);
    } while (accessible.Count() != 0);

    return solution;
}

test(part1, "part1", "sample.txt", 13);
test(part1, "part1", "input.txt", 1604);

test(part2, "part2", "sample.txt", 43);
test(part2, "part2", "input.txt", 9397);
