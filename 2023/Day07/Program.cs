using static Library.Parsing;

using HandData = ((long type, string value) hand, long bid);

string translate(string hand, char j_value) => hand.Replace('J', j_value).Replace('Q', 'V').Replace('K', 'W').Replace('A', 'X');

long handType(string hand)
{
    var groups = hand.GroupBy(_ => _).OrderByDescending(_ => _.Count());
    HandType value;

    if (groups.Count() == 1) value = HandType.FiveOfAKind;
    else if (groups.Count() == 5) value = HandType.HighCard;
    else if (groups.Count() == 4) value = HandType.OnePair;
    else if (groups.Count() == 3) value = (groups.First().Count() == 2) ? HandType.TwoPair : HandType.ThreeOfAKind;
    else value = (groups.First().Count() == 4) ? HandType.FourOfAKind : HandType.FullHouse;

    return (long)value;
}

string part2Preprocess(string hand) =>
    (hand == "JJJJJ") ? hand
                      : hand.Replace('J', hand.Where(_ => _ != 'J').GroupBy(_ => _).OrderByDescending(_ => _.Count()).First().Key);

HandData mangle1(string[] s) => (hand: (type: handType(s[0]), value: translate(s[0], 'U')), bid: long.Parse(s[1]));
HandData mangle2(string[] s) => (hand: (type: handType(part2Preprocess(s[0])), value: translate(s[0], '1')), bid: long.Parse(s[1]));

IEnumerable<string[]> parse(string file_name) => readFileLines(file_name).Select(_ => _.Split(' '));

long getWinnings(IEnumerable<HandData> hand_data) => hand_data.OrderBy(_ => _.hand).Select((h, i) => h.bid * (i + 1)).Sum();

void part1(string file_name)
{
    long ans = getWinnings(parse(file_name).Select(_ => mangle1(_)));
    Console.WriteLine($"Part 1 - {file_name}: {ans}");
}

void part2(string file_name)
{
    long ans = getWinnings(parse(file_name).Select(_ => mangle2(_)));
    Console.WriteLine($"Part 2 - {file_name}: {ans}");
}

part1("sample.txt");
part1("input.txt");
part2("sample.txt");
part2("input.txt");

enum HandType
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind,
}