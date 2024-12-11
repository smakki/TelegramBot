import { useState, useEffect, ReactElement } from 'react';
import { ICell } from 'bsr-table';

const useTableData = () => {
    const [tableData, setTableData] = useState<(string | number | ReactElement | ICell)[][]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    const loadData = async () => {
        try {
            const data = await getDataTable();
            setTableData(data);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        loadData();
        const intervalId = setInterval(() => {
            loadData();
        }, 20000);
        return () => clearInterval(intervalId);
    }, []);

    return { tableData, isLoading };
};

async function getDataTable(): Promise<(string | number | ReactElement | ICell)[][]> {
    const url = "/stats";

    try {
        const response = await fetch(url);

        if (!response.ok) {
            throw new Error(`Ошибка HTTP: ${response.status}`);
        }

        const data = await response.json();

        const dataArray = Array.isArray(data) ? data : [data];
        return dataArray.map((item) => [
            item.UserId,
            item.UserName,
            item.TotalTasks,
            item.CompletedTasks,
            item.LastCompletedTaskDate
        ]);
    } catch (error) {
        console.error("Ошибка при получении данных:", error);
        return [];
    }
}

export default useTableData;
