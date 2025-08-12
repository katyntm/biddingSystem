import LoginPage from "../../pages/auth/LoginPage";
import { Navigate, Route, Routes } from "react-router-dom";

const AuthRoutes = () => {
   return (
      <Routes>
         <Route index element={<Navigate to="/auth/login" />} />
         <Route path="login" element={<LoginPage />} />
      </Routes>
   );
};

export default AuthRoutes;