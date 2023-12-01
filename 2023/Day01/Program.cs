using Library;
using static Library.Parsing;

long toCalibrationValue(IEnumerable<long> nums) => (nums.First().ToString()[0] - '0') * 10 + nums.Last() % 10;

string replaceWordDigits(string line)
{
    (string text, string digit)[] replacers = [
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

    var first_word = replacers.Select(_ => (index: line.IndexOf(_.text), digit: _.digit))
        .Append((index: line.Length, digit: ""))
        .Where(_ => _.index != -1)
        .OrderBy(_ => _.index)
        .First();

    var last_word = replacers.Select(_ => (index: line.LastIndexOf(_.text), digit: _.digit))
        .Append((index: 0, digit: ""))
        .Where(_ => _.index != -1)
        .OrderByDescending(_ => _.index)
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