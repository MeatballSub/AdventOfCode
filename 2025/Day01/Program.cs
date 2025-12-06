using System.Diagnostics;
using static Library.Parsing;
using static Library.Testing;

(char dir, long dist) parseRotation(string line)
{
    return (line[0], long.Parse(line.Substring(1)));
}

long normalizeRotation((char dir, long dist) rotation)
{
    long d = rotation.dist % 100;
    return rotation.dir == 'R' ? d : 100 - d;
}

long part1(string file_name)
{
    var input = readFileLines(file_name);

    var rotations = input.Select(parseRotation).Select(normalizeRotation);

    long value = 50;
    long count = 0;

    foreach (var rotation in rotations)
    {
        value = (value + rotation) % 100;
        if (value == 0) ++count;
    }

    return count;
}

long part2(string file_name)
{
    var input = readFileLines(file_name);
    var rotations = input.Select(parseRotation);

    long old_value = 50;
    long value = 50;
    long count = 0;

    foreach (var rotation in rotations)
    {
        value = (value + normalizeRotation(rotation)) % 100;

        count += rotation.dist / 100;
        if (rotation.dir == 'R' && value < old_value) ++count;
        if (rotation.dir == 'L' && ((value > old_value && old_value != 0) || value == 0)) ++count;

        old_value = value;
    }

    return count;
}

test(part1, "part1", "sample.txt", 3);
test(part1, "part1", "input.txt", 1081);

test(part2, "part2", "sample.txt", 6);
test(part2, "part2", "input.txt", 6689);
