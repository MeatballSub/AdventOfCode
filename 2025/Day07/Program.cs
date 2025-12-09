using Library;
using static Library.Parsing;
using static Library.Testing;

void add_beam(Dictionary<Geometry.Point, long> beams, Geometry.Point key, long count)
{
    if (beams.ContainsKey(key))
    {
        beams[key] += count;
    }
    else
    {
        beams.Add(key, count);
    }
}

long solve(char[][] input, Func<long, Dictionary<Geometry.Point, long>, long> update_solution)
{
    var solution = 0L;
    var (_, start_loc) = input.find('S');
    Dictionary<Geometry.Point, long> beams = new() { { start_loc, 1 } };
    while (beams.Count > 0)
    {
        beams = beams.Where(b => b.Key.Y < input.Length - 1).ToDictionary();
        var classified_beams = beams.ToLookup(b => input.at(b.Key.Down()) == '^');
        var split_beams = classified_beams[true];
        var unsplit_beams = classified_beams[false];

        Dictionary<Geometry.Point, long> new_beams = new();
        foreach (var (beam, count) in split_beams)
        {
            add_beam(new_beams, beam.Left().Down(), count);
            add_beam(new_beams, beam.Right().Down(), count);
        }

        foreach (var (beam, count) in unsplit_beams)
        {
            add_beam(new_beams, beam.Down(), count);
        }

        solution = update_solution(solution, beams);
        beams = new_beams;
    }

    return solution;
}

long part1(string file_name)
{
    var input = readFileAsGrid(file_name);
    var update_solution = (long value, Dictionary<Geometry.Point, long> beams) => value + beams.Where(b => input.at(b.Key.Down()) == '^').ToHashSet().Count();
    return solve(input, update_solution);
}

long part2(string file_name)
{
    var input = readFileAsGrid(file_name);
    var update_solution = (long value, Dictionary<Geometry.Point, long> beams) => Math.Max(value, beams.Values.Sum());
    return solve(input, update_solution);
}

test(part1, "part1", "sample.txt", 21);
test(part1, "part1", "input.txt", 1555);

test(part2, "part2", "sample.txt", 40);
test(part2, "part2", "input.txt", 12895232295789);