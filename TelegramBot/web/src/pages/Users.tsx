import React from 'react';
import Table from '../components/Table.tsx';
import Form from '../components/Form.tsx';

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
