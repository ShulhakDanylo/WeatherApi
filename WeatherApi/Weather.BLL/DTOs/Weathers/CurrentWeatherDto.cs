namespace Weather.BLL.DTOs.Weathers;

public class CurrentWeatherDto
{
    // Головний заголовок
    public string RegionName { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public string Condition { get; set; } = string.Empty; // "Сонячно", "Хмарно"
    public double MinTempToday { get; set; }
    public double MaxTempToday { get; set; }

    // Віджет: Вітер
    public double WindSpeed { get; set; }

    // Віджет: Опади
    public double Precipitation { get; set; } // в мм
    public string PrecipitationDescription { get; set; } = string.Empty; // "Очікується за останні 24 год"

    // Віджет: Вологість
    public int Humidity { get; set; }

    // Віджет: Відчувається як
    public double FeelsLike { get; set; }
    public string FeelsLikeDescription { get; set; } = string.Empty; // "Вітер робить повітря холоднішим"

    // Віджет: Тиск
    public double Pressure { get; set; }
    public string PressureStatus { get; set; } = string.Empty; // "Стабільний протягом дня"

    // Віджет: Видимість
    public int Cloudiness { get; set; }
    public string CloudinessDescription { get; set; } = string.Empty; // "Сьогодні дуже ясно"

    // Віджет: Схід/Захід сонця
    public string Sunrise { get; set; } = string.Empty; // "06:17"
    public string Sunset { get; set; } = string.Empty;  // "19:42"
}