import { useState, useEffect } from 'react';

export type ColumnConfig = {
    key: string;
    header: string;
    transform?: (value: any) => React.ReactNode;
}

export const useTableData = (
    columnsConfig: ColumnConfig[],
    url: string,
    setIsLoading: React.Dispatch<React.SetStateAction<boolean>>
) => {
    const [tableData, setTableData] = useState<{ [key: string]: string | number }[]>([]);

    const loadData = async () => {
        try {
            const response = await fetch(url);
            const data = await response.json();

            interface RawDataItem {
                [key: string]: string | number | undefined;
            }

            const processedData = data.map((item: RawDataItem) => {
                const processedItem: { [key: string]: any } = {};

                columnsConfig.forEach(col => {
                    processedItem[col.key] = col.transform
                        ? col.transform(item[col.key])
                        : item[col.key];
                });

                return processedItem;
            });
            setTableData(processedData);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        loadData();
        const intervalId = setInterval(loadData, 20000);
        return () => clearInterval(intervalId);
    }, []);

    return {
        tableData:tableData || [],
    };
};




