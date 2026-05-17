import React, { useState, useEffect, useRef } from 'react';
import './Dashboard.css';
import TopBar from '../../components/common/TopBar/TopBar.jsx';
import HeroWeather from '../../components/common/HeroWeather/HeroWeather.jsx';
import HourlyForecast from '../../components/common/HourlyForecast/HourlyForecast.jsx';
import TenDayForecast from '../../components/common/TenDayForecast/TenDayForecast.jsx';
import WeatherGrid from '../../components/common/WeatherGrid/WeatherGrid.jsx';
import PageManager from '../../components/common/PageManager/PageManager.jsx';
import { weatherService } from '../../services/weatherService';
// Імпортуємо константу міст
import { AVAILABLE_CITIES } from '../../Constants/cityConfig.js';

export default function Dashboard({ user, onLogout }) {
    // Ініціалізуємо першим ID зі списку: "Grid_46,4_24,0"
    const [pages, setPages] = useState([AVAILABLE_CITIES[0].id]);
    const [activeIdx, setActiveIdx] = useState(0);
    const [weatherData, setWeatherData] = useState({});
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const cityId = pages[activeIdx];
        if (!cityId) return;

        async function fetchAllWeather() {
            // Якщо дані вже є, не показуємо лоадер на весь екран, щоб не блимало
            if (!weatherData[cityId]) setLoading(true);

            try {
                const current = await weatherService.getCurrentWeather(cityId);
                const daily = await weatherService.getDailyForecast(cityId);

                if (current && daily) {
                    setWeatherData(prev => ({
                        ...prev,
                        [cityId]: { current, daily }
                    }));
                }
            } catch (err) {
                // Виправляємо помилку "axios is not defined" - просто виводимо err
                console.error("Помилка завантаження погоди:", err);
            } finally {
                setLoading(false);
            }
        }

        fetchAllWeather();
    }, [activeIdx, pages]);

    const activeCityId = pages[activeIdx];
    const data = weatherData[activeCityId];

    const getCombinedHourly = () => {
        if (!data || !data.daily || data.daily.length < 2) {
            console.log("Даних за завтра ще немає у масиві daily");
            return data?.daily[0]?.hourly || [];
        }

        const today = data.daily[0].hourly || [];
        const tomorrow = data.daily[1].hourly || [];

        console.log("Сьогодні годин:", today.length, "Завтра годин:", tomorrow.length);

        return [...today, ...tomorrow].slice(0, 24);
    };
    const addPage = (cityId) => {
        if (!pages.includes(cityId)) {
            setPages([...pages, cityId]);
            setActiveIdx(pages.length);
        }
    };

    const removePage = (idx) => {
        if (pages.length <= 1) return;
        const newPages = pages.filter((_, i) => i !== idx);
        setPages(newPages);
        setActiveIdx(0);
    };

    if (loading && !data) return <div className="loader">Завантаження...</div>;
    if (!data) return <div>Дані для міста {activeCityId} не знайдені</div>;

    return (
        <div className="dash-wrapper">
            <TopBar onLogout={onLogout} />
            <main className="dash-main">
                <div className="dash-content">
                    <HeroWeather data={{
                        city: data.current.regionName,
                        temp: Math.round(data.current.temperature),
                        condition: data.current.condition,
                        high: Math.round(data.current.maxTempToday),
                        low: Math.round(data.current.minTempToday)
                    }} />

                    <HourlyForecast
                        summary={data.current.cloudinessDescription}
                        hourly={data.daily[0]?.hourly || []}
                    />

                    <TenDayForecast days={data.daily} />

                    <WeatherGrid data={{
                        sunset: data.current.sunset,
                        sunrise: data.current.sunrise,
                        wind: data.current.windSpeed,
                        windDir: data.daily[0]?.windDirection,
                        precipitation: data.current.precipitation,
                        humidity: data.current.humidity,
                        feelsLike: data.current.feelsLike,
                        pressure: data.current.pressure,
                        dewPoint: Math.round(data.current.temperature - 2)
                    }} />
                </div>

                <PageManager
                    pages={pages.map(id => ({
                        id,
                        city: AVAILABLE_CITIES.find(c => c.id === id)?.label || "Місто"
                    }))}
                    activeIndex={activeIdx}
                    onSwitch={setActiveIdx}
                    onAdd={addPage}
                    onRemove={removePage}
                />
            </main>
        </div>
    );
}