import React from 'react';
import Form from '../components/Form.tsx';

const Settings: React.FC = () => {
    return (
        <div>
            <h2>Settings</h2>
            <Form url="https://api.example.com/settings" />
        </div>
    );
};

export default Settings;
