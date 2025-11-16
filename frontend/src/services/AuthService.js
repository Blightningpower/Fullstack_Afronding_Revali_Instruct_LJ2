import api from "./api";

export async function login(username, password) {
  const res = await api.post("/auth/login", { username, password });
  const token = res?.data?.token;
  if (!token) throw new Error("Geen token ontvangen van server");
  localStorage.setItem("token", token);
  return token;
}

export function logout() {
  localStorage.removeItem("token");
}

export const isAuthenticated = () => !!localStorage.getItem("token");
