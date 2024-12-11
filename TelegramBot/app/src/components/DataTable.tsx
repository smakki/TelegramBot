import React, { ReactElement } from 'react';
import { Table, Column, ICell } from "bsr-table";

interface DataTableProps {
    tableData: (string | number | ReactElement | ICell)[][];
}

const DataTable: React.FC<DataTableProps> = ({ tableData }) => {
    return (
        <div id="table-12" style={{ paddingTop: 10 }}>
            <Table id="table_123" caption="Quick stat table:" rowItems={tableData} style={{ width: "700px" }}>
                <Column style={{ width: "100px" }}>UserId</Column>
                <Column>UserName</Column>
                <Column>TotalTasks</Column>
                <Column>CompletedTasks</Column>
                <Column>LastCompletedTaskDate</Column>
            </Table>
        </div>
    );
};

export default DataTable;
