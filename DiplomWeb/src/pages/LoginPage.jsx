import { useState } from 'react';
import { useAuthContext } from '../context/AuthContext.jsx';
import { useNavigate } from 'react-router-dom';

const LoginPage = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isLogin, setIsLogin] = useState(true);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { signIn, signUp } = useAuthContext();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      if (isLogin) {
        await signIn(email, password);
      } else {
        await signUp({ email, password });
      }
      navigate('/');
    } catch (err) {
  setError(err.response?.data?.message || err.message || 'Ошибка авторизации');
} finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <h1>{isLogin ? '🚀 Вход' : '📝 Регистрация'}</h1>
        <form onSubmit={handleSubmit}>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Email"
            required
            disabled={loading}
          />
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Пароль"
            required
            disabled={loading}
          />
          {error && <div className="error">{error}</div>}
          <button type="submit" disabled={loading}>
            {loading ? '⏳ Загрузка...' : (isLogin ? 'Войти' : 'Зарегистрироваться')}
          </button>
        </form>
        <p onClick={() => setIsLogin(!isLogin)} className="switch-auth">
          {isLogin ? 'Нет аккаунта? Регистрация' : 'Уже есть аккаунт? Войти'}
        </p>
      </div>
    </div>
  );
};

export default LoginPage;
