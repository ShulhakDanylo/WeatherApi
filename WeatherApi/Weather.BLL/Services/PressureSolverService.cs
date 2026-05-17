using Microsoft.EntityFrameworkCore;
using Weather.BLL.Helpers;
using Weather.DAL.Data;
using Weather.DAL.Entities;

namespace Weather.BLL.Services;

public class PressureSolverService
{
    private readonly WeatherDbContext _context;
    private const double Kh_P = 20.0; 

    public PressureSolverService(WeatherDbContext context)
    {
        _context = context;
    }

    public void CalculateAndSavePressureAsync(double dt, List<WeatherRecord> nextData, List<WeatherRecord> currentData)
    {
        int n = nextData.Count;
        double[,] matrixA = new double[n, n];
        double[] vectorB_p = new double[n];
        
        var frozenCurrent = currentData.Select(r => new WeatherRecord
        {
            Pressure = r.Pressure,
            Temperature = r.Temperature,
            WindU = r.WindU,
            WindV = r.WindV,
            Region = r.Region
        }).ToList();

        for (int i = 0; i < n; i++)
        {
            var current = frozenCurrent[i];
            double hour = nextData[i].RecordedAt.Hour + (nextData[i].RecordedAt.Minute / 60.0);
            int dayOfYear = nextData[i].RecordedAt.DayOfYear;

            var (dpdx, dpdy) = Calculate2DGradients(i, frozenCurrent, r => r.Pressure);
            double laplacianP = Calculate2DLaplacian(i, frozenCurrent);
            
            double advectionP = -(current.WindU * dpdx + current.WindV * dpdy) * 0.15;
            double diffusionP = Kh_P * laplacianP;
            
            double thermalEffect = -(current.Temperature - 14.0) * 35.0;
            double regionPhase = (current.Region.Lat * 2.0) + (current.Region.Lon * 1.2);
            double wavePhase = ((dayOfYear + hour / 24.0) * 2.0 * Math.PI / 4.5) + regionPhase;
            double synopticWave = Math.Sin(wavePhase) * 450.0;

            double targetPressure = current.Pressure + thermalEffect + synopticWave;
            double relaxation = (targetPressure - current.Pressure) * 0.0015;

            double dp_dt = advectionP + diffusionP + relaxation;
            dp_dt = Math.Clamp(dp_dt, -0.02, 0.02); 

            matrixA[i, i] = 1.0;
            vectorB_p[i] = current.Pressure + (dt * dp_dt);
        }

        double[] initialGuessP = frozenCurrent.Select(r => r.Pressure).ToArray();
        double[] nextP = LinearSolver.SolveGaussSeidel(matrixA, vectorB_p, initialGuessP);

        for (int i = 0; i < n; i++)
        {
            double p = double.IsNaN(nextP[i]) ? currentData[i].Pressure : nextP[i];
            double finalPressure = Math.Clamp(p, 84000.0, 104000.0);
            
            nextData[i].Pressure = Math.Round((currentData[i].Pressure * 0.1) + (finalPressure * 0.9), 0); 
        }
    }

    private double Calculate2DLaplacian(int idx, List<WeatherRecord> nodes)
    {
        var curr = nodes[idx];
        double d = 50000.0;
        var east = nodes.Where(n => n.Region.Lon > curr.Region.Lon && Math.Abs(n.Region.Lat - curr.Region.Lat) < 0.3).OrderBy(n => n.Region.Lon).FirstOrDefault();
        var west = nodes.Where(n => n.Region.Lon < curr.Region.Lon && Math.Abs(n.Region.Lat - curr.Region.Lat) < 0.3).OrderByDescending(n => n.Region.Lon).FirstOrDefault();
        var north = nodes.Where(n => n.Region.Lat > curr.Region.Lat && Math.Abs(n.Region.Lon - curr.Region.Lon) < 0.3).OrderBy(n => n.Region.Lat).FirstOrDefault();
        var south = nodes.Where(n => n.Region.Lat < curr.Region.Lat && Math.Abs(n.Region.Lon - curr.Region.Lon) < 0.3).OrderByDescending(n => n.Region.Lat).FirstOrDefault();

        double pE = east?.Pressure ?? curr.Pressure;
        double pW = west?.Pressure ?? curr.Pressure;
        double pN = north?.Pressure ?? curr.Pressure;
        double pS = south?.Pressure ?? curr.Pressure;

        return (pE + pW + pN + pS - 4 * curr.Pressure) / (d * d);
    }

    private (double dx, double dy) Calculate2DGradients(int idx, List<WeatherRecord> nodes, Func<WeatherRecord, double> property)
    {
        var curr = nodes[idx];
        var east = nodes.Where(n => n.Region.Lon > curr.Region.Lon && Math.Abs(n.Region.Lat - curr.Region.Lat) < 0.4).OrderBy(n => n.Region.Lon).FirstOrDefault();
        var north = nodes.Where(n => n.Region.Lat > curr.Region.Lat && Math.Abs(n.Region.Lon - curr.Region.Lon) < 0.4).OrderBy(n => n.Region.Lat).FirstOrDefault();

        double gradX = 0, gradY = 0;
        if (east != null) {
            double dx = Math.Max((east.Region.Lon - curr.Region.Lon) * 80000, 20000); 
            gradX = (property(east) - property(curr)) / dx;
        }
        if (north != null) {
            double dy = Math.Max((north.Region.Lat - curr.Region.Lat) * 111000, 20000); 
            gradY = (property(north) - property(curr)) / dy;
        }
        return (gradX, gradY);
    }
}