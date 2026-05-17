import React from 'react';
import './WeatherGrid.css';

function GridCard({ icon, label, value, sub, unit }) {
    return (
        <div className="wg-card">
            <div className="wg-card-header">
                <span className="wg-card-icon">{icon}</span>
                <span className="wg-card-label">{label}</span>
            </div>
            <div className="wg-card-value">
                {value}
                {unit && <span className="wg-card-unit"> {unit}</span>}
            </div>
            {sub && <p className="wg-card-sub">{sub}</p>}
        </div>
    );
}

export default function WeatherGrid({ data }) {
    const cards = [
        { icon: '🌅', label: 'ЗАХІД СОНЦЯ',  value: data.sunset,       sub: `Схід: ${data.sunrise}` },
        { icon: '💨', label: 'ВІТЕР',        value: data.wind,         unit: 'м/с', sub: data.windDir },
        { icon: '🌧', label: 'ОПАДИ',        value: `${data.precipitation} мм`, sub: 'За добу' },
        { icon: '💧', label: 'ВОЛОГІСТЬ',    value: `${data.humidity}%` },
        { icon: '🌡', label: 'ВІДЧУВАЄТЬСЯ', value: `${data.feelsLike}°`, sub: 'Враховуючи вітер' },
        { icon: '⬆', label: 'ТИСК',         value: data.pressure,     sub: 'мм' },
    ];

    return (
        <div className="wg-grid">
            {cards.map((c, i) => (
                <GridCard key={i} {...c} />
            ))}
        </div>
    );
}