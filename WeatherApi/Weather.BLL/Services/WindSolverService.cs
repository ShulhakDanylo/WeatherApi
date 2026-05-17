using Microsoft.EntityFrameworkCore;
using Weather.BLL.Helpers;
using Weather.DAL.Data;
using Weather.DAL.Entities;

namespace Weather.BLL.Services;

public class WindSolverService
{
    private readonly WeatherDbContext _context;
    private const double Omega = 7.2921e-5; // Кутова швидкість Землі
    private const double R = 287.05;       // Газова стала повітря

    public WindSolverService(WeatherDbContext context)
    {
        _context = context;
    }

    public async Task<List<WeatherRecord>> CalculateWindStepAsync(double dt, List<WeatherRecord> inputData)
    {
        int n = inputData.Count;
        double[,] matrixA = new double[n, n];
        double[] vectorB_u = new double[n];
        double[] vectorB_v = new double[n];

        for (int i = 0; i < n; i++)
        {
            var current = inputData[i];
            if (current.Region == null)
            {
                current.Region = await _context.Regions.FindAsync(current.RegionId);
                if (current.Region == null) throw new Exception($"Region {current.RegionId} not found.");
            }

            double latRad = current.Region.Lat * Math.PI / 180.0;
            double f = 2 * Omega * Math.Sin(latRad); // Параметр Коріоліса
            double rho = (current.Pressure * 100) / (R * (current.Temperature + 273.15)); // Густина повітря

            var (dpdx, dpdy) = Calculate2DGradients(i, inputData, r => r.Pressure);
        
            double hour = current.RecordedAt.Hour + (current.RecordedAt.Minute / 60.0);
            double synopticPhase = (current.RecordedAt.DayOfYear + hour / 24.0) * 2.0 * Math.PI / 4.0;
            
            double backgroundU = 1.6 + Math.Sin(synopticPhase) * 1.0; 
            double backgroundV = 0.5 + Math.Cos(synopticPhase * 1.2) * 0.6;

            double pressureForceU = -(1.0 / rho) * dpdx * 0.35;
            double pressureForceV = -(1.0 / rho) * dpdy * 0.35;
            
            double coriolisU = f * current.WindV;
            double coriolisV = -f * current.WindU;

            double frictionU = (backgroundU - current.WindU) * 0.0002;
            double frictionV = (backgroundV - current.WindV) * 0.0002;

            double accU = pressureForceU + coriolisU + frictionU;
            double accV = pressureForceV + coriolisV + frictionV;
          
            accU = Math.Clamp(accU, -0.005, 0.005);
            accV = Math.Clamp(accV, -0.005, 0.005);

            matrixA[i, i] = 1.0; 
            vectorB_u[i] = current.WindU + dt * accU;
            vectorB_v[i] = current.WindV + dt * accV;
        }

        double[] nextU = LinearSolver.SolveGaussSeidel(matrixA, vectorB_u, inputData.Select(r => r.WindU).ToArray());
        double[] nextV = LinearSolver.SolveGaussSeidel(matrixA, vectorB_v, inputData.Select(r => r.WindV).ToArray());
        
        for (int i = 0; i < n; i++)
        {
            double u = double.IsNaN(nextU[i]) ? inputData[i].WindU : nextU[i];
            double v = double.IsNaN(nextV[i]) ? inputData[i].WindV : nextV[i];
            
            u = Math.Clamp(u, -7.5, 7.5);
            v = Math.Clamp(v, -7.5, 7.5);

            inputData[i].WindU = Math.Round(u, 2);
            inputData[i].WindV = Math.Round(v, 2);
            inputData[i].WindSpeed = Math.Round(Math.Sqrt(u * u + v * v), 2);
        }

        return inputData;
    }

    private (double dx, double dy) Calculate2DGradients(int idx, List<WeatherRecord> nodes, Func<WeatherRecord, double> property)
    {
        var curr = nodes[idx];
        var east = nodes.Where(n => n.Region.Lon > curr.Region.Lon && Math.Abs(n.Region.Lat - curr.Region.Lat) < 0.6).OrderBy(n => n.Region.Lon).FirstOrDefault();
        var north = nodes.Where(n => n.Region.Lat > curr.Region.Lat && Math.Abs(n.Region.Lon - curr.Region.Lon) < 0.6).OrderBy(n => n.Region.Lat).FirstOrDefault();

        double gradX = 0, gradY = 0;
        if (east != null) {
            double dx = Math.Max((east.Region.Lon - curr.Region.Lon) * 73000, 20000); 
            gradX = (property(east) - property(curr)) / dx;
        }
        if (north != null) {
            double dy = Math.Max((north.Region.Lat - curr.Region.Lat) * 111000, 20000); 
            gradY = (property(north) - property(curr)) / dy;
        }
        return (gradX, gradY);
    }
}