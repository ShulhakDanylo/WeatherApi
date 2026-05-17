import React from 'react';
import './TopBar.css';

export default function TopBar({ onLogout }) {
    return (
        <header className="topbar">
            {/* ── Logo ── */}
            <a className="topbar-brand" href="/">
                <span className="topbar-brand-icon">
                    <svg width="30" height="30" viewBox="0 0 30 30" fill="none">
                        <circle cx="15" cy="15" r="14" fill="#E0F7FF" stroke="#29B6F6" strokeWidth="1.5"/>
                        <circle cx="15" cy="13" r="3.8" fill="#0288D1"/>
                        <path d="M8 22c0-3.9 3.1-7 7-7s7 3.1 7 7"
                              stroke="#0288D1" strokeWidth="2" strokeLinecap="round" fill="none"/>
                    </svg>
                </span>
                <span className="topbar-brand-name">WeatherFlow</span>
            </a>

            {/* ── Logout ── */}
            <button className="topbar-logout" onClick={onLogout} title="Вийти">
                <svg width="18" height="18" viewBox="0 0 18 18" fill="none">
                    <path d="M7 2H3a1 1 0 00-1 1v12a1 1 0 001 1h4"
                          stroke="rgba(255,255,255,0.85)" strokeWidth="1.5" strokeLinecap="round"/>
                    <path d="M12 13l4-4-4-4"
                          stroke="rgba(255,255,255,0.85)" strokeWidth="1.5"
                          strokeLinecap="round" strokeLinejoin="round"/>
                    <line x1="16" y1="9" x2="7" y2="9"
                          stroke="rgba(255,255,255,0.85)" strokeWidth="1.5" strokeLinecap="round"/>
                </svg>
                <span>Вийти</span>
            </button>
        </header>
    );
}
