import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5128/api', // Твій порт з Visual Studio
});

export default api;