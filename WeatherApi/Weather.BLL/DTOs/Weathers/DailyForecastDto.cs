namespace Weather.BLL.DTOs.Weathers;

public record DailyForecastDto(
    string DayOfWeek,
    double MinTemp,
    double MaxTemp,
    DateTime Date,
    double AvgWindSpeed,
    string WindDirection,
    double TotalPrecipitation,
    int AvgHumidity,
    double AvgPressure,
    double Visibility,
    string Sunrise,
    string Sunset,
    string Condition,
    List<HourlyForecastDto> Hourly
);
public record HourlyForecastDto(string Time, double Temperature,  string Condition);