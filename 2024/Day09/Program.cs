using static Library.Geometry;
using static Library.Parsing;

const long FREE_SPACE = -1;

void add_func1(ref List<long> mem_blocks, long block_id, int length)
{
    for(int i = 0; i < length; ++i)
    {
        mem_blocks.Add(block_id);
    }
}

void add_func2(ref List<(long id, int length)> mem_blocks, long block_id, int length)
{
    mem_blocks.Add((block_id, length));
}

T parse<T>(string file_name, AddFunc<T> add_func) where T:new()
{
    string input = readFile(file_name);
    T mem_blocks = new();
    long file_id = 0;
    for (int i = 0; i < input.Length;)
    {
        int length = input[i] - '0';
        ++i;

        long mode = i % 2;
        long block_id = ((0 == mode) ? FREE_SPACE : file_id);
        add_func(ref mem_blocks, block_id, length);
        file_id += mode;
    }
    return mem_blocks;
}

int findOpening(List<(long id, int length)> input, int last_block)
{
    int opening_index = 0;
    while (opening_index < last_block && (input[opening_index].id != -1 || input[opening_index].length < input[last_block].length))
    {
        ++opening_index;
    }
    return opening_index;
}

void moveFile(ref List<(long id, int length)> input, int opening_index, int last_block)
{
    input[opening_index] = (input[opening_index].id, input[opening_index].length - input[last_block].length);
    var save = input[last_block];
    input[last_block] = (-1, input[last_block].length);
    input.Insert(opening_index, save);
}

long checkSum(List<(long id, int length)> input)
{
    long checksum = 0;
    int i = 0;
    foreach (var block in input)
    {
        if (block.id != -1)
        {
            for (int j = 0; j < block.length; j++)
            {
                checksum += block.id * i;
                ++i;
            }
        }
        else
        {
            i += block.length;
        }
    }
    return checksum;
}

void part1(string file_name)
{
    long solution = 0;
    var input = parse<List<long>>(file_name, add_func1);
    int blank_index = 0;
    int last_block = input.Count - 1;

    var nextBlock = () =>
    {
        while (input[blank_index] != FREE_SPACE) ++blank_index;
        while (input[last_block] == FREE_SPACE) --last_block;
    };

    for(nextBlock(); blank_index < last_block; nextBlock())
    {
        input[blank_index] = input[last_block];
        input[last_block] = FREE_SPACE;
    }

    solution = input.Select((v, i) => v * i).Where(v => v > 0).Sum();

    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    long solution = 0;
    var input = parse<List<(long id, int length)>>(file_name, add_func2);
    long block_id = FREE_SPACE;
    int last_block = input.Count - 1;

    var nextBlock = () =>
    {
        while (input[last_block].id == FREE_SPACE || input[last_block].id == block_id) --last_block;
        block_id = input[last_block].id;
    };

    for(nextBlock(); last_block > 0; nextBlock())
    {
        int opening_index = findOpening(input, last_block);

        if (opening_index < last_block)
        {
            moveFile(ref input, opening_index, last_block);
        }
    }

    solution = checkSum(input);

    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
part1("input.txt");

part2("sample.txt");
part2("input.txt");

delegate void AddFunc<T>(ref T mem_blocks, long block_id, int length);