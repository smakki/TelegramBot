import React, { useState } from 'react';
import { Route, Routes, Navigate } from 'react-router-dom';
import Login from './components/Login';
import Sidebar from './components/Sidebar';
import Dashboard from './pages/Dashboard';
import Users from './pages/Users';
import Settings from './pages/Settings';

const App: React.FC = () => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    const handleLogin = () => {
        setIsAuthenticated(true);
    };

    return (
        <div>
            {isAuthenticated ? (
                <div style={{ display: 'flex' }}>
                    <Sidebar />
                    <div style={{ flex: 1, padding: '20px' }}>
                        <Routes>
                            <Route path="/dashboard" element={<Dashboard />} />
                            <Route path="/users" element={<Users />} />
                            <Route path="/settings" element={<Settings />} />
                            <Route path="*" element={<Navigate to="/dashboard" />} />
                        </Routes>
                    </div>
                </div>
            ) : (
                <Login onLogin={handleLogin} />
            )}
        </div>
    );
};

export default App;