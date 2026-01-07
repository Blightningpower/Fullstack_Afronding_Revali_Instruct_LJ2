import api from './api';
import { reactive } from 'vue';

export const authState = reactive({
  username: localStorage.getItem('username') || '',
  role: localStorage.getItem('role') || '',
  isLogged: !!localStorage.getItem('token')
});

export async function login(username, password) {
  const res = await api.post('/auth/login', { username, password });
  const token = res?.data?.token?.trim();
  const role = res?.data?.role;
  const serverUsername = res?.data?.username || username;

  if (!token) throw new Error('Geen token ontvangen van server');

  authState.username = serverUsername;
  authState.role = role;
  authState.isLogged = true;

  localStorage.setItem('token', token);
  localStorage.setItem('username', serverUsername);
  localStorage.setItem('role', role);

  return { token, role };
}

export function logout() {
  try {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('role');
    localStorage.removeItem('serverInstance');

    authState.username = '';
    authState.role = '';
    authState.isLogged = false;
  } catch (e) {
    console.error("Logout error", e);
  }
}