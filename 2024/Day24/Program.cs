using Library;
using System.Xml;
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

long part2(string file_name)
{
    var solution = 0L;
    var error_output = 0L;
    var (wires, gates) = parse(file_name);
    var sorted_gates = TopologicalSort(wires, gates);

    Dictionary<string, bool> outputs = wires.ToDictionary(w => w.name, w => w.value);

    foreach (var gate in sorted_gates)
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

    foreach (var output in outputs.Where(o => o.Key.StartsWith("z")).OrderByDescending(o => o.Key))
    {
        error_output <<= 1;
        if (output.Value) error_output |= 1;
    }

    foreach (var output in outputs.Where(o => o.Key.StartsWith("z")).OrderByDescending(o => o.Key))
    {
        error_output <<= 1;
        if (output.Value) error_output |= 1;
    }

    return solution;
}

test(part1, "part1", "sample.txt", 4);
test(part1, "part1", "sample2.txt", 2024);
test(part1, "part1", "input.txt", 55114892239566);

test(part2, "part2", "sample.txt", 0);
test(part2, "part2", "input.txt", 0);