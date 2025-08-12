import { Routes, Route } from 'react-router-dom';
import PurchaseHistoryPage from '../../pages/reports/PurchaseHistoryPage';
import BidHistoryPage from '../../pages/reports/BidHistoryPage';

const ReportsRoutes = () => {
  return (
    <Routes>
      <Route path="purchases" element={<PurchaseHistoryPage />} />
      <Route path="bids" element={<BidHistoryPage />} />
    </Routes>
  );
};

export default ReportsRoutes;