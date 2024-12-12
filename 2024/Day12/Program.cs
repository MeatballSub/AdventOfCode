using static Library.Parsing;
using static Library.Geometry;

List<(HashSet<Point> area, long perimeter)> parse(char[][] map)
{
    HashSet<Point> visited = new();
    List<(HashSet<Point>, long)> areas = new();
    for(int y = 0; y < map.Length; y++)
    {
        for(int x = 0; x < map[y].Length; x++)
        {
            Point key = new Point(x, y);
            if(!visited.Contains(key))
            {
                long perimeter = 0;
                HashSet<Point> area = new HashSet<Point>();
                Queue<Point> frontier = new Queue<Point>();
                frontier.Enqueue(key);
                while(frontier.TryDequeue(out Point curr))
                {
                    visited.Add(curr);
                    if (area.Add(curr))
                    {
                        foreach (var neighbor in curr.orthogonalNeighbors())
                        {
                            if (neighbor.boundsCheck(map) && (map.at(neighbor) == map.at(curr)))
                            {
                                frontier.Enqueue(neighbor);
                            }
                            else
                            {
                                ++perimeter;
                            }
                        }
                    }
                }
                areas.Add((area, perimeter));
            }
        }
    }
    return areas;
}

/*
 * Number of corners = number of sides
 * 
 * 4 corner types:
 * 
 *    -+      |     |      +-
 *     |     -+     +-     |
 *  
 *  You can be either on the inside of the corner or the outside:
 *  
 *   ---+       v|     |v       +---
 *   <<^|     <<v|     |v>>     |^>>
 *     ^|     ---+     +---     |^  
 * 
 * 
 *   <<<<^                         ^>>>>
 *   ---+^        |^     v|        ^+---
 *      |^        |^     v|        ^|   
 *      |^     ---+^     v+---     ^|  
 *             >>>>^     v>>>>
 *
 *  Inner corners are easy: if a horizontal neighbor and a vertical neighbor are missing you're the corner piece
 *  UR, LR, LL, UL
 *
 *  Outer corners: Harder to describe...you're looking for a hook around the corner, example:
 *  
 *    XX
 *    XA
 *    
 *    Neighbors exist both to the South and the East, but the Southeast is not part of your area
 *  
 */
long countSides(HashSet<Point> area, ref char[][] map)
{
    long sides = 0;
    foreach(var p in area)
    {
        // Inner UL
        // +-
        // |X
        if (!area.Contains(p.Up()) && !area.Contains(p.Left()))
        {
            ++sides;
        }

        // Inner UR
        // -+
        // X|
        if (!area.Contains(p.Up()) && !area.Contains(p.Right()))
        {
            ++sides;
        }

        // Inner LL
        // |X
        // +-
        if (!area.Contains(p.Down()) && !area.Contains(p.Left()))
        {
            ++sides;
        }

        // Inner LR
        // X|
        // -+
        if (!area.Contains(p.Down()) && !area.Contains(p.Right()))
        {
            ++sides;
        }

        // Outer UL
        // XXX
        // X+-
        // X|
        if (area.Contains(p.Down()) && area.Contains(p.Right()) && !area.Contains(p.Down().Right()))
        {
            ++sides;
        }

        // Outer UR
        // XXX
        // -+X
        //  |X
        if (area.Contains(p.Down()) && area.Contains(p.Left()) && !area.Contains(p.Down().Left()))
        {
            ++sides;
        }

        // Outer LL
        // X| 
        // X+-
        // XXX
        if (area.Contains(p.Up()) && area.Contains(p.Right()) && !area.Contains(p.Up().Right()))
        {
            ++sides;
        }

        // Outer LR
        //  |X
        // -+X
        // XXX
        if (area.Contains(p.Up()) && area.Contains(p.Left()) && !area.Contains(p.Up().Left()))
        {
            ++sides;
        }
    }
    return sides;
}

void part1(string file_name)
{
    var solution = 0L;
    var input = readFileAsGrid(file_name);
    var areas = parse(input);

    solution = areas.Select(a => a.area.Count * a.perimeter).Sum();

    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    var solution = 0L;
    var map = readFileAsGrid(file_name);
    var areas = parse(map);

    solution = areas.Select(a => a.area.Count * countSides(a.area, ref map)).Sum();

    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");