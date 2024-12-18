import React from 'react';
import './App.css';
import DataTable from './components/DataTable';
import useTableData from './hooks/useTableData';


function App() {
    const { tableData, isLoading} = useTableData();

    if (isLoading) {
        return <div>Загрузка...</div>;
    }

    return (
        <div>
            <DataTable tableData={tableData} />
        </div>
    );
}

export default App;
