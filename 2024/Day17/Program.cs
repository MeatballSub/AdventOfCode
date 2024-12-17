using static Library.Parsing;

List<long> run(long reg_A, long reg_B, long reg_C, List<long> program)
{
    long IC = 0;
    List<long> output = new();

    while (IC + 1 < program.Count)
    {
        Instruction instruction = (Instruction)program[(int)IC];
        long operand = program[(int)IC + 1];
        long combo_op = operand;
        if (operand == 4)
        {
            combo_op = reg_A;
        }
        else if (operand == 5)
        {
            combo_op = reg_B;
        }
        else if (operand == 6)
        {
            combo_op = reg_C;
        }

        if (instruction == Instruction.JNZ)
        {
            IC = (reg_A != 0) ? operand : IC + 2;
        }
        else
        {
            if (instruction == Instruction.ADV)
            {
                reg_A >>= (int)combo_op;
            }
            else if (instruction == Instruction.BXL)
            {
                reg_B ^= operand;
            }
            else if (instruction == Instruction.BST)
            {
                reg_B = combo_op & 7;
            }
            else if (instruction == Instruction.BXC)
            {
                reg_B ^= reg_C;
            }
            else if (instruction == Instruction.OUT)
            {
                output.Add(combo_op & 7);
            }
            else if (instruction == Instruction.BDV)
            {
                reg_B = reg_A >> (int)combo_op;
            }
            else if (instruction == Instruction.CDV)
            {
                reg_C = reg_A >> (int)combo_op;
            }

            IC += 2;
        }
    }

    return output;
}

void part1(string file_name)
{
    var input = readFileLines(file_name);
    long reg_A = input[0].ExtractLongs().Single();
    long reg_B = input[1].ExtractLongs().Single();
    long reg_C = input[2].ExtractLongs().Single();
    List<long> program = input[3].ExtractLongs().ToList();

    var output = run(reg_A, reg_B, reg_C, program);
    Console.WriteLine($"Part 1: {string.Join(',', output)}");
}

int matchFromEndCount(List<long> a, List<long> b)
{
    List<long> new_a = new(a);
    List<long> new_b = new(b);
    new_a.Reverse();
    new_b.Reverse();

    int index = 0;
    while (index < new_a.Count && index < new_b.Count && new_a[index] == new_b[index]) ++index;
    return index;
}

void part2(string file_name)
{
    var input = readFileLines(file_name);
    long reg_A = input[0].ExtractLongs().Single();
    long reg_B = input[1].ExtractLongs().Single();
    long reg_C = input[2].ExtractLongs().Single();
    List<long> program = input[3].ExtractLongs().ToList();

    long inc = 1L << ((program.Count - 1) * 3);

    Stack<(int matches, long base_value, long inc_value)> frontier = new();
    frontier.Push((0, 0, inc));

    while (frontier.TryPop(out var possibility))
    {
        long count = 0;
        for (long value = possibility.base_value; count < 8; value += possibility.inc_value)
        {
            var output = run(value, reg_B, reg_C, program);
            int match_count = matchFromEndCount(program, output);
            if (match_count == program.Count)
            {
                Console.WriteLine($"Part 2: {value}");
                break;
            }
            if (match_count > possibility.matches)
            {
                frontier.Push((possibility.matches + 1, value, possibility.inc_value >> 3));
            }
            ++count;
        }
    }
}

part1("input.txt");
part2("input.txt");

enum Instruction
{
    ADV,
    BXL,
    BST,
    JNZ,
    BXC,
    OUT,
    BDV,
    CDV
};
