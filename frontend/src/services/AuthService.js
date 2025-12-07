import api from './api';

export async function login(username, password) {
  const res = await api.post('/auth/login', { username, password });
  const rawToken = res?.data?.token;
  const token = rawToken?.trim();

  if (!token) throw new Error('Geen token ontvangen van server');

  // token + username in localStorage
  localStorage.setItem('token', token);
  localStorage.setItem('username', username);

  return token;
}

export function logout() {
  try {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('serverInstance');
  } catch (e) {
    // ignore
  }
}

export const isAuthenticated = () => {
  const token = localStorage.getItem('token');
  return !!(token && token.trim().length > 0);
};

export const getCurrentUserName = () => {
  return localStorage.getItem('username') ?? '';
};