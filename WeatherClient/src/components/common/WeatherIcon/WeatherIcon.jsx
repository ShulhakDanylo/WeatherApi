// src/components/common/WeatherIcon/WeatherIcon.jsx
import React from 'react';

export default function WeatherIcon({ condition, className = "" }) {
    const cond = condition?.toLowerCase().trim() || "";
    const isNight = cond.includes('ніч');

    // 1. ГРОЗА (Універсальна для дня і ночі)
    if (cond.includes('гроза')) {
        return (
            <svg className={className} viewBox="0 0 24 24" width="100%" height="100%" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M19 16.9A5 5 0 0 0 18 7h-1.26a8 8 0 1 0-11.62 8.58" fill="#78909C" stroke="#455A64" />
                <polyline points="13 11 9 17 12 17 11 23 15 17 12 17 13 11" fill="#FFD54F" stroke="#FFB300" />
            </svg>
        );
    }

    // 2. МІНЛИВА ХМАРНІСТЬ + ДОЩ (Дощ із проясненнями)
    if (cond.includes('дощ') && cond.includes('мінлива хмарність')) {
        return (
            <svg className={className} viewBox="0 0 24 24" width="100%" height="100%" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
                {isNight ? (
                    /* НІЧ: Місяць визирає з-за хмари + дощ */
                    <path d="M11 4a5 5 0 0 0 6 6 6 6 0 1 1-6-6Z" fill="#B0BEC5" stroke="#78909C"/>
                ) : (
                    /* ДЕНЬ: Сонце визирає з-за хмари + дощ */
                    <circle cx="8.5" cy="7" r="4.5" fill="#FFA726" stroke="#FB8C00"/>
                )}
                <path d="M20 17.5A4.5 4.5 0 0 0 18.5 9h-0.8A6.5 6.5 0 1 0 6 15.5H19a1 1 0 0 0 1-1Z" fill="#ECEFF1" stroke="#B0BEC5"/>
                <line x1="10" y1="18" x2="9" y2="21" stroke="#29B6F6" strokeWidth="2"/>
                <line x1="14" y1="18" x2="13" y2="21" stroke="#29B6F6" strokeWidth="2"/>
            </svg>
        );
    }

    // 3. СИЛЬНИЙ ЗАТЯЖНИЙ ДОЩ (Суцільна хмарність, місяця/сонця все одно не видно)
    if (cond.includes('дощ')) {
        return (
            <svg className={className} viewBox="0 0 24 24" width="100%" height="100%" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M19 16.9A5 5 0 0 0 18 7h-1.26a8 8 0 1 0-11.62 8.58" fill="#90A4AE" stroke="#546E7A"/>
                <line x1="12" y1="18" x2="10" y2="22" stroke="#29B6F6" strokeWidth="2"/>
                <line x1="8" y1="18" x2="6" y2="22" stroke="#29B6F6" strokeWidth="2"/>
                <line x1="16" y1="18" x2="14" y2="22" stroke="#29B6F6" strokeWidth="2"/>
            </svg>
        );
    }

    // 4. МІНЛИВА ХМАРНІСТЬ (Без опадів)
    if (cond.includes('мінлива хмарність')) {
        return (
            <svg className={className} viewBox="0 0 24 24" width="100%" height="100%" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
                {isNight ? (
                    /* НІЧ: Місяць з-за хмари */
                    <path d="M11 4a5 5 0 0 0 6 6 6 6 0 1 1-6-6Z" fill="#CFD8DC" stroke="#90A4AE"/>
                ) : (
                    /* ДЕНЬ: Сонце з-за хмари */
                    <>
                        <circle cx="8.5" cy="7" r="5.5" fill="#FFA726" stroke="#FB8C00"/>
                        <path d="M8.5 1v1.5M3.2 3.2l1.1 1.1M1 8.5h1.5" stroke="#FB8C00"/>
                    </>
                )}
                <path d="M20 17.5A4.5 4.5 0 0 0 18.5 9h-0.8A6.5 6.5 0 1 0 6 15.5H19a1 1 0 0 0 1-1Z" fill="#ECEFF1" stroke="#B0BEC5"/>
            </svg>
        );
    }

    // 5. ПОХМУРО (Суцільні сірі хмари для дня і ночі)
    if (cond.includes('похмуро')) {
        return (
            <svg className={className} viewBox="0 0 24 24" width="100%" height="100%" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M16 14a3 3 0 0 0-3-3h-0.5A4.5 4.5 0 0 0 4 14.5" fill="#CFD8DC" stroke="#90A4AE"/>
                <path d="M19 16.9A5 5 0 0 0 18 7h-1.26a8 8 0 1 0-11.62 8.58" fill="#ECEFF1" stroke="#B0BEC5"/>
            </svg>
        );
    }

    // 6. ЧИСТЕ НЕБО (ЯСНО / СОНЯЧНО)
    return (
        <svg className={className} viewBox="0 0 24 24" width="100%" height="100%" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            {isNight ? (
                /* НІЧ: Гарний неоновий Місяць */
                <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79Z" fill="#FFF59D" stroke="#FBC02D"/>
            ) : (
                /* ДЕНЬ: Яскраве Сонце */
                <>
                    <circle cx="12" cy="12" r="5.5" fill="#FFA726" stroke="#FB8C00"/>
                    <path d="M12 1v2M12 21v2M4.22 4.22l1.42 1.42M18.36 18.36l1.42 1.42M1 12h2M21 12h2M4.22 19.78l1.42-1.42M18.36 5.64l1.42-1.42" stroke="#FB8C00"/>
                </>
            )}
        </svg>
    );
}