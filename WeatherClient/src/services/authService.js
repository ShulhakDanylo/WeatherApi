import api from '../utils/api';

export const authService = {
    async register(name, email) {
        // Шлемо лише те, що є у формі
        const response = await api.post('/Auth/register', { name, email });
        return response.data;
    },

    async login(email) {
        // Для логіну шлемо тільки емейл (або нікнейм, залежно як на бекенді)
        const response = await api.post('/Auth/login', { email });
        return response.data;
    }
};