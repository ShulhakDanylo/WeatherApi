import React, { useState } from 'react';
import './LoginForm.css';
import { authService } from '../../../services/authService.js';

const UserIcon = () => (
    <svg width="18" height="18" viewBox="0 0 18 18" fill="none">
        <circle cx="9" cy="7" r="3.5" stroke="rgba(255,255,255,0.50)" strokeWidth="1.4"/>
        <path d="M2 16c0-3.9 3.1-7 7-7s7 3.1 7 7"
              stroke="rgba(255,255,255,0.50)" strokeWidth="1.4" strokeLinecap="round" fill="none"/>
    </svg>
);

const GoogleIcon = () => (
    <svg width="15" height="15" viewBox="0 0 18 18" fill="none">
        <path d="M17.1 9.2c0-.6-.1-1.2-.2-1.7H9v3.3h4.6c-.2 1-.8 1.9-1.7 2.5v2h2.7c1.6-1.5 2.5-3.6 2.5-6.1z" fill="rgba(255,255,255,0.85)"/>
        <path d="M9 18c2.4 0 4.4-.8 5.9-2.1l-2.7-2c-.8.5-1.9.9-3.2.9-2.4 0-4.5-1.6-5.2-3.9H1v2.1C2.5 16.1 5.5 18 9 18z" fill="rgba(255,255,255,0.85)"/>
        <path d="M3.8 10.9c-.2-.5-.3-1.1-.3-1.7s.1-1.2.3-1.7V5.4H1C.4 6.5 0 7.7 0 9s.4 2.5 1 3.6l2.8-1.7z" fill="rgba(255,255,255,0.85)"/>
        <path d="M9 3.6c1.3 0 2.5.5 3.4 1.3l2.6-2.6C13.4.9 11.4 0 9 0 5.5 0 2.5 1.9 1 4.6l2.8 2.1C4.5 5.2 6.6 3.6 9 3.6z" fill="rgba(255,255,255,0.85)"/>
    </svg>
);

const AppleIcon = () => (
    <svg width="15" height="15" viewBox="0 0 18 18" fill="none">
        <path d="M14.2 9.5c0-2.5 2-3.7 2.1-3.8-1.1-1.6-2.9-1.8-3.5-1.9-1.5-.2-2.9.9-3.7.9-.7 0-1.9-.9-3.1-.8C4.4 4 3 4.8 2.2 6.1c-1.6 2.7-.4 6.8 1.1 9 .8 1.1 1.7 2.3 2.9 2.2 1.1 0 1.6-.7 3-.7s1.8.7 3 .7c1.3 0 2.1-1.1 2.8-2.2.9-1.3 1.3-2.5 1.3-2.6-.1 0-2.1-.8-2.1-3zM11.7 2.7c.6-.7 1-1.8.9-2.8-.9.1-2 .6-2.6 1.3-.6.6-1.1 1.7-1 2.7 1 .1 2-.5 2.7-1.2z" fill="rgba(255,255,255,0.85)"/>
    </svg>
);

export default function LoginForm({ onClose, onAuthSuccess, onSwitchToRegister }) {
    const [email, setEmail] = useState(''); // Використовуємо email для логіну
    const [loading, setLoading] = useState(false);

    const handleSubmit = async () => {
        if (!email.trim()) return;
        setLoading(true);

        try {
            // Викликаємо бекенд
            const data = await authService.login(email.trim());

            // Якщо бекенд повернув дані користувача
            if (onAuthSuccess) {
                onAuthSuccess(data);
            }
            onClose();
        } catch (err) {
            console.error(err);
            alert("Користувача не знайдено або помилка сервера");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-overlay" onClick={onClose}>
            <div className="auth-card" onClick={e => e.stopPropagation()}>

                <button className="auth-close" onClick={onClose}>✕</button>

                <span className="auth-badge">WeatherFlow</span>
                <h2 className="auth-title">Вхід до акаунту</h2>
                <p className="auth-sub">Введіть ваш емаіл щоб продовжити</p>

                <div className="auth-field">
                    <input
                        className="auth-input"
                        type="email"
                        placeholder="Введіть ваш Email"
                        value={email}
                        onChange={e => setEmail(e.target.value)}
                        onKeyDown={e => e.key === 'Enter' && handleSubmit()}
                        autoFocus
                    />
                </div>

                <button
                    className="auth-btn-primary"
                    onClick={handleSubmit}
                    disabled={!email.trim() || loading}
                >
                    {loading ? "Вхід..." : "Увійти →"}
                </button>

                <p className="auth-switch-hint">
                    Немає акаунту?{' '}
                    <button className="auth-link" onClick={onSwitchToRegister}>Зареєструватись</button>
                </p>
            </div>
        </div>
    );
}
