using Library;
using static Library.Parsing;

using MapEntry = (long dest, long source, long len);

MapEntry asMapEntry(string str)
{
    var values = str.ExtractLongs().ToList();
    return new MapEntry(values[0], values[1], values[2]);
}

List<MapEntry> asMapEntries(string arr)
{
    return arr.SplitLines().Skip(1).Select(asMapEntry).OrderBy(_ => _.source).ToList();
}

List<MapEntry> MakeSeedRangePart1(string str)
{
    return str.ExtractLongs().Select(_ => new MapEntry(_, _, 1)).OrderBy(_ => _.dest).ToList();
}

List<MapEntry> MakeSeedRangePart2(string str)
{
    List<MapEntry> result = new();

    List<long> seeds = str.ExtractLongs().ToList();
    for (int i = 0; i < seeds.Count; i += 2)
    {
        result.Add((seeds[i], seeds[i], seeds[i + 1]));
    }

    return result.OrderBy(_ => _.dest).ToList();
}

List<MapEntry> remapRange(List<MapEntry> range, List<MapEntry> map)
{
    List<MapEntry> result = new();
    // range is sorted by dest, map is sorted by source, so we have some early out possibilities below
    foreach (MapEntry range_entry in range)
    {
        int count = result.Count;
        long range_start = range_entry.dest;
        long range_end = range_entry.dest + range_entry.len - 1;
        long covered_offset = 0;
        foreach (MapEntry map_entry in map)
        {
            long map_start = map_entry.source;
            long map_end = map_entry.source + map_entry.len - 1;
            long map_offset = map_entry.dest - map_entry.source;

            // map range is too low move to next
            if(range_start > map_end)
            {
                continue;
            }

            // map range is too high, because sorted, no other map range will match, quit looking
            if (map_start > range_end)
            {
                break;
            }

            // Range:   |---------|
            //   Map:        |---------|
            // Because map extends past range, and maps are sorted, no later maps will overlap quit looking
            if (range_start <= map_start && map_start <= range_end && range_end <= map_end)
            {
                //            1    2
                // Range:   |----|----|
                //   Map:        |---------|
                result.Add((range_entry.dest + covered_offset, range_entry.source + covered_offset, map_start - range_start));
                result.Add((range_entry.dest + covered_offset + map_offset, range_entry.source + covered_offset, range_end - map_start + 1));
                break;
            }

            // Range:      |---|
            //   Map:   |---------|
            // Because map extends past range, and maps are sorted, no later maps will overlap quit looking
            if (map_start <= range_start && range_end <= map_end)
            {
                result.Add((range_entry.dest + covered_offset + map_offset, range_entry.source + covered_offset, range_entry.len));
                break;
            }

            // Range:   |---------|
            //   Map:      |---|
            // Because range extends past map, we have to keep looking, but change range start and how much of range is covered
            if (range_start <= map_start && map_end <= range_end)
            {
                //           1   2
                // Range:   |--|---|--|
                //   Map:      |---|
                result.Add((range_entry.dest + covered_offset, range_entry.source + covered_offset, map_start - range_start));
                result.Add((range_entry.dest + covered_offset + map_offset, range_entry.source + covered_offset, map_end - map_start + 1));
                range_start = map_end + 1;
                covered_offset += range_start - range_entry.dest;
            }
            // Range:        |---------|
            //   Map:   |---------|
            // Because range extends past map, we have to keep looking, but change range start and how much of range is covered
            else if (map_start <= range_start && range_start <= map_end && map_end <= range_end)
            {
                //                 1
                // Range:        |----|----|
                //   Map:   |---------|
                result.Add((range_entry.dest + covered_offset + map_offset, range_entry.source + covered_offset, map_end - range_start + 1));
                range_start = map_end + 1;
                covered_offset += range_start - range_entry.dest;
            }
        }

        // We didn't add anything to the result(because no map ranges overlapped), add the whole range
        if(result.Count == count)
        {
            result.Add(range_entry);
        }
        else
        {
            // see if range was partially covered, add the uncovered part
            long last_end = result.Last().source + result.Last().len - 1;
            if(last_end < range_entry.source + range_entry.len - 1)
            {
                long last_offset = last_end - range_entry.source + 1;
                result.Add((range_entry.dest + last_offset, range_entry.source + last_offset, range_entry.len - result.Last().len));
            }
        }
    }
    // filter out the zero length ranges and make sure it's sorted by dest
    return result.Where(_ => _.len != 0).OrderBy(_ => _.dest).ToList();
}

(List<MapEntry> range, List<List<MapEntry>> maps_list) parse(string file_name, MakeSeedRange makeSeedRange)
{
    string[] fields = readFile(file_name).Split($"{Environment.NewLine}{Environment.NewLine}");
    List<MapEntry> range = makeSeedRange(fields[0]);
    List<List<MapEntry>> maps_list = fields.Skip(1).Select(asMapEntries).ToList();

    return (range, maps_list);
}

long process(string file_name, MakeSeedRange makeSeedRange)
{
    (List<MapEntry> range, List<List<MapEntry>> maps_list) = parse(file_name, makeSeedRange);
    foreach (List<MapEntry> map in maps_list)
    {
        range = remapRange(range, map);
    }
    return range.First().dest;
}

void part1(string file_name)
{
    long answer = process(file_name, MakeSeedRangePart1);
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    long answer = process(file_name, MakeSeedRangePart2);
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");

public delegate List<MapEntry> MakeSeedRange(string str);