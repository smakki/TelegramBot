import React from 'react';
import Table from '../components/Table';

interface User {
    Id: number;
    Username: string | null;
    FirstName: string | null;
    LastName: string | null;
    TimeZone: string;
    ReminderToTaskMinutes: number;
    CreatedAt: string;
    NotSendBroadcastMessages: boolean;
}

interface UserTaskStatisticsDto {
    UserId: number;
    UserName: string | null;
    Name: string | null;
    TotalTasks: number;
    CompletedTasks: number;
    LastCompletedTaskDate: string | null;
}

const Dashboard: React.FC = () => {
    const renderUserHeader = () => (
        <>
            <th>ID</th>
            <th>Username</th>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Time Zone</th>
            <th>Reminder Minutes</th>
            <th>Created At</th>
            <th>Not Send Broadcast Messages</th>
        </>
    );

    const renderUserItem = (item: User) => (
        <>
            <td>{item.Id}</td>
            <td>{item.Username}</td>
            <td>{item.FirstName}</td>
            <td>{item.LastName}</td>
            <td>{item.TimeZone}</td>
            <td>{item.ReminderToTaskMinutes}</td>
            <td>{item.CreatedAt}</td>
            <td>{item.NotSendBroadcastMessages ? 'Yes' : 'No'}</td>
        </>
    );

    const renderUserTaskStatisticsHeader = () => (
        <>
            <th>User ID</th>
            <th>User Name</th>
            <th>Name</th>
            <th>Total Tasks</th>
            <th>Completed Tasks</th>
            <th>Last Completed Task Date</th>
        </>
    );

    const renderUserTaskStatisticsItem = (item: UserTaskStatisticsDto) => (
        <>
            <td>{item.UserId}</td>
            <td>{item.UserName}</td>
            <td>{item.Name}</td>
            <td>{item.TotalTasks}</td>
            <td>{item.CompletedTasks}</td>
            <td>{item.LastCompletedTaskDate}</td>
        </>
    );

    return (
        <div>
            <h2>Dashboard</h2>
            <h3>Users</h3>
            <Table<User>
                url="/api/users"
                renderItem={renderUserItem}
                renderHeader={renderUserHeader}
            />
            <h3>User Task Statistics</h3>
            <Table<UserTaskStatisticsDto>
                url="/api/stats"
                renderItem={renderUserTaskStatisticsItem}
                renderHeader={renderUserTaskStatisticsHeader}
            />
        </div>
    );
};

export default Dashboard;
