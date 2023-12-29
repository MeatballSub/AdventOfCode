using Library;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

using PointD = (double X, double Y);

Random rand = new Random();

int X = 0;
int Y = 1;
int Z = 2;
int VX = 3;
int VY = 4;
int VZ = 5;

bool IsParallel(List<long> a, List<long> b)
{
    // note, not checking colinearity
    return (a[VY] * b[VX] == b[VY] * a[VX]);
}

double SolveT(List<long> a, List<long> b)
{
    double t = b[VX] * (b[Y] - a[Y]) + b[VY] * (a[X] - b[X]);
    t /= (a[VY] * b[VX]) - (a[VX] * b[VY]);
    return t;
}

(bool status, PointD p) Intersect(List<long> a, List<long> b)
{
    if(IsParallel(a,b))
    {
        return (false, new PointD(0, 0));
    }

    double t0 = SolveT(a, b);
    double t1 = SolveT(b, a);

    if (t0 < 0 || t1 < 0) // in the past
    {
        return (false, new PointD(0, 0));
    }

    double x = a[X] + t0 * a[VX];
    double y = a[Y] + t0 * a[VY];

    return (true, new PointD(x, y));
}

bool InAreaSample(PointD p) => 7 <= p.X && p.X <= 27 && 7 <= p.Y && p.Y <= 27;
bool InArea(PointD p) => 200000000000000 <= p.X && p.X <= 400000000000000 && 200000000000000 <= p.Y && p.Y <= 400000000000000;

void part1(string file_name, InAreaFunc inArea)
{
    long count = 0;
    List<List<long>> input = readFileLines(file_name).ExtractSignedLongs().Select(e => e.ToList()).ToList();
    for(int i = 0; i < input.Count; i++)
    {
        for (int j = i + 1; j < input.Count; j++)
        {
            (bool status, PointD p) = Intersect(input[i], input[j]);
            if (status && inArea(p))
            {
                ++count;
            }
        }
    }
    Console.WriteLine($"Part 1 - {file_name}: {count}");
}

List<List<double>> BuildMatrix1(List<List<long>> input)
{    
    input = input.OrderBy(e => rand.Next()).ToList();
    List<List<double>> matrix = new();
    for(int i = 1; i < 5; ++i)
    {
        matrix.Add(new());
        matrix[i - 1].Add(input[i][VY] - input[0][VY]);
        matrix[i - 1].Add(input[0][VX] - input[i][VX]);
        matrix[i - 1].Add(input[0][Y] - input[i][Y]);
        matrix[i - 1].Add(input[i][X] - input[0][X]);
        matrix[i - 1].Add(input[0][Y] * input[0][VX] - input[i][Y] * input[i][VX] + input[i][X] * input[i][VY] - input[0][X] * input[0][VY]);
    }
    return matrix;
}

List<List<double>> BuildMatrix2(List<List<long>> input)
{
    input = input.OrderBy(e => rand.Next()).ToList();
    List<List<double>> matrix = new();
    for (int i = 1; i < 5; ++i)
    {
        matrix.Add(new());
        matrix[i - 1].Add(input[i][VZ] - input[0][VZ]);
        matrix[i - 1].Add(input[0][VX] - input[i][VX]);
        matrix[i - 1].Add(input[0][Z] - input[i][Z]);
        matrix[i - 1].Add(input[i][X] - input[0][X]);
        matrix[i - 1].Add(input[0][Z] * input[0][VX] - input[i][Z] * input[i][VX] + input[i][X] * input[i][VZ] - input[0][X] * input[0][VZ]);
    }
    return matrix;
}

void part2(string file_name)
{
    List<List<long>> input = readFileLines(file_name).ExtractSignedLongs().Select(e => e.ToList()).ToList();

    int RESULT = 4;

    long t_dec = 0;
    long v_x0_dec = 0;
    long x_dec = 0;
    long v_y0_dec = 0;
    long y_dec = 0;
    long v_z0_dec = 0;
    long z_dec = 0;

    do
    {
        List<List<double>> matrix = BuildMatrix1(input);
        if(LinearEquationSolver.Solve(matrix))
        {
            double x_0 = matrix[0][RESULT];
            double y_0 = matrix[1][RESULT];
            double v_x0 = matrix[2][RESULT];
            double v_y0 = matrix[3][RESULT];

            matrix = BuildMatrix2(input);
            if(LinearEquationSolver.Solve(matrix))
            {
                double z_0 = matrix[1][RESULT];
                double v_z0 = matrix[3][RESULT];

                t_dec = (long)Math.Round((x_0 - input[0][X]) / (input[0][VX] - v_x0));
                v_x0_dec = (long)Math.Round(v_x0);
                x_dec = (long)Math.Round(x_0);
                v_y0_dec = (long)Math.Round(v_y0);
                y_dec = (long)Math.Round(y_0);
                v_z0_dec = (long)Math.Round(v_z0);
                z_dec = (long)Math.Round(z_0);
            }
        }
    } while ((x_dec + v_x0_dec * t_dec != input[0][X] + input[0][VX] * t_dec) || (y_dec + v_y0_dec * t_dec != input[0][Y] + input[0][VY] * t_dec) || (z_dec + v_z0_dec * t_dec != input[0][Z] + input[0][VZ] * t_dec));

    long sum = x_dec + y_dec + z_dec;

    Console.WriteLine($"Part 2 - {file_name}:");
    Console.WriteLine($"    {x_dec}, {y_dec}, {z_dec} @ {v_x0_dec}, {v_y0_dec}, {v_z0_dec}");
    Console.WriteLine($"    {sum}");
}

part1("sample.txt", InAreaSample);
part1("input.txt", InArea);
part2("sample.txt");
part2("input.txt");

public static class LinearEquationSolver
{
    public static bool Solve(List<List<double>> matrix)
    {
        int rowCount = matrix.Count;

        for (int col = 0; col + 1 < rowCount; col++)
        {
            if (matrix[col][col] == 0)
            {
                int swapRow = col + 1;

                while (swapRow < rowCount && matrix[swapRow][col] == 0)
                {
                    ++swapRow;
                }

                if (matrix[swapRow][col] == 0)
                {
                    return false;
                }

                (matrix[swapRow], matrix[col]) = (matrix[col], matrix[swapRow]);
            }
        }

        for (int sourceRow = 0; sourceRow + 1 < rowCount; sourceRow++)
        {
            double df = matrix[sourceRow][sourceRow];
            for (int destRow = sourceRow + 1; destRow < rowCount; destRow++)
            {
                double sf = matrix[destRow][sourceRow];
                matrix[destRow] = matrix[destRow].Zip(matrix[sourceRow]).Select(e => e.First * df - e.Second * sf).ToList();
            }
        }

        for (int row = rowCount - 1; row >= 0; row--)
        {
            if (matrix[row][row] == 0)
            {
                return false;
            }

            matrix[row] = matrix[row].Select(e => e /= matrix[row][row]).ToList();
            Enumerable.Range(0, row).ToList().ForEach(i => matrix[i][rowCount] -= matrix[i][row] * matrix[row][rowCount]);
            Enumerable.Range(0, row).ToList().ForEach(i => matrix[i][row] = 0);
        }

        return true;
    }
}

delegate bool InAreaFunc(PointD p);