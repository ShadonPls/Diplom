import { api } from "./api.js";
import { state } from "./state.js";

export async function login(email, password) {
  const result = await api.post("/auth/login", { email, password });
  state.token = result.token;
  state.user = result.user;
  await loadCurrentUser(); 
  return result;
}

export async function register(email, password, role = "Teacher") {
  const result = await api.post("/auth/register", { email, password, role });
  state.token = result.token;
  state.user = result.user;
  await loadCurrentUser(); 
  return result;
}

export async function loadCurrentUser() {
  const user = await api.get("/auth/me");
  state.user = user;
  return user;
}

export function logout() {
  state.token = null;
  state.user = null;
}