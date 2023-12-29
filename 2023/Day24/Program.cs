using Library;
using static Library.Geometry;
using static Library.Optimize;
using static Library.Parsing;

using PointD = (double X, double Y);

Random rand = new Random();

bool IsParallel(List<long> a, List<long> b)
{
    // note, not checking colinearity
    return (a[4] * b[3] == b[4] * a[3]);
}

double SolveT(List<long> a, List<long> b)
{
    double t = b[3] * (b[1] - a[1]) + b[4] * (a[0] - b[0]);
    t /= (a[4] * b[3]) - (a[3] * b[4]);
    return t;
}

(bool status, PointD p) Intersect(List<long> a, List<long> b)
{
    if(IsParallel(a,b))
    {
        return (false, new PointD(0, 0));
    }

    double t0 = SolveT(a,b);
    double t1 = SolveT(b, a);

    if (t0 < 0 || t1 < 0) // in the past
    {
        return (false, new PointD(0, 0));
    }

    double x = a[0] + t0 * a[3];
    double y = a[1] + t0 * a[4];

    return (true, new PointD(x,y));
}

bool InAreaSample(PointD p) => 7 <= p.X && p.X <= 27 && 7 <= p.Y && p.Y <= 27;
bool InArea(PointD p) => 200000000000000 <= p.X && p.X <= 400000000000000 && 200000000000000 <= p.Y && p.Y <= 400000000000000;

