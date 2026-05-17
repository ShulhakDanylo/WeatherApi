namespace Weather.BLL.Helpers;

public class LinearSolver
{
    public static double[] SolveGaussSeidel(double[,] a, double[] b, double[] x0, int maxIterations = 100, double epsilon = 1e-5)
    {
        int n = b.Length;
        double[] x = (double[])x0.Clone();

        for (int iter = 0; iter < maxIterations; iter++)
        {
            
            double maxDiff = 0;
                
        
            for (int i = 0; i < n; i++)
            {
                double sum = b[i];
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                        sum -= a[i, j] * x[j];
                }

                double newValue = sum / a[i, i];
                maxDiff = Math.Max(maxDiff, Math.Abs(newValue - x[i]));
                x[i] = newValue;
            }
            Console.WriteLine($"[MATH] Зейдель Ітерація {iter}: Похибка = {maxDiff:E4}");
            if (maxDiff < epsilon) 
            {
                Console.WriteLine($"[MATH] Збіжність досягнута на ітерації {iter}");
                break;
            }
        }
        return x;
    }
}