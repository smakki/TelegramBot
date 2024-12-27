import React, { useEffect, useState } from 'react';
import axiosInstance from './axiosInstance';

interface TableProps<T> {
    url: string;
    renderItem: (item: T) => React.ReactNode;
    renderHeader: (item: T) => React.ReactNode;
}

const Table = <T,>({ url, renderItem, renderHeader }: TableProps<T>) => {
    const [data, setData] = useState<T[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        axiosInstance.get(url)
            .then(response => {
                if (Array.isArray(response.data)) {
                    setData(response.data);
                } else {
                    setError('Data is not an array');
                }
                setLoading(false);
            })
            .catch(error => {
                setError(error.message);
                setLoading(false);
            });
    }, [url]);

    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error}</div>;
    }

    return (
        <table>
            <thead>
            <tr>
                {data[0] && renderHeader(data[0])}
            </tr>
            </thead>
            <tbody>
            {data.map((item, index) => (
                <tr key={index}>
                    {renderItem(item)}
                </tr>
            ))}
            </tbody>
        </table>
    );
};

export default Table;
