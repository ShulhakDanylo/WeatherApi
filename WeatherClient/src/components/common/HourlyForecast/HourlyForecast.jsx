// src/components/common/HourlyForecast/HourlyForecast.jsx
import React from 'react';
import './HourlyForecast.css';
import WeatherIcon from '../WeatherIcon/WeatherIcon';

export default function HourlyForecast({ summary, hourly }) {
    if (!hourly || hourly.length === 0) {
        return (
            <div className="hourly-card">
                <p className="hourly-summary">{summary}</p>
                <p style={{ padding: '20px', textAlign: 'center', opacity: 0.5 }}>
                    Погодинний прогноз очікується...
                </p>
            </div>
        );
    }

    return (
        <div className="hourly-card">
            <p className="hourly-summary">{summary}</p>
            <div className="hourly-list">
                {hourly.map((item, i) => (
                    <div key={i} className="hourly-item">
                        <span className="hourly-label">{item.time || item.label}</span>

                        {/* Передаємо точний стан години, який прийшов з C# мапера */}
                        <span className="hourly-icon" style={{ width: '26px', height: '26px', display: 'inline-block' }}>
                            <WeatherIcon condition={item.condition} />
                        </span>

                        <span className="hourly-temp">
                            {Math.round(item.temperature || item.temp)}°
                        </span>
                    </div>
                ))}
            </div>
        </div>
    );
}