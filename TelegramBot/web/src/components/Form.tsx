import React, { useState } from 'react';
import axios from 'axios';

interface FormProps {
    url: string;
}

const Form: React.FC<FormProps> = ({ url }) => {
    const [formData, setFormData] = useState({});

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        axios.post(url, formData)
            .then(response => console.log('Form submitted:', response.data))
            .catch(error => console.error('Error submitting form:', error));
    };

    return (
        <form onSubmit={handleSubmit}>
            <div>
                <label>Field 1:</label>
                <input type="text" name="field1" onChange={handleChange} />
            </div>
            <div>
                <label>Field 2:</label>
                <input type="text" name="field2" onChange={handleChange} />
            </div>
            <button type="submit">Submit</button>
        </form>
    );
};

export default Form;
