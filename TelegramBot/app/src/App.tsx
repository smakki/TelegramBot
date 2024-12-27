import React, {useRef, useState} from 'react';
import './App.css';
import DataTable from './components/DataTable';
import Editor from './components/Editor'
import { Editor as TinyMCEEditor } from 'tinymce'
import {useTableData, ColumnConfig} from './hooks/useTableData';


function App() {

    const [isLoading, setIsLoading] = useState (false);
    const StatsColumns: ColumnConfig[] = [
        {
            key: 'UserId',
            header: 'User ID'
        },
        {
            key: 'TotalTasks',
            header: 'Total tasks'
        },
        {
            key: 'CompletedTasks',
            header: 'Completed'
        },
        {
            key: 'LastCompletedTaskDate',
            header: 'Дата последней выполненой задачи'
        }
    ];
    const UsersColumns: ColumnConfig[] = [
        {
            key: 'Id',
            header: 'ID'
        },
        {
            key: 'Username',
            header: 'Username'
        },
        {
            key: 'FirstName',
            header: 'FirstName'
        },
        {
            key: 'LastName',
            header: 'LastName'
        },
        {
            key: 'TimeZone',
            header: 'TimeZone'
        },
        {
            key: 'ReminderToTaskMinutes',
            header: 'Reminder To Task Minutes'
        },
        {
            key: 'CreatedAt',
            header: 'Created at'
        }
    ];
    const {tableData: stats} = useTableData (StatsColumns, '/stats', setIsLoading);
    const {tableData: users} = useTableData (UsersColumns, '/users', setIsLoading);

        const editorRef = useRef<TinyMCEEditor | null>(null);
        const [response, setResponse] = useState (null);
        const handleClick = async () => {
            if (editorRef.current) {
                const content = editorRef.current.getContent ();
                try {
                    const result = await fetch ('/messages', {
                        method: 'POST',
                        headers: {'Content-Type': 'application/json',},
                        body: JSON.stringify ({message:content}),
                    });
                    const data = await result.json ();
                    setResponse (data);
                } catch (error) {
                    console.error ('Ошибка при отправке POST запроса:', error);
                }
            }
        };

    if (isLoading) {
        return <div>Загрузка...</div>;
    }

    return (
        <div className="container mx-auto p-4">
            <DataTable caption="Users" tableData={users}/>
            <DataTable caption="Statistics" tableData={stats}/>
            <Editor apiKey="z3w0pqgoloczm1jip7kpeihjszftavshn357de6radumwr9j"
                           initialValue="<p>Initial content</p>"
                    onInit={(evt, editor) => editorRef.current = editor}
                    init={{
                height: 500,
                menubar: false,
                plugins: ['link', 'table', 'image', 'code', 'emoticons'],
                toolbar: 'undo redo | bold italic underline forecolor | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | removeformat | emoticons'
            }}/>
            <button onClick={handleClick}>Отправить POST запрос</button> {response && <p>Ответ сервера: {JSON.stringify(response)}</p>}
        </div>
    );
}

export default App;
