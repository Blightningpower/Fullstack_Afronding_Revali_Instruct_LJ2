import axios from 'axios';

const api = axios.create({
  baseURL: '/api', // Vite proxy: /api -> http://localhost:5000
  timeout: 10000
});

// Voeg token alleen toe wanneer present en niet leeg (voorkomt "Authorization: Bearer" zonder token)
api.interceptors.request.use(config => {
  try {
    const token = localStorage.getItem('token');
    if (token && token.trim().length > 0) {
      config.headers = config.headers ?? {};
      config.headers['Authorization'] = `Bearer ${token.trim()}`;
    } else {
      if (config.headers && 'Authorization' in config.headers) {
        delete config.headers['Authorization'];
      }
    }
  } catch (e) {
    console.warn('api request interceptor error', e);
  }
  return config;
}, error => Promise.reject(error));

export default api;