import { login, register } from '../api/auth.js';

class AuthService {
  async signIn(email, password) {
    return await login(email, password);
  }

  async signUp(userData) {
    return await register(userData);
  }

  logout() {
    localStorage.removeItem('token');
  }

  isAuthenticated() {
    return !!localStorage.getItem('token');
  }
}

export default new AuthService();
