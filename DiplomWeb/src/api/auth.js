import apiClient from './apiClient.js';

export const login = async (email, password) => {
  const { data } = await apiClient.post('auth/login', { email, password });
  localStorage.setItem('token', data.token);
  return data;
};

export const register = async (dto) => {
  const { data } = await apiClient.post('auth/register', dto);
  localStorage.setItem('token', data.token);
  return data;
};
