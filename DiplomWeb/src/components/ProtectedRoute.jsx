import { Navigate } from 'react-router-dom';
import { useAuthContext } from '../context/AuthContext.jsx';

const ProtectedRoute = ({ children }) => {
  const { user, loading } = useAuthContext();
  
  if (loading) return <div>Загрузка...</div>;
  return user ? children : <Navigate to="/login" />;
};

export default ProtectedRoute;
