using Library;
using static Library.Parsing;
using static Library.Testing;

(List<(string name, bool value)>, List<(List<string> inputs, string type, string output)>) parse(string file_name)
{
    var parts = SplitBlankLine(file_name);
    var wire_parts = parts[0].SplitLines();
    var gate_parts = parts[1].SplitLines();
    var wires = wire_parts.Select(w => w.Split(": ", StringSplitOptions.RemoveEmptyEntries)).Select(w_parts => (name: w_parts[0], value: w_parts[1] == "1")).ToList();
    var gates = gate_parts.Select(g => g.Split(" ", StringSplitOptions.RemoveEmptyEntries)).Select(g_parts => (inputs: new List<string>() { g_parts[0], g_parts[2] }, type: g_parts[1], output: g_parts[4])).ToList();
    return (wires, gates);
}

List<(List<string> inputs, string type, string output)> TopologicalSort(List<(string name, bool value)> wires, List<(List<string> inputs, string type, string output)> gates)
{
    List<(List<string> inputs, string type, string output)> sorted_gates = new();
    Dictionary<string,(List<string> inputs, string type, string output)> remaining_gates = gates.ToDictionary(g => g.output, g => g);
    HashSet<string> ready_wires = wires.Select(w => w.name).ToHashSet();

    while(sorted_gates.Count != gates.Count)
    {
        foreach(var (output, gate) in remaining_gates)
        {
            if (ready_wires.Contains(gate.inputs[0]) && ready_wires.Contains(gate.inputs[1]))
            {
                sorted_gates.Add(gate);
                ready_wires.Add(output);
                remaining_gates.Remove(output);
            }
        }
    }

    return sorted_gates;
}

long wires_to_long(List<bool> wire_values)
{
    long value = 0L;
    foreach (var output in wire_values)
    {
        value <<= 1;
        if (output) value |= 1;
    }
    return value;
}

long part1(string file_name)
{
    var solution = 0L;
    var (wires, gates) = parse(file_name);
    var sorted_gates = TopologicalSort(wires, gates);

    Dictionary<string, bool> outputs = wires.ToDictionary(w => w.name, w => w.value);

    foreach(var gate in sorted_gates)
    {
        var input0 = outputs[gate.inputs[0]];
        var input1 = outputs[gate.inputs[1]];
        var output_value = gate.type switch
        {
            "OR" => input0 || input1,
            "AND" => input0 && input1,
            "XOR" => input0 ^ input1,
        };
        outputs[gate.output] = output_value;
    }

    var output_wire_values = outputs.Where(o => o.Key.StartsWith("z")).OrderByDescending(o => o.Key).Select(o => o.Value).ToList();

    solution = wires_to_long(output_wire_values);

    return solution;
}

bool FindHalfAdd(int i, List<(List<string> inputs, string type, string output)> gates, out string half_add_gate)
{
    string wire_num = i.ToString("D2");
    string x_in = "x" + wire_num;
    string y_in = "y" + wire_num;

    var gate = gates.Where(g => g.inputs.Contains(x_in) && g.inputs.Contains(y_in) && g.type == "XOR");

    if(gate.Count() == 1)
    {
        half_add_gate = gate.Single().output;
        return true;
    }

    half_add_gate = "";
    return false;
}

bool FindHalfCarry(int i, List<(List<string> inputs, string type, string output)> gates, out string half_carry_gate)
{
    string wire_num = i.ToString("D2");
    string x_in = "x" + wire_num;
    string y_in = "y" + wire_num;

    var gate = gates.Where(g => g.inputs.Contains(x_in) && g.inputs.Contains(y_in) && g.type == "AND");

    if (gate.Count() == 1)
    {
        half_carry_gate = gate.Single().output;
        return true;
    }

    half_carry_gate = "";
    return false;
}

bool FindFullAdd(List<(List<string> inputs, string type, string output)> gates, string half_add, string prev_carry, out string full_add_gate)
{
    var gate = gates.Where(g => g.inputs.Contains(half_add) && g.inputs.Contains(prev_carry) && g.type == "XOR");

    if (gate.Count() == 1)
    {
        full_add_gate = gate.Single().output;
        return true;
    }

    full_add_gate = "";
    return false;
}

bool FindOverflowCarry(List<(List<string> inputs, string type, string output)> gates, string half_add, string prev_carry, out string overflow_carry_gate)
{
    var gate = gates.Where(g => g.inputs.Contains(half_add) && g.inputs.Contains(prev_carry) && g.type == "AND");

    if (gate.Count() == 1)
    {
        overflow_carry_gate = gate.Single().output;
        return true;
    }

    overflow_carry_gate = "";
    return false;
}

