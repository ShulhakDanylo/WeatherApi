import React, { useState, useEffect, useRef } from 'react';
import './PageManager.css';
import { AVAILABLE_CITIES } from '../../../Constants/cityConfig.js';

const MAX_PAGES = 5;


export default function PageManager({ pages, activeIndex, onSwitch, onAdd, onRemove }) {
    const [showPicker, setShowPicker] = useState(false);
    const pickerRef = useRef(null);
    const usedIds   = pages.map(p => p.id);
    const available = AVAILABLE_CITIES.filter(c => !usedIds.includes(c.id));

    const canPrev = activeIndex > 0;
    const canNext = activeIndex < pages.length - 1;

    useEffect(() => {
        const handler = (e) => {
            if (pickerRef.current && !pickerRef.current.contains(e.target)) {
                setShowPicker(false);
            }
        };
        document.addEventListener('mousedown', handler);
        return () => document.removeEventListener('mousedown', handler);
    }, []);

    return (
        <div className="pm-bar">

            {/* ── Navigation row: arrow · dots · arrow ── */}
            <div className="pm-nav">

                {/* Left arrow */}
                <button
                    className={`pm-arrow${canPrev ? '' : ' pm-arrow--hidden'}`}
                    onClick={() => canPrev && onSwitch(activeIndex - 1)}
                    aria-label="Назад"
                >
                    <svg width="18" height="18" viewBox="0 0 18 18" fill="none">
                        <path d="M11 4L6 9l5 5" stroke="currentColor" strokeWidth="1.8"
                              strokeLinecap="round" strokeLinejoin="round"/>
                    </svg>
                </button>

                {/* Dots */}
                <div className="pm-dots">
                    {pages.map((page, i) => (
                        <button
                            key={page.id}
                            className={`pm-dot${i === activeIndex ? ' pm-dot--active' : ''}`}
                            onClick={() => onSwitch(i)}
                            aria-label={page.city}
                            title={page.city}
                        />
                    ))}
                </div>

                {/* Right arrow */}
                <button
                    className={`pm-arrow${canNext ? '' : ' pm-arrow--hidden'}`}
                    onClick={() => canNext && onSwitch(activeIndex + 1)}
                    aria-label="Вперед"
                >
                    <svg width="18" height="18" viewBox="0 0 18 18" fill="none">
                        <path d="M7 4l5 5-5 5" stroke="currentColor" strokeWidth="1.8"
                              strokeLinecap="round" strokeLinejoin="round"/>
                    </svg>
                </button>
            </div>

            {/* City name label */}
            <span className="pm-city-label">{pages[activeIndex]?.city}</span>

            {/* ── Add button (+ circle) ── */}
            {pages.length < MAX_PAGES && (
                <div className="pm-add-wrap" ref={pickerRef}>
                    <button
                        className={`pm-add-btn${showPicker ? ' pm-add-btn--open' : ''}`}
                        onClick={() => setShowPicker(v => !v)}
                        aria-label="Додати місто"
                    >
                        <svg width="14" height="14" viewBox="0 0 14 14" fill="none">
                            <line x1="7" y1="1" x2="7" y2="13" stroke="currentColor" strokeWidth="2" strokeLinecap="round"/>
                            <line x1="1" y1="7" x2="13" y2="7" stroke="currentColor" strokeWidth="2" strokeLinecap="round"/>
                        </svg>
                    </button>

                    {showPicker && available.length > 0 && (
                        <div className="pm-picker">
                            {available.map(city => (
                                <button
                                    key={city.id}
                                    className="pm-picker-item"
                                    onClick={() => { onAdd(city.id); setShowPicker(false); }}
                                >
                                    <span className="pm-picker-emoji">{city.emoji}</span>
                                    <span className="pm-picker-name">{city.label}</span>
                                </button>
                            ))}
                        </div>
                    )}
                </div>
            )}

            {/* Remove current page (only if more than 1) */}
            {pages.length > 1 && (
                <button
                    className="pm-remove-btn"
                    onClick={() => onRemove(activeIndex)}
                    title="Видалити цю сторінку"
                >
                    Видалити {pages[activeIndex]?.city}
                </button>
            )}
        </div>
    );
}