void part1(string file_name, InAreaFunc inArea)
{
    long count = 0;
    List<List<long>> input = readFileLines(file_name).ExtractSignedLongs().Select(e => e.ToList()).ToList();
    for(int i = 0; i < input.Count; i++)
    {
        for(int j = i + 1; j < input.Count; j++)
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

double[,] BuildMatrix1(List<List<long>> input)
{    
    input = input.OrderBy(e => rand.Next()).ToList();
    double[,] matrix = new double[4, 5];
    for(int i = 1; i < 5; ++i)
    {
        matrix[i - 1, 0] = input[i][4] - input[0][4];
        matrix[i - 1, 1] = input[0][3] - input[i][3];
        matrix[i - 1, 2] = input[0][1] - input[i][1];
        matrix[i - 1, 3] = input[i][0] - input[0][0];
        matrix[i - 1, 4] = input[0][1] * input[0][3] - input[i][1] * input[i][3] + input[i][0] * input[i][4] - input[0][0] * input[0][4];
    }
    return matrix;
}

double[,] BuildMatrix2(List<List<long>> input, double x_0, double y_0, double v_x0, double v_y0)
{
    input = input.OrderBy(e => rand.Next()).ToList();
    double[,] matrix = new double[4, 5];
    for (int i = 1; i < 5; ++i)
    {
        matrix[i - 1, 0] = input[i][5] - input[0][5];
        matrix[i - 1, 1] = input[0][3] - input[i][3];
        matrix[i - 1, 2] = input[0][2] - input[i][2];
        matrix[i - 1, 3] = input[i][0] - input[0][0];
        matrix[i - 1, 4] = input[0][2] * input[0][3] - input[i][2] * input[i][3] + input[i][0] * input[i][5] - input[0][0] * input[0][5];
    }
    return matrix;
}

void part2(string file_name)
{
    List<List<long>> input = readFileLines(file_name).ExtractSignedLongs().Select(e => e.ToList()).ToList();
    List<List<long>> answers = new();

    long t_dec = 0;
    long v_x0_dec = 0;
    long x_dec = 0;
    long v_y0_dec = 0;
    long y_dec = 0;
    long v_z0_dec = 0;
    long z_dec = 0;

    do
    {
        double[,] matrix = BuildMatrix1(input);
        if(LinearEquationSolver.Solve(matrix))
        {
            double x_0 = matrix[0, 4];
            double y_0 = matrix[1, 4];
            double v_x0 = matrix[2, 4];
            double v_y0 = matrix[3, 4];

            matrix = BuildMatrix2(input, x_0, y_0, v_x0, v_y0);
            if(LinearEquationSolver.Solve(matrix))
            {
                double z_0 = matrix[1, 4];
                double v_z0 = matrix[3, 4];

                t_dec = (long)Math.Round((x_0 - input[0][0]) / (input[0][3] - v_x0));
                v_x0_dec = (long)Math.Round(v_x0);
                x_dec = (long)Math.Round(x_0);
                v_y0_dec = (long)Math.Round(v_y0);
                y_dec = (long)Math.Round(y_0);
                v_z0_dec = (long)Math.Round(v_z0);
                z_dec = (long)Math.Round(z_0);
            }
        }
    } while ((x_dec + v_x0_dec * t_dec != input[0][0] + input[0][3] * t_dec) || (y_dec + v_y0_dec * t_dec != input[0][1] + input[0][4] * t_dec) || (z_dec + v_z0_dec * t_dec != input[0][2] + input[0][5] * t_dec));

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
    public static bool Solve(double[,] M)
    {
        int rowCount = M.GetLength(0);

        for (int col = 0; col + 1 < rowCount; col++)
        {
            if (M[col, col] == 0)
            {
                int swapRow = col + 1;
                while(swapRow < rowCount && M[swapRow, col] == 0)
                {
                    ++swapRow;
                }

                if(M[swapRow, col] == 0)
                {
                    return false;
                }

                double tmp = 0;
                for (int i = 0; i < rowCount + 1; i++)
                {
                    tmp = M[swapRow, i];
                    M[swapRow, i] = M[col, i];
                    M[col, i] = tmp;
                }
            }
        }

        for (int sourceRow = 0; sourceRow + 1 < rowCount; sourceRow++)
        {
            for (int destRow = sourceRow + 1; destRow < rowCount; destRow++)
            {
                double df = M[sourceRow, sourceRow];
                double sf = M[destRow, sourceRow];
                for (int i = 0; i < rowCount + 1; i++)
                {
                    M[destRow, i] = M[destRow, i] * df - M[sourceRow, i] * sf;
                }
            }
        }

        for (int row = rowCount - 1; row >= 0; row--)
        {
            double f = M[row, row];
            if (f == 0)
            {
                return false;
            }

            for (int i = 0; i < rowCount + 1; i++)
            {
                M[row, i] /= f;
            }

            for (int destRow = 0; destRow < row; destRow++)
            {
                M[destRow, rowCount] -= M[destRow, row] * M[row, rowCount];
                M[destRow, row] = 0;
            }
        }

        return true;
    }

    public static bool Solve(double[][] matrix)
    {
        int rowCount = matrix.GetLength(0);

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

                double tmp = 0;
                for (int i = 0; i < rowCount + 1; i++)
                {
                    tmp = matrix[swapRow][i];
                    matrix[swapRow][i] = matrix[col][i];
                    matrix[col][i] = tmp;
                }
            }
        }

        for (int sourceRow = 0; sourceRow + 1 < rowCount; sourceRow++)
        {
            for (int destRow = sourceRow + 1; destRow < rowCount; destRow++)
            {
                double df = matrix[sourceRow][sourceRow];
                double sf = matrix[destRow][sourceRow];
                for (int i = 0; i < rowCount + 1; i++)
                {
                    matrix[destRow][i] = matrix[destRow][i] * df - matrix[sourceRow][i] * sf;
                }
            }
        }

        for (int row = rowCount - 1; row >= 0; row--)
        {
            double f = matrix[row][row];
            if (f == 0)
            {
                return false;
            }

            for (int i = 0; i < rowCount + 1; i++)
            {
                matrix[row][i] /= f;
            }

            for (int destRow = 0; destRow < row; destRow++)
            {
                matrix[destRow][rowCount] -= matrix[destRow][row] * matrix[row][rowCount];
                matrix[destRow][row] = 0;
            }
        }

        return true;
    }
}

delegate bool InAreaFunc(PointD p);