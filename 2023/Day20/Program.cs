using Library;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;


void parse(string file_name)
{
    string[] lines = readFileLines(file_name);
    List<(string name, List<string> targets)> mods = lines.Select(l => (l.Split(" -> ")[0], l.Split(" -> ")[1].Split(", ").ToList())).ToList();
    foreach( (string name, List<string> targets) in mods)
    {
        if(name.StartsWith('%'))
        {
            Global.modules.Add(name.Substring(1), new FlipFlop(name.Substring(1), targets));
        }
        else if(name.StartsWith('&'))
        {
            List<string> sources = mods.Where(m => m.targets.Contains(name.Substring(1))).Select(m => m.name).ToList();
            sources = sources.Select(s => s.StartsWith('%') || s.StartsWith('&') ? s.Substring(1) : s).ToList();
            Global.modules.Add(name.Substring(1), new Conjunction(name.Substring(1), targets, sources));
        }
        else if(name == "broadcaster")
        {
            Global.modules.Add(name, new Broadcaster(targets));
        }
    }
    foreach(string name in mods.SelectMany(m => m.targets).ToHashSet())
    {
        if(!Global.modules.ContainsKey(name))
        {
            Global.modules.Add(name, new Untyped());
        }
    }
}

void part1(string file_name)
{
    Global.init();
    Button button = new();
    parse(file_name);
    for(int i = 0; i < 1000; ++i)
    {
        button.push();
        Queue<IModule> queue = new Queue<IModule>();
        queue.Enqueue(button);
        while(queue.Count > 0)
        {
            IModule module = queue.Dequeue();
            module.activate().ForEach(m => queue.Enqueue(Global.modules[m]));
        }
    }

    long answer = Global.counts.Values.Product();
    Console.WriteLine($"Part 1 - {file_name}: {answer}");
}

void part2(string file_name)
{
    Global.init();
    Button button = new();
    parse(file_name);
    while (Global.final_conjunction_input_counts.Count < ((Conjunction)Global.modules[Global.FINAL_CONJUNCTION]).state.Count)
    {
        button.push();
        Queue<IModule> queue = new Queue<IModule>();
        queue.Enqueue(button);
        while (queue.Count > 0)
        {
            IModule module = queue.Dequeue();
            module.activate().ForEach(m => queue.Enqueue(Global.modules[m]));
        }
    }

    long answer = Global.final_conjunction_input_counts.Select(kvp => kvp.Value).LCM();
    Console.WriteLine($"Part 2 - {file_name}: {answer}");
}

part1("sample1.txt");
part1("sample2.txt");
part1("input.txt");
part2("input.txt");

public static class Global
{
    public const bool LOW = false;
    public const bool HIGH = true;
    public const string FINAL_CONJUNCTION = "dt";
    public static long button_count = 0;
    public static Dictionary<bool, long> counts = new Dictionary<bool, long>() { { false, 0 }, { true, 0 } };
    public static Dictionary<string, IModule> modules = new();
    public static Dictionary<string, long> final_conjunction_input_counts = new();
    

    public static void init()
    {
        button_count = 0;
        modules.Clear();
        counts[true] = 0;
        counts[false] = 0;
        final_conjunction_input_counts.Clear();
    }

    public static void sendPulse(string source, string target, bool pulse_type)
    {
        if(source == "button")
        {
            ++button_count;
        }
        //Console.WriteLine($"{source} -{(pulse_type ? "high" : "low")}-> {target}");
        if(pulse_type == Global.HIGH && target == FINAL_CONJUNCTION)
        {
            if(!final_conjunction_input_counts.ContainsKey(source))
            {
                final_conjunction_input_counts.Add(source, button_count);
            }
        }
        ++counts[pulse_type];
        modules[target].receive(source, pulse_type);
    }
}

public interface IModule
{
    void receive(string source, bool pulse_type);
    List<string> activate();
}

class Button : IModule
{
    List<string> targets = new List<string>() { "broadcaster" };
    public void receive(string source, bool pulse_type)
    {
    }

    public List<string> activate()
    {
        return targets;
    }

    public void push()
    {
        Global.sendPulse("button", "broadcaster", Global.LOW);
    }
}

class Broadcaster : IModule
{
    public bool state = false;
    public List<string> targets;
    public Broadcaster(List<string> targets)
    {
        this.targets = targets;
    }

    public List<string> activate()
    {
        foreach (var target in targets)
        {
            Global.sendPulse("broadcaster", target, state);
        }
        return targets;
    }

    public void receive(string source, bool pulse_type)
    {
        state = pulse_type;
    }
}

class FlipFlop : IModule
{
    public string name;
    public bool state = false;
    public bool send = false;
    public List<string> targets;
    public List<string> empty = new();
    public FlipFlop(string name, List<string> targets)
    {
        this.name = name;
        this.targets = targets;
    }

    public List<string> activate()
    {
        if (send)
        {
            send = false;
            foreach (var target in targets)
            {
                Global.sendPulse(name, target, state);
            }
            return targets;
        }
        return empty;
    }

    public void receive(string source, bool pulse_type)
    {
        if (pulse_type == Global.LOW)
        {
            send = true;
            state = !state;
        }
    }
}

class Conjunction: IModule
{
    public string name;
    public Dictionary<string, bool> state = new();
    public List<string> targets;
    public Conjunction(string name, List<string> targets, List<string> sources)
    {
        this.name = name;
        this.targets = targets;
        foreach(var source in sources)
        {
            state[source] = false;
        }
    }

    public List<string> activate()
    {
        bool output = (state.Values.All(v => v)) ? Global.LOW : Global.HIGH;
        foreach (var target in targets)
        {
            Global.sendPulse(name, target, output);
        }
        return targets;
    }

    public void receive(string source, bool pulse_type)
    {
        state[source] = pulse_type;
    }
}

class Untyped : IModule
{
    public List<string> targets = new();
    public void receive(string source, bool pulse_type)
    {
    }

    public List<string> activate()
    {
        return targets;
    }
}