import axios from "axios";

const api = axios.create({
  baseURL: "/api", // Vite proxy: /api -> http://localhost:5173
  timeout: 10000,
});

// Request interceptor: voeg token toe indien aanwezig
api.interceptors.request.use(
  (config) => {
    try {
      const token = localStorage.getItem("token");
      if (token && token.trim().length > 0) {
        config.headers = config.headers ?? {};
        config.headers["Authorization"] = `Bearer ${token.trim()}`;
      } else {
        if (config.headers && "Authorization" in config.headers) {
          delete config.headers["Authorization"];
        }
      }
    } catch (e) {
      console.warn("api request interceptor error", e);
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Helper: controleer server instance header en clear token bij mismatch
function handleServerInstanceHeader(headers) {
  try {
    const headerValue =
      headers?.["x-server-instance"] ?? headers?.["X-Server-Instance"] ?? null;
    if (!headerValue) return;

    const stored = localStorage.getItem("serverInstance");
    if (stored && stored !== headerValue) {
      // server instance changed -> clear token and force login
      console.warn(
        "Detected backend restart (server instance changed). Clearing token and reloading."
      );
      try {
        localStorage.removeItem("token");
      } catch {}
      // update stored instance to newest so we don't loop forever
      localStorage.setItem("serverInstance", headerValue);
      // redirect to login to force re-auth
      window.location.href = "/login";
      return;
    }

    // store instance if first time
    if (!stored) {
      localStorage.setItem("serverInstance", headerValue);
    }
  } catch (e) {
    console.warn("Failed to handle server instance header", e);
  }
}

// Response interceptor: controleer header en behandel 401
api.interceptors.response.use(
  (response) => {
    try {
      handleServerInstanceHeader(response.headers);
    } catch (e) {
      console.warn("response handler error", e);
    }
    return response;
  },
  (error) => {
    try {
      // als response aanwezig is: controleer header (kan helpen bij 401 na restart)
      if (error?.response) {
        handleServerInstanceHeader(error.response.headers);

        if (error.response.status === 401) {
          console.warn(
            "API returned 401 — clearing token and redirecting to /login"
          );
          try {
            localStorage.removeItem("token");
          } catch {}
          // redirect to login
          window.location.href = "/login";
        }
      }
    } catch (e) {
      console.error("Error handling API error", e);
    }
    return Promise.reject(error);
  }
);

export default api;