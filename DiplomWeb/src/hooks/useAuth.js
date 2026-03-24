import { useState, useEffect } from 'react';
import authService from '../services/authService.js';

export const useAuth = () => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
  const initAuth = async () => {
    if (authService.isAuthenticated()) {
      try {
        // TODO: fetch('/api/auth/me') для реальных данных
        setUser({ id: 1, role: 'teacher' });
      } catch {
        authService.logout();
      }
    }
    setLoading(false);
  };
  initAuth();
}, []);

  const signIn = async (email, password) => {
    await authService.signIn(email, password);
    setUser({ id: 1, role: 'teacher' });
  };

  const signUp = async (userData) => {
    await authService.signUp(userData);
    setUser({ id: 1, role: 'teacher' });
  };

  const logout = () => {
    authService.logout();
    setUser(null);
  };

  return { user, loading, signIn, signUp, logout };
};
