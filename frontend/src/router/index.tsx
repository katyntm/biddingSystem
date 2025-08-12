import { Navigate, Route, Routes } from "react-router-dom";
import AuthRoutes from "./auth";
import ReportsRoutes from "./report";
import AuthLayout from "../layouts/AuthLayout";
import MainLayout from "../layouts/MainLayout";
import VehiclesPage from "../pages/vehicles/VehiclesPage";

const AppRouter = () => {
  return (
    <Routes>
      <Route element={<AuthLayout />}>
        <Route path="/auth/*" element={<AuthRoutes />} />
      </Route>

      <Route element={<MainLayout />}>
        <Route path="/reports/*" element={<ReportsRoutes />} />
        {/* Add more main routes here later */}
        <Route path="/vehicles" element={<VehiclesPage />} />
        {/* <Route path="/auctions" element={<AuctionsPage />} /> */}
      </Route>

      {/* <Route path="/" element={<Navigate to="/reports/purchases" />} /> */}
      <Route path="/" element={<Navigate to="/auth/login" />} />
    </Routes>
  );
};

export default AppRouter;
