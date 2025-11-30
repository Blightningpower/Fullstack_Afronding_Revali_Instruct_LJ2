import api from "./api";

export async function login(username, password) {
  const res = await api.post("/auth/login", { username, password });
  const rawToken = res?.data?.token;
  const token = rawToken?.trim();

  if (!token) throw new Error("Geen token ontvangen van server");

  localStorage.setItem("token", token);
  return token;
}

export function logout() {
  try {
    localStorage.removeItem("token");
    localStorage.removeItem("serverInstance");
  } catch {}
}

export const isAuthenticated = () => {
  const token = localStorage.getItem("token");
  return !!(token && token.trim().length > 0);
};
