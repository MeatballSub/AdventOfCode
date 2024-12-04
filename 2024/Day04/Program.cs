using Library;
using static Library.Geometry;
using static Library.Parsing;
using static System.Net.Mime.MediaTypeNames;

void display(string[] arr)
{
    Console.SetCursorPosition(0,0);
    for (int h = 0; h < arr.Length; h++)
    {
        for(int w = 0;  w < arr[h].Length; w++)
        {
            Console.Write(arr[h][w]);
        }
        Console.WriteLine();
    }
}

void part1(string file_name)
{
    Thread.Sleep(5000);
    var orig_color = Console.ForegroundColor;
    Console.CursorVisible = false;
    var sleep_time = 200;
    var search = "XMAS";
    var input = readFileLines(file_name);
    display(input);
    long solution = 0;
    for(int h = 0; h < input.Length; ++h)
    {
        for ( int w = 0; w < input[h].Length; ++w)
        {
            Console.SetCursorPosition(w, h);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(input.at(new Point(w, h)));
            Console.ForegroundColor = orig_color;
            Thread.Sleep(sleep_time);

            int index = 0;
            if ((index < search.Length) && new Point(w,h).boundsCheck(input) && (input.at(new Point(w, h)) == search[index]))
            {
                for (int y_dir = -1; y_dir < 2; ++y_dir)
                {
                    for (int x_dir = -1; x_dir < 2; x_dir++)
                    {
                        if (x_dir != 0 || y_dir != 0)
                        {
                            index = 0;
                            var move = new Point(x_dir, y_dir);
                            var next = new Point(w, h);
                            while ((index < search.Length) && next.boundsCheck(input) && (input.at(next) == search[index]))
                            {
                                Console.SetCursorPosition((int)next.X, (int)next.Y);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(input.at(next));
                                Console.ForegroundColor = orig_color;
                                Thread.Sleep(sleep_time);
                                next += move;
                                ++index;
                                if ((index < search.Length) && next.boundsCheck(input))
                                {
                                    Console.SetCursorPosition((int)next.X, (int)next.Y);
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write(input.at(next));
                                    Console.ForegroundColor = orig_color;
                                    Thread.Sleep(sleep_time);
                                }
                            }
                            if (index == search.Length)
                            {
                                ++solution;
                                Console.SetCursorPosition(0, input.Length);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"part 1 - {file_name}: {solution}");
                                Console.ForegroundColor = orig_color;
                                Thread.Sleep(sleep_time);
                                Console.SetCursorPosition(0, input.Length);
                                Console.WriteLine($"part 1 - {file_name}: {solution}");
                                Thread.Sleep(sleep_time);
                            }
                            display(input);
                            Console.SetCursorPosition(w, h);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(input.at(new Point(w, h)));
                            Console.ForegroundColor = orig_color;
                            Thread.Sleep(sleep_time);
                        }
                    }
                }
            }

            Console.SetCursorPosition(w, h);
            Console.Write(input.at(new Point(w, h)));
            Console.SetCursorPosition(0, input.Length);
            Console.WriteLine($"part 1 - {file_name}: {solution}");
            Thread.Sleep(sleep_time);
        }
    }
    Console.SetCursorPosition(0, input.Length);
    Console.WriteLine($"part 1 - {file_name}: {solution}");
}

void part2(string file_name)
{
    var input = readFileLines(file_name);
    long solution = 0;
    for (int h = 0; h < input.Length; ++h)
    {
        for (int w = 0; w < input[h].Length; ++w)
        {
            Point p = new(w, h);
            if (input.at(p) == 'A')
            {
                var ul = p.Up().Left();
                var ur = p.Up().Right();
                var ll = p.Down().Left();
                var lr = p.Down().Right();

                if(ul.boundsCheck(input) && lr.boundsCheck(input))
                {
                    if ((input.at(ul) == 'M' && input.at(lr) == 'S') || (input.at(ul) == 'S' && input.at(lr) == 'M'))
                    {
                        if ((input.at(ur) == 'M' && input.at(ll) == 'S') || (input.at(ur) == 'S' && input.at(ll) == 'M'))
                        {
                            ++solution;
                        }
                    }
                }
            }
        }
    }
    Console.WriteLine($"part 2 - {file_name}: {solution}");
}

part1("sample.txt");
//part1("input.txt");

part2("sample.txt");
//part2("input.txt");

public static class Day4
{
    public static char at(this string[] arr, Point p) => arr[p.Y][(int)p.X];
    public static bool boundsCheck(this Point p, string[] arr) => p.X >= 0 && p.Y >= 0 && p.Y < arr.Length && p.X < arr[p.Y].Length;
}