import AppRouter from './router'
import 'bootstrap/dist/css/bootstrap.min.css'
import './App.css'
import { useEffect, useState } from 'react';
import { getCurrentUser, isAuthenticated, removeTokens } from './shared/utils/auth';
import { logoutApi, type User } from './services/auth.service';

function App() {
  const [user, setUser] = useState<User | null>(null);
  const [authenticated, setAuthenticated] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Initialize auth state on app start
    const initializeAuth = () => {
      const currentUser = getCurrentUser();
      const isAuth = isAuthenticated();
      
      setUser(currentUser);
      setAuthenticated(isAuth);
      setLoading(false);
    };

    initializeAuth();
  }, []);

  const handleLoginSuccess = (user: User) => {
    setUser(user);
    setAuthenticated(true);
  };

  const handleLogout = async () => {
    try {
      // Call backend logout API
      await logoutApi();
    } catch (error) {
      console.error('Logout API failed:', error);
      // Continue with local logout even if API fails
    } finally {
      // Clear local storage and state
      removeTokens();
      setUser(null);
      setAuthenticated(false);
    }
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <div className="spinner-border" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  return (
    <AppRouter 
      user={user}
      isAuthenticated={authenticated}
      onLoginSuccess={handleLoginSuccess}
      onLogout={handleLogout}
    />
  );
}

export default App;