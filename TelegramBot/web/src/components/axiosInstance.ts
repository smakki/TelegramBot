import axios from 'axios';

const axiosInstance = axios.create({
    baseURL: 'http://localhost:5000/', // Используйте переменные окружения Vite
    timeout: 10000, // Установите таймаут для запросов
    headers: {
        'Content-Type': 'application/json',
    },
});

export default axiosInstance;