bool FindFullCarry(List<(List<string> inputs, string type, string output)> gates, string half_carry, string overflow_carry, out string full_carry_gate)
{
    var gate = gates.Where(g => g.inputs.Contains(half_carry) && g.inputs.Contains(overflow_carry) && g.type == "OR");

    if (gate.Count() == 1)
    {
        full_carry_gate = gate.Single().output;
        return true;
    }

    full_carry_gate = "";
    return false;
}

string part2(string file_name)
{
    var solution = "";
    var error_output = 0L;
    var (wires, gates) = parse(file_name);
    Dictionary<int, string> half_add = new();
    Dictionary<int, string> half_carry = new();
    Dictionary<int, string> full_add = new();
    Dictionary<int, string> overflow_carry = new();
    Dictionary<int, string> full_carry = new();
    List<string> swaps = new();
    Dictionary<string, (List<string> inputs, string type, string output)> gate_dict = gates.ToDictionary(g => g.output, g => g);

    if(gate_dict["z00"].type != "XOR" || !gate_dict["z00"].inputs.Contains("x00") || !gate_dict["z00"].inputs.Contains("y00"))
    {
        Console.WriteLine("z00 is wrong, not half adder");
    }

    try
    {
        full_carry[0] = gates.Single(g => g.type == "AND" && g.inputs.Contains("x00") && g.inputs.Contains("y00")).output;
    }
    catch(Exception e)
    {
        Console.WriteLine("Couldn't find a carry gate for 00");
        throw new(e.ToString());
    }

    var z_gates = gates.Where(g => g.output.StartsWith("z"));

    for (int i = 1; i < z_gates.Count() - 1; ++i)
    {
        if(FindHalfAdd(i, gates, out var half_add_gate))
        {
            half_add[i] = half_add_gate;
        }
        else
        {
            Console.WriteLine($"Couldn't find half add gate: {i}");
        }

        if (FindHalfCarry(i, gates, out var half_carry_gate))
        {
            half_carry[i] = half_carry_gate;
        }
        else
        {
            Console.WriteLine($"Couldn't find half carry gate: {i}");
        }

        if(FindFullAdd(gates, half_add[i], full_carry[i - 1], out var full_add_gate))
        {
            string wire_num = i.ToString("D2");
            string expected = "z" + wire_num;
            if(full_add_gate != expected)
            {
                Console.WriteLine($"Swap: {expected} <-> {full_add_gate}");
                swaps.Add(expected);
                swaps.Add(full_add_gate);
                int expected_index = gates.FindIndex(g => g.output == expected);
                int actual_index = gates.FindIndex(g => g.output == full_add_gate);

                var temp = gates[expected_index];
                temp.output = full_add_gate;
                gates[expected_index] = temp;

                temp = gates[actual_index];
                temp.output = expected;
                gates[actual_index] = temp;

                --i;
                continue;
            }
            full_add[i] = expected;
        }
        else
        {
            string wire_num = i.ToString("D2");
            string output = "z" + wire_num;
            var gate = gate_dict[output];
            string expected;
            string actual;
            if(gate.inputs.Contains(half_add[i]))
            {
                expected = gate.inputs.Where(inp => inp != half_add[i]).Single();
                actual = full_carry[i - 1];
            }
            else
            {
                expected = gate.inputs.Where(inp => inp != full_carry[i - 1]).Single();
                actual = half_add[i];
            }
            Console.WriteLine($"Swap: {expected} <-> {actual}");

            swaps.Add(expected);
            swaps.Add(actual);
            int expected_index = gates.FindIndex(g => g.output == expected);
            int actual_index = gates.FindIndex(g => g.output == actual);

            var temp = gates[expected_index];
            temp.output = actual;
            gates[expected_index] = temp;

            temp = gates[actual_index];
            temp.output = expected;
            gates[actual_index] = temp;

            --i;
            continue;
        }

        if (FindOverflowCarry(gates, half_add[i], full_carry[i - 1], out var overflow_carry_gate))
        {
            overflow_carry[i] = overflow_carry_gate;
        }
        else
        {
            Console.WriteLine($"Couldn't find overflow carry gate: {i}");
        }

        if(FindFullCarry(gates, half_carry[i], overflow_carry[i], out var full_carry_gate))
        {
            full_carry[i] = full_carry_gate;
        }
        else
        {
            Console.WriteLine($"Couldn't find full carry gate: {i}: {half_carry[i]} OR {overflow_carry[i]}");
        }
    }

    swaps.Sort();
    solution = string.Join(',', swaps);

    return solution;
}

test(part1, "part1", "sample.txt", 4);
test(part1, "part1", "sample2.txt", 2024);
test(part1, "part1", "input.txt", 55114892239566);

test(part2, "part2", "input.txt", "cdj,dhm,gfm,mrb,qjd,z08,z16,z32");