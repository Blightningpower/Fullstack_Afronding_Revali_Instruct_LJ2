import api from './api';
import { reactive } from 'vue';

// De 'source of truth' voor de status van de gebruiker
export const authState = reactive({
  username: localStorage.getItem('username') || '',
  isLogged: !!localStorage.getItem('token')
});

export async function login(username, password) {
  const res = await api.post('/auth/login', { username, password });
  const rawToken = res?.data?.token;
  const token = rawToken?.trim();

  if (!token) throw new Error('Geen token ontvangen van server');

  // 1. Sla op in localStorage voor persistentie bij refresh
  localStorage.setItem('token', token);
  localStorage.setItem('username', username);

  // 2. Update de reactieve state voor directe UI-update zonder refresh
  authState.username = username;
  authState.isLogged = true;

  return token;
}

export function logout() {
  try {
    // 1. Verwijder uit localStorage
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('serverInstance');

    // 2. Reset de reactieve state
    authState.username = '';
    authState.isLogged = false;
  } catch (e) {
    console.error("Logout error", e);
  }
}