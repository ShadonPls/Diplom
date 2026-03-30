// Твой api.js с исправлениями для бекенда
import { state } from "./state.js";

async function request(path, options = {}) {
  const headers = {
    "Content-Type": "application/json",
    ...(options.headers || {})
  };

  if (state.token) {
    headers["Authorization"] = `Bearer ${state.token}`;
  }

  const response = await fetch(`${state.apiBaseUrl}${path}`, {
    ...options,
    headers
  });

  if (!response.ok) {
    let errorBody = null;
    try {
      errorBody = await response.json();
    } catch {
      errorBody = { message: "Ошибка запроса" };
    }
    throw {
      status: response.status,
      body: errorBody
    };
  }

  const contentType = response.headers.get("content-type") || "";
  
  // PDF возвращает blob
  if (path.includes('/pdf')) {
    return await response.blob();
  }

  if (contentType.includes("application/json")) {
    return await response.json();
  }

  return response;
}

export const api = {
  get: (path) => request(path, { method: "GET" }),
  post: (path, body) => request(path, { method: "POST", body: JSON.stringify(body) }),
  put: (path, body) => request(path, { method: "PUT", body: JSON.stringify(body) }),
  delete: (path) => request(path, { method: "DELETE" })
};