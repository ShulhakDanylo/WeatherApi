import React from 'react';
import './HeroWeather.css';

export default function HeroWeather({ data }) {
    const { city, temp, condition, high, low } = data;

    return (
        <section className="hero">
            <div className="hero-location">
                <svg width="14" height="14" viewBox="0 0 14 14" fill="none" className="hero-location-icon">
                    <path d="M7 1a4.5 4.5 0 014.5 4.5C11.5 8.5 7 13 7 13S2.5 8.5 2.5 5.5A4.5 4.5 0 017 1z"
                          stroke="rgba(255,255,255,0.9)" strokeWidth="1.3" fill="none"/>
                    <circle cx="7" cy="5.5" r="1.5" fill="rgba(255,255,255,0.9)"/>
                </svg>
                <span className="hero-city">{city}</span>
            </div>

            <div className="hero-temp">{temp}</div>

            <div className="hero-condition">{condition}</div>
            <div className="hero-range">
                М: {high}° &nbsp; Н: {low}°
            </div>
        </section>
    );
}
