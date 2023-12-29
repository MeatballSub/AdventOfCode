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

void showIntersect(List<long> a, List<long> b, PointD p)
{
    Console.WriteLine($"Hailstone A: {a[0]}, {a[1]}, {a[2]} @ {a[3]}, {a[4]}, {a[5]}");
    Console.WriteLine($"Hailstone B: {b[0]}, {b[1]}, {b[2]} @ {b[3]}, {b[4]}, {b[5]}");
    Console.WriteLine($"Hailstones' paths will cross inside the test area (at x={p.X}, y={p.Y}).");
}

//bool InArea(PointD p) => 7 <= p.X && p.X <= 27 && 7 <= p.Y && p.Y <= 27;
bool InArea(PointD p) => 200000000000000 <= p.X && p.X <= 400000000000000 && 200000000000000 <= p.Y && p.Y <= 400000000000000;

void part1(string file_name)
{
    long count = 0;
    List<List<long>> input = readFileLines(file_name).ExtractSignedLongs().Select(e => e.ToList()).ToList();
    for(int i = 0; i < input.Count; i++)
    {
        for(int j = i + 1; j < input.Count; j++)
        {
            (bool status, PointD p) = Intersect(input[i], input[j]);
            if (status && InArea(p))
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

    //input = input.OrderBy(e => rand.Next()).ToList();
    //double[,] matrix = new double[2, 3];
    //for (int i = 1; i < 3; ++i)
    //{
    //    matrix[i - 1, 0] = v_x0 - input[i][3];
    //    matrix[i - 1, 1] = input[i][0] - x_0;
    //    matrix[i - 1, 2] = matrix[i - 1, 0] * input[i][2] + matrix[i - 1, 1] * input[i][5];
    //}
    //return matrix;
}

long Average(List<List<long>> answers, int index)
{
    double average = answers[0][index];
    for(int i = 1; i < answers.Count; ++i)
    {
        average = average / (i + 1);
        average *= i;
        double new_val = answers[i][index];
        average += new_val / (i + 1);
    }
    return (long)Math.Round(average);
}

// 769840447420951 is too low
// 769840447420956
// 769840447420961 is too high
// 769840447420962 is too high
// 
void part2(string file_name)
{
    List<List<long>> input = readFileLines(file_name).ExtractSignedLongs().Select(e => e.ToList()).ToList();
    List<List<long>> answers = new();

    for (int i = 0; i < 1000; ++i)
    {
        double[,] matrix = BuildMatrix1(input);
        LinearEquationSolver.Solve(matrix);

        double x_0 = matrix[0, 4];
        double y_0 = matrix[1, 4];
        double v_x0 = matrix[2, 4];
        double v_y0 = matrix[3, 4];

        matrix = BuildMatrix2(input, x_0, y_0, v_x0, v_y0);
        LinearEquationSolver.Solve(matrix);

        double z_0 = matrix[1, 4];
        double v_z0 = matrix[3, 4];

        long t_dec = (long)Math.Round((x_0 - input[0][0]) / (input[0][3] - v_x0));
        long v_x0_dec = (long)Math.Round(v_x0);
        long x_dec = (long)Math.Round(x_0);
        long v_y0_dec = (long)Math.Round(v_y0);
        long y_dec = (long)Math.Round(y_0);
        long v_z0_dec = (long)Math.Round(v_z0);
        long z_dec = (long)Math.Round(z_0);
        if ((x_dec + v_x0_dec * t_dec == input[0][0] + input[0][3] * t_dec) && (y_dec + v_y0_dec * t_dec == input[0][1] + input[0][4] * t_dec) && (z_dec + v_z0_dec * t_dec == input[0][2] + input[0][5] * t_dec))
        {
            int index = answers.Count;
            answers.Add(new());
            answers[index].Add(x_dec);
            answers[index].Add(y_dec);
            answers[index].Add(z_dec);
            answers[index].Add(v_x0_dec);
            answers[index].Add(v_y0_dec);
            answers[index].Add(v_z0_dec);
            answers[index].Add(x_dec + y_dec + z_dec);
            Console.WriteLine($"{x_dec}, {y_dec}, {z_dec} @ {v_x0_dec}, {v_y0_dec}, {v_z0_dec}");
            Console.WriteLine($"{x_dec + y_dec + z_dec}");
        }
    }

    long x_0_fin = Average(answers, 0);
    long y_0_fin = Average(answers, 1);
    long z_0_fin = Average(answers, 2);
    long v_x0_fin = Average(answers, 3);
    long v_y0_fin = Average(answers, 4);
    long v_z0_fin = Average(answers, 5);
    long sum = x_0_fin + y_0_fin + z_0_fin;

    Console.WriteLine("Averages:");
    Console.WriteLine($"{x_0_fin}, {y_0_fin}, {z_0_fin} @ {v_x0_fin}, {v_y0_fin}, {v_z0_fin}");
    Console.WriteLine($"{sum}");
}

part2("input.txt");

public static class LinearEquationSolver
{
    /// <summary>Computes the solution of a linear equation system.</summary>
    /// <param name="M">
    /// The system of linear equations as an augmented matrix[row, col] where (rows + 1 == cols).
    /// It will contain the solution in "row canonical form" if the function returns "true".
    /// </param>
    /// <returns>Returns whether the matrix has a unique solution or not.</returns>
    public static bool Solve(double[,] M)
    {
        int rowCount = M.GetUpperBound(0) + 1;

        // pivoting
        for (int col = 0; col + 1 < rowCount; col++) if (M[col, col] == 0)
        // check for zero coefficients
        {
            // find non-zero coefficient
            int swapRow = col + 1;
            for (; swapRow < rowCount; swapRow++)
            {
                if (M[swapRow, col] != 0)
                    break;
            }

            if (M[swapRow, col] != 0) // found a non-zero coefficient?
            {
                // yes, then swap it with the above
                double[] tmp = new double[rowCount + 1];
                for (int i = 0; i < rowCount + 1; i++)
                {
                    tmp[i] = M[swapRow, i];
                    M[swapRow, i] = M[col, i];
                    M[col, i] = tmp[i];
                }
            }
            else
            {
                return false; // no, then the matrix has no unique solution
            }
        }

        // elimination
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

        // back-insertion
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
}