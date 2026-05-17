import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import LandingPage from './pages/LandingPage/LandingPage.jsx';
import Dashboard from "./pages/DashBoard/DashBoard.jsx";

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const savedUser = localStorage.getItem('user');
        if (savedUser) {
            setUser(JSON.parse(savedUser));
            setIsAuthenticated(true);
        }
        setLoading(false);
    }, []);

    const handleAuthSuccess = (userData) => {
        localStorage.setItem('user', JSON.stringify(userData));
        setUser(userData);
        setIsAuthenticated(true);
    };

    const handleLogout = () => {
        localStorage.removeItem('user');
        setIsAuthenticated(false);
        setUser(null);
    };

    if (loading) return <div className="app-loader">Завантаження профілю...</div>;

    return (
        <Router>
            <Routes>
                <Route path="/" element={
                    !isAuthenticated ?
                        <LandingPage onAuthSuccess={handleAuthSuccess} /> :
                        <Navigate to="/dashboard" replace />
                } />

                <Route path="/dashboard" element={
                    isAuthenticated ?
                        <Dashboard user={user} onLogout={handleLogout} /> :
                        <Navigate to="/" replace />
                } />
            </Routes>
        </Router>
    );
}

export default App;