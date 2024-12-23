using System.Collections.Concurrent;
using static Library.Parsing;
using static Library.Testing;

Dictionary<long, Dictionary<long, long>> sequence_value_cache = new();

void next_secret(ref long secret)
{
    secret ^= secret << 6;
    secret &= 16777215;
    secret ^= secret >> 5;
    secret &= 16777215;
    secret ^= secret << 11;
    secret &= 16777215;
}

long evolve((long secret, long iterations) args)
{
    long original_secret = args.secret;
    sequence_value_cache[original_secret] = new();
    long prev = args.secret % 10;
    long hash = 0;
    for (long i = 0; i < args.iterations; ++i)
    {
        next_secret(ref args.secret);

        long bid = args.secret % 10;
        var diff = bid - prev + 10; // adjust range to 1..19
        hash = ((hash * 20) + diff) % 160000; // 19 * 8000 + 19 * 400 + 19 * 20 + 19 = 159999, anything greater is outside the window

        sequence_value_cache[original_secret].TryAdd(hash, bid);

        prev = bid;
    }
    return args.secret;
}

List<long> GenerateSecrets(string file_name)
{
    return readFileLines(file_name).ExtractLongs().Select(v => evolve((v.Single(), 2000L))).ToList();
}

long part1(string file_name)
{
    return GenerateSecrets(file_name).Sum();
}
long part2(string file_name)
{
    if(sequence_value_cache.Count == 0)
    {
        GenerateSecrets(file_name);
    }

    ConcurrentDictionary<long, long> summed_sequences = new();
    foreach(var dict in sequence_value_cache)
    {
        foreach(var (key, value) in dict.Value)
        {
            summed_sequences.AddOrUpdate(key, value, (k, v) => v + value);
        }
    }
    return summed_sequences.Max(kvp => kvp.Value);
}

test(evolve, "evolve", (123L, 1L), 15887950);
test(evolve, "evolve", (123L, 2L), 16495136);
test(evolve, "evolve", (123L, 3L), 527345);
test(evolve, "evolve", (123L, 4L), 704524);
test(evolve, "evolve", (123L, 5L), 1553684);
test(evolve, "evolve", (123L, 6L), 12683156);
test(evolve, "evolve", (123L, 7L), 11100544);
test(evolve, "evolve", (123L, 8L), 12249484);
test(evolve, "evolve", (123L, 9L), 7753432);
test(evolve, "evolve", (123L, 10L), 5908254);
test(evolve, "evolve", (1L, 2000L), 8685429);
test(evolve, "evolve", (10L, 2000L), 4700978L);
test(evolve, "evolve", (100L, 2000L), 15273692L);
test(evolve, "evolve", (2024L, 2000L), 8667524L);

sequence_value_cache.Clear();

test(part1, "part1", "sample.txt", 37327623);

sequence_value_cache.Clear();

test(part2, "part2", "sample2.txt", 23);

sequence_value_cache.Clear();

test(part1, "part1", "input.txt", 14622549304);
test(part2, "part2", "input.txt", 1735);