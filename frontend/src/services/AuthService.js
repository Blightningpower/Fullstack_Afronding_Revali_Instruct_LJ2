import api from './api';
import { reactive } from 'vue';

// De 'source of truth' inclusief de rol voor US10
export const authState = reactive({
  username: localStorage.getItem('username') || '',
  role: localStorage.getItem('role') || '',
  isLogged: !!localStorage.getItem('token')
});

export async function login(username, password) {
  const res = await api.post('/auth/login', { username, password });
  
  // 1. Haal alle data uit de response
  const token = res?.data?.token?.trim();
  const role = res?.data?.role; // Zorg dat je backend dit stuurt!
  const serverUsername = res?.data?.username || username;

  if (!token) throw new Error('Geen token ontvangen van server');

  // 2. Update de reactieve state EERST (voor de router)
  authState.username = serverUsername;
  authState.role = role; // Dit lost de 'undefined' op
  authState.isLogged = true;

  // 3. Sla op in localStorage voor later
  localStorage.setItem('token', token);
  localStorage.setItem('username', serverUsername);
  localStorage.setItem('role', role);

  return { token, role };
}

export function logout() {
  try {
    // 1. Verwijder alles uit localStorage
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('role'); // Verwijder de rol
    localStorage.removeItem('serverInstance');

    // 2. Reset de reactieve state
    authState.username = '';
    authState.role = '';
    authState.isLogged = false;
  } catch (e) {
    console.error("Logout error", e);
  }
}