import React, { useState, ReactElement } from 'react';
import '../index.css';

interface DataTableProps {
    tableData: { [key: string]: string | number | ReactElement }[];
    caption: string;
}

const DataTable: React.FC<DataTableProps> = ({ tableData, caption }) => {

    const [sortConfig, setSortConfig] = useState<{ key: string, direction: 'ascending' | 'descending' } | null>(null);

    const sortedTableData = React.useMemo(() => {
        let sortableItems = [...tableData];
        if (sortConfig !== null) {
            sortableItems.sort((a, b) => {
                if (a[sortConfig.key] < b[sortConfig.key]) {
                    return sortConfig.direction === 'ascending' ? -1 : 1;
                }
                if (a[sortConfig.key] > b[sortConfig.key]) {
                    return sortConfig.direction === 'ascending' ? 1 : -1;
                }
                return 0;
            });
        }
        return sortableItems;
    }, [tableData, sortConfig]);

    const requestSort = (key: string) => {
        let direction: 'ascending' | 'descending' = 'ascending';
        if (sortConfig && sortConfig.key === key && sortConfig.direction === 'ascending') {
            direction = 'descending';
        }
        setSortConfig({ key, direction });
    };
    if (!tableData || tableData.length === 0) {
        return <div>Нет данных</div>;
    }
    return (
        <div className="overflow-x-auto p-4">
            <table className="min-w-full bg-white shadow-md rounded-lg">
                <caption className="caption-top text-lg font-semibold mb-2 text-left">{caption}</caption>
                <thead className="bg-gray-50">
                <tr>
                    {Object.keys(tableData[0]).map((key) => (
                        <th
                            key={key}
                            onClick={() => requestSort(key)}
                            className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
                        >
                            {key}
                            {sortConfig?.key === key ? (
                                sortConfig.direction === 'ascending' ? ' ▲' : ' ▼'
                            ) : null}
                        </th>
                    ))}
                </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                {sortedTableData.map((row, rowIndex) => (
                    <tr key={rowIndex}>
                        {Object.keys(row).map((key) => (
                            <td key={key} className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                {row[key]}
                            </td>
                        ))}
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
};

export default DataTable;
