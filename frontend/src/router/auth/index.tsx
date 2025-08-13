import LoginPage from "../../pages/auth/LoginPage";
import { Navigate, Route, Routes } from "react-router-dom";
import type { User } from "../../services/auth.service";

interface AuthRoutesProps {
  onLoginSuccess: (user: User) => void;
}

const AuthRoutes: React.FC<AuthRoutesProps> = ({ onLoginSuccess }) => {
   return (
      <Routes>
         <Route index element={<Navigate to="/auth/login" />} />
         <Route path="login" element={<LoginPage onLoginSuccess={onLoginSuccess} />} />
      </Routes>
   );
};

export default AuthRoutes;