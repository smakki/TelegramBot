import React from 'react';
import Table from '../components/Table';

const Dashboard: React.FC = () => {
    return (
        <div>
            <h2>Dashboard</h2>
            <Table url="https://api.example.com/dashboard" />
        </div>
    );
};

export default Dashboard;
