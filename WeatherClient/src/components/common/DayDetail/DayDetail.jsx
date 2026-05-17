// src/components/common/DayDetail/DayDetail.jsx
import React from 'react';
import './DayDetail.css';
import WeatherIcon from '../WeatherIcon/WeatherIcon';

const StatCard = ({ icon, label, value, sub }) => (
    <div className="dd-stat">
        <div className="dd-stat-header">
            <span className="dd-stat-icon">{icon}</span>
            <span className="dd-stat-label">{label}</span>
        </div>
        <div className="dd-stat-value">{value}</div>
        {sub && <div className="dd-stat-sub">{sub}</div>}
    </div>
);

export default function DayDetail({ day }) {
    if (!day) return null;

    return (
        <div className="dd-panel">
            <div className="dd-header">
                <div className="dd-header-text">
                    <h3 className="dd-day-name">{day.dayOfWeek}</h3>
                    <p className="dd-condition">Детальний прогноз на добу</p>
                </div>
            </div>

            {day.hourly && (
                <div className="dd-hourly">
                    {day.hourly.map((h, i) => (
                        <div className="dd-hour" key={i}>
                            <span className="dd-hour-time">{h.time}</span>

                            {/* Передаємо реальний стан цієї години з бекенду */}
                            <span className="dd-hour-icon" style={{ width: '22px', height: '22px', display: 'inline-block' }}>
                                <WeatherIcon condition={h.condition} />
                            </span>

                            <span className="dd-hour-temp">{Math.round(h.temperature)}°</span>
                        </div>
                    ))}
                </div>
            )}

            <div className="dd-stats">
                <StatCard icon="🌅" label="ЗАХІД СОНЦЯ"  value={day.sunset} sub={`Схід: ${day.sunrise}`} />
                <StatCard icon="💨" label="ВІТЕР" value={`${day.avgWindSpeed} м/с`} sub={day.windDirection} />
                <StatCard icon="🌧" label="ОПАДИ" value={`${day.totalPrecipitation} мм`} sub="За добу" />
                <StatCard icon="💧" label="ВОЛОГІСТЬ" value={`${day.avgHumidity}%`} sub="Середня" />
                <StatCard icon="🌡" label="ВІДЧУВАЄТЬСЯ" value={`${Math.round(day.minTemp)}° / ${Math.round(day.maxTemp)}°`} sub="Діапазон" />
                <StatCard icon="⬆" label="ТИСК" value={day.avgPressure} sub="гПа" />
            </div>
        </div>
    );
}