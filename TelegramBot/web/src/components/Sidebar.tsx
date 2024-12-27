import React from 'react';
import { Link } from 'react-router-dom';

const Sidebar: React.FC = () => {
    return (
        <div style={{ width: '200px', background: '#f4f4f4', padding: '20px' }}>
            <ul>
                <li><Link to="/dashboard">Dashboard</Link></li>
                <li><Link to="/users">Users</Link></li>
                <li><Link to="/settings">Settings</Link></li>
            </ul>
        </div>
    );
};

export default Sidebar;
