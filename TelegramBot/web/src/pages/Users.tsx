import React from 'react';
import Table from '../components/Table';
import Form from '../components/Form';

const Users: React.FC = () => {
    return (
        <div>
            <h2>Users</h2>
            <Table url="https://api.example.com/users" />
            <Form url="https://api.example.com/users" />
        </div>
    );
};

export default Users;
