using Library;
using static Library.Parsing;

long toCalibrationValue(IEnumerable<long> nums) => (nums.First().ToString()[0] - '0') * 10 + nums.Last() % 10;

string replaceWordDigits(string line)
{
    (string, string)[] replacers = [
        ("zero", "0"),
        ("one", "1"),
        ("two", "2"),
        ("three", "3"),
        ("four", "4"),
        ("five", "5"),
        ("six", "6"),
        ("seven", "7"),
        ("eight", "8"),
        ("nine", "9")
    ];

    (int index, string digit) first_word = replacers.Select(_ => (line.IndexOf(_.Item1), _.Item2))
        .Append((line.Length, ""))
        .Where(_ => _.Item1 != -1)
        .OrderBy(_ => _.Item1)
        .First();

    (int index, string digit) last_word = replacers.Select(_ => (line.LastIndexOf(_.Item1), _.Item2))
        .Append((0, ""))
        .Where(_ => _.Item1 != -1)
        .OrderByDescending(_ => _.Item1)
        .First();

    return line.Substring(0, first_word.index) + first_word.digit + last_word.digit + line.Substring(last_word.index);
}

void run(string file_name)
{
    long sum = readFileLines(file_name).Select(_ => _.ExtractLongs()).Select(toCalibrationValue).Sum();
    Console.WriteLine($"Part 1 - {file_name}: {sum}");
}

void run2(string file_name)
{
    long sum = readFileLines(file_name).Select(replaceWordDigits).Select(_ => _.ExtractLongs()).Select(toCalibrationValue).Sum();
    Console.WriteLine($"Part 2 - {file_name}: {sum}");
}

run("sample1.txt");
run("input.txt");

run2("sample2.txt");
run2("input.txt");