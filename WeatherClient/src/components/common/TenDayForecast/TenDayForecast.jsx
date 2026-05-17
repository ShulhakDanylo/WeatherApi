// src/components/common/TenDayForecast/TenDayForecast.jsx
import React, { useState } from 'react';
import './TenDayForecast.css';
import DayDetail from '../DayDetail/DayDetail';
import WeatherIcon from '../WeatherIcon/WeatherIcon'; // Імпортуємо наш новий розумний SVG компонент

// Функція для розрахунку позиції смужки температури (шкала від -10 до +35)
const toPercent = (v) => {
    const min = -10;
    const max = 35;
    return Math.round(((v - min) / (max - min)) * 100);
};

export default function SevenDayForecast({ days }) {
    const [activeDate, setActiveDate] = useState(null);

    // Обмежуємо масив до 7 днів
    const forecastDays = days ? days.slice(0, 7) : [];

    return (
        <div className="tdf-card">
            <div className="tdf-title-row">
                <span className="tdf-title-icon">📅</span>
                <span className="tdf-title-text">7-ДЕННИЙ ПРОГНОЗ</span>
            </div>

            <div className="tdf-list">
                {forecastDays.map((day, index) => {
                    const isExpanded = activeDate === day.date;

                    // Розрахунок ширини смужки температури
                    const leftShift = toPercent(day.minTemp);
                    const widthBar = toPercent(day.maxTemp) - leftShift;

                    return (
                        <React.Fragment key={day.date}>
                            <button
                                className={`tdf-row ${isExpanded ? 'tdf-row--active' : ''}`}
                                onClick={() => setActiveDate(isExpanded ? null : day.date)}
                                aria-expanded={isExpanded}
                            >
                                <span className="tdf-day">{day.dayOfWeek}</span>

                                {/* Динамічна розумна SVG іконка */}
                                <span className="tdf-icon" style={{ width: '28px', height: '28px', display: 'inline-block' }}>
                                    <WeatherIcon condition={day.condition} />
                                </span>

                                <span className="tdf-low">{Math.round(day.minTemp)}°</span>

                                <div className="tdf-bar-track">
                                    <div
                                        className="tdf-bar-fill"
                                        style={{
                                            left: `${leftShift}%`,
                                            width: `${widthBar}%`
                                        }}
                                    />
                                </div>

                                <span className="tdf-high">{Math.round(day.maxTemp)}°</span>

                                <span className={`tdf-chevron ${isExpanded ? 'tdf-chevron--up' : ''}`}>
                                    <svg width="12" height="12" viewBox="0 0 14 14" fill="none">
                                        <path d="M4 5.5l3 3 3-3" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round"/>
                                    </svg>
                                </span>
                            </button>

                            {isExpanded && (
                                <div className="tdf-detail-panel">
                                    {/* Передаємо об'єкт дня в деталі */}
                                    <DayDetail day={day} />
                                </div>
                            )}
                        </React.Fragment>
                    );
                })}
            </div>
        </div>
    );
}