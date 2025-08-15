import { Navigate, Route, Routes } from "react-router-dom";
import AuthRoutes from "./auth";
import ReportsRoutes from "./report";
import AuthLayout from "../layouts/AuthLayout";
import MainLayout from "../layouts/MainLayout";
import ProtectedRoute from "../components/ProtectedRoute";
import VehiclesRoutes from "./vehicle";
import type { User } from "../types/auth.types";

interface AppRouterProps {
  user: User | null;
  isAuthenticated: boolean;
  onLoginSuccess: (user: User) => void;
  onLogout: () => void;
}

const AppRouter: React.FC<AppRouterProps> = ({ user, isAuthenticated, onLoginSuccess, onLogout }) => {
  return (
    <Routes>
      <Route element={<AuthLayout />}>
        <Route path="/auth/*" element={<AuthRoutes onLoginSuccess={onLoginSuccess} />} />
      </Route>

      <Route element={
        <ProtectedRoute isAuthenticated={isAuthenticated}>
          <MainLayout user={user} onLogout={onLogout} />
        </ProtectedRoute>
      }>
        <Route path="/reports/*" element={<ReportsRoutes />} />
        <Route path="/vehicles/*" element={<VehiclesRoutes />} />
        {/* Add more main routes here later */}
      </Route>

      <Route path="/" element={<Navigate to={isAuthenticated ? "/reports/bids" : "/auth/login"} />} />
    </Routes>
  );
};

export default AppRouter;