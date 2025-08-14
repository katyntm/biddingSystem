import React from "react";
import { Routes, Route } from "react-router-dom";
import VehiclesPage from "../../pages/vehicles/VehiclesPage";

const VehiclesRoutes: React.FC = () => (
  <Routes>
    <Route path="/" element={<VehiclesPage />} />
  </Routes>
);

export default VehiclesRoutes;
