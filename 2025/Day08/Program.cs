using System.Collections.Generic;
using Library;
using static Library.Parsing;
using static Library.Testing;

using Connection = (int index_1, int index_2, long distance);

long get_distance(Geometry.Point3d a, Geometry.Point3d b)
{
    Geometry.Point3d delta = a - b;
    return (delta.X * delta.X) + (delta.Y * delta.Y) + (delta.Z * delta.Z);
}

(List<Geometry.Point3d> junction_boxes, List<HashSet<int>> circuits, List<Connection> connections) init_problem(string file_name)
{
    var input = readFileLines(file_name);
    var junction_boxes = input.Select(l => l.Split(',').Select(long.Parse).ToList()).Select(p => new Geometry.Point3d(p[0], p[1], p[2])).ToList();
    List<HashSet<int>> circuits = new();

    List<(int index_1, int index_2, long distance)> connections = new();
    for (int i = 0; i < junction_boxes.Count(); ++i)
    {
        circuits.Add(new() { i });
        for (int j = i + 1; j < junction_boxes.Count(); ++j)
        {
            connections.Add((i, j, get_distance(junction_boxes[i], junction_boxes[j])));
        }
    }

    connections = connections.OrderBy(d => d.distance).ToList();
    return (junction_boxes, circuits, connections);
}

void make_connection(List<HashSet<int>> circuits, Connection connection)
{
    var circuit_1_index = circuits.FindIndex(c => c.Contains(connection.index_1));
    var circuit_2_index = circuits.FindIndex(c => c.Contains(connection.index_2));
    var circuit_1 = circuits[circuit_1_index];
    var circuit_2 = circuits[circuit_2_index];
    new HashSet<int>() { circuit_1_index, circuit_2_index }.OrderDescending().ToList().ForEach(i => circuits.RemoveAt(i));
    circuit_1.UnionWith(circuit_2);
    circuits.Add(circuit_1);
}

long part1(string file_name)
{
    int num_connections = (file_name == "sample.txt") ? 10 : 1000;

    var (junction_boxes, circuits, connections) = init_problem(file_name);
    connections = connections.Take(num_connections).ToList();

    foreach (var connection in connections)
    {
        make_connection(circuits, connection);
    }

    return circuits.Select(c => c.Count).OrderDescending().Take(3).Product();
}

long part2(string file_name)
{
    var (junction_boxes, circuits, connections) = init_problem(file_name);
    Connection final_connection = connections.First();
    foreach (var connection in connections)
    {
        make_connection(circuits, connection);
        final_connection = connection;
        if (circuits.Count == 1) break; ;
    }

    return junction_boxes[final_connection.index_1].X * junction_boxes[final_connection.index_2].X;
}

test(part1, "part1", "sample.txt", 40);
test(part1, "part1", "input.txt", 54180);

test(part2, "part2", "sample.txt", 25272);
test(part2, "part2", "input.txt", 25325968);