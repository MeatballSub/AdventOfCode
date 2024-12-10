using static Library.Parsing;
using static Library.Geometry;

(char[][] map, List<Point> trailheads) parse(string file_name)
{
    var input = readFileAsGrid(file_name);
    List<Point> trailheades = new();

    for (int y = 0; y < input.Length; y++)
    {
        for (int x = 0; x < input[y].Length; x++)
        {
            if(input[y][x] == '0')
            {
                trailheades.Add(new Point(x, y));
            }
        }
    }

    return (input, trailheades);
}

List<Point> score(char[][] map, Point trailhead)
{
    List<Point> peaks = new();
    Stack<Point> frontier = new();
    frontier.Push(trailhead);
    while (frontier.Count != 0)
    {
        var curr = frontier.Pop();
        
        if (map.at(curr) == '9')
        {
            peaks.Add(curr);
        }
        else
        {
            var validNeighbor = (Point n) => n.boundsCheck(map) && map.at(n) == map.at(curr) + 1;
            foreach (var neighbor in curr.orthogonalNeighbors().Where(validNeighbor))
            {
                frontier.Push(neighbor);
            }
        }
    }
    return peaks;
}

long solve(string file_name, ScoreFunc score_func)
{
    long solution = 0;
    var (map, trailheads) = parse(file_name);

    foreach (var trailhead in trailheads)
    {
        solution += score_func(map, trailhead);
    }

    return solution;
}

void part1(string file_name)
{
    ScoreFunc score_func = (char[][] map, Point trailhead) => score(map, trailhead).ToHashSet().Count;
    var solution = solve(file_name, score_func);

    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    ScoreFunc score_func = (char[][] map, Point trailhead) => score(map, trailhead).Count;
    var solution = solve(file_name, score_func);

    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");

delegate int ScoreFunc(char[][] map, Point trailhead);