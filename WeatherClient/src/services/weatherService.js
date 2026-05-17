import axios from 'axios'; // ОБОВ'ЯЗКОВО
const API_URL = 'http://localhost:5128/api/WeatherForecast';

export const weatherService = {
    // 1. Поточна погода
    async getCurrentWeather(cityId) {
        // cityId = "Grid_46,4_24,0" -> стане "Grid_46%2C4_24%2C0"
        const encodedId = encodeURIComponent(cityId);
        const response = await axios.get(`${API_URL}/current/${encodedId}`);
        return response.data; // Оскільки в контролері Ok(result.Value), дані вже в корені
    },

    async getDailyForecast(cityId) {
        const encodedId = encodeURIComponent(cityId);
        const response = await axios.get(`${API_URL}/daily/${encodedId}`);
        return response.data;
    }
};