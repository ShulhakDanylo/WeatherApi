import React, { useState } from 'react';
import LoginForm    from '../../components/auth/LoginForm/LoginForm.jsx';
import RegisterForm from '../../components/auth/RegisterForm/RegisterForm.jsx';
import './LandingPage.css';

/* ── Icons ── */
const SunIcon = () => (
    <svg width="38" height="38" viewBox="0 0 38 38" fill="none">
        <circle cx="19" cy="19" r="8" fill="#FACC15" stroke="#FDE68A" strokeWidth="1.5"/>
        {[0,45,90,135,180,225,270,315].map((angle, i) => {
            const rad = (angle * Math.PI) / 180;
            return (
                <line key={i}
                      x1={19 + 12 * Math.cos(rad)} y1={19 + 12 * Math.sin(rad)}
                      x2={19 + 16 * Math.cos(rad)} y2={19 + 16 * Math.sin(rad)}
                      stroke="#FACC15" strokeWidth="2.2" strokeLinecap="round"
                />
            );
        })}
    </svg>
);

const features = [
    {
        title: 'Точність',
        desc: 'Гіперлокальні дані про температуру з точністю до вашого будинку.',
        icon: (
            <svg width="22" height="22" viewBox="0 0 22 22" fill="none">
                <rect x="8" y="3" width="6" height="13" rx="3" stroke="#64748b" strokeWidth="1.4"/>
                <rect x="9.5" y="10" width="3" height="5" rx="1.5" fill="#64748b"/>
                <path d="M11 2v1M11 18v1M3 11h1M18 11h1" stroke="#64748b" strokeWidth="1.4" strokeLinecap="round"/>
            </svg>
        ),
    },
    {
        title: 'Вітер',
        desc: 'Детальні карти вітрів та прогноз поривів для вашої безпеки.',
        icon: (
            <svg width="22" height="22" viewBox="0 0 22 22" fill="none">
                <path d="M2 8c2-2 4-2 6 0s4 2 6 0 4-2 6 0" stroke="#64748b" strokeWidth="1.4" strokeLinecap="round" fill="none"/>
                <path d="M2 13c2-2 4-2 6 0s4 2 6 0" stroke="#64748b" strokeWidth="1.4" strokeLinecap="round" fill="none"/>
            </svg>
        ),
    },
    {
        title: 'Опади',
        desc: 'Миттєві сповіщення про дощ чи сніг у вашому регіоні.',
        icon: (
            <svg width="22" height="22" viewBox="0 0 22 22" fill="none">
                <path d="M5 10c0-3 2.5-5 5-5s5 2 5 5h1a3 3 0 010 6H4a3 3 0 010-6h1z" stroke="#64748b" strokeWidth="1.4" fill="none"/>
                <path d="M8 18l-1 2M11 18l-1 2M14 18l-1 2" stroke="#64748b" strokeWidth="1.4" strokeLinecap="round"/>
            </svg>
        ),
    },
    {
        title: 'УФ-індекс',
        desc: 'Захистіть себе завдяки актуальним даним про сонячну активність.',
        icon: (
            <svg width="22" height="22" viewBox="0 0 22 22" fill="none">
                <circle cx="11" cy="11" r="4" stroke="#64748b" strokeWidth="1.4"/>
                {[0,45,90,135,180,225,270,315].map((a,i) => {
                    const r = (a * Math.PI) / 180;
                    return <line key={i}
                                 x1={11+7*Math.cos(r)} y1={11+7*Math.sin(r)}
                                 x2={11+9*Math.cos(r)} y2={11+9*Math.sin(r)}
                                 stroke="#64748b" strokeWidth="1.4" strokeLinecap="round"/>;
                })}
            </svg>
        ),
    },
];

export default function LandingPage({ onAuthSuccess }) {
    const [modal, setModal] = useState(null);

    const closeModal = () => setModal(null);

    return (
        <div className="lp-wrapper">

            {/* ── Navbar ── */}
            <nav className="lp-navbar">
                <a className="lp-logo" href="/">
                    <span className="lp-logo-icon">
                        <svg width="26" height="26" viewBox="0 0 26 26" fill="none">
                            <circle cx="13" cy="13" r="12" fill="#E0F0FF" stroke="#4AB3F4" strokeWidth="1.2"/>
                            <circle cx="13" cy="11" r="3" fill="#4AB3F4"/>
                            <path d="M7 18c0-3.3 2.7-6 6-6s6 2.7 6 6"
                                  stroke="#4AB3F4" strokeWidth="1.8" strokeLinecap="round" fill="none"/>
                        </svg>
                    </span>
                    <span className="lp-logo-text">WeatherFlow</span>
                </a>
                <div className="lp-nav-actions">
                    <button className="lp-btn-login" onClick={() => setModal('login')}>Login</button>
                    <button className="lp-btn-signup" onClick={() => setModal('register')}>Sign Up</button>
                </div>
            </nav>

            {/* ── Hero ── */}
            <section className="lp-hero">
                <div className="lp-hero-inner">
                    <div className="lp-sun-icon"><SunIcon /></div>
                    <h1 className="lp-hero-title">
                        Погода у ваших<br /><strong>руках</strong>
                    </h1>
                    <p className="lp-hero-sub">
                        WeatherFlow — ваш персональний гід у світі погоди.<br />
                        Точні прогнози, яскраві візуалізації та миттєві сповіщення.
                    </p>
                    <div className="lp-hero-btns">
                        <button className="lp-btn-create"   onClick={() => setModal('register')}>Створити акаунт</button>
                        <button className="lp-btn-enter"   onClick={() => setModal('login')}>Увійти</button>
                    </div>
                </div>
            </section>

            {/* ── Features ── */}
            <section className="lp-features">
                <div className="lp-features-grid">
                    {features.map((f) => (
                        <div className="lp-card" key={f.title}>
                            <div className="lp-card-icon">{f.icon}</div>
                            <h3>{f.title}</h3>
                            <p>{f.desc}</p>
                        </div>
                    ))}
                </div>
            </section>

            {/* ── Auth modals (рендеряться поверх лендінгу) ── */}
            {modal === 'login' && (
                <LoginForm
                    onClose={closeModal}
                    onAuthSuccess={onAuthSuccess} // Передаємо функцію з App.js
                    onSwitchToRegister={() => setModal('register')}
                />
            )}

            {modal === 'register' && (
                <RegisterForm
                    onClose={closeModal}
                    onAuthSuccess={onAuthSuccess} // Передаємо функцію з App.js
                    onSwitchToLogin={() => setModal('login')}
                />
            )}
        </div>
    );
}
