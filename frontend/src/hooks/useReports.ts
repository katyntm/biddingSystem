import { useQuery } from '@tanstack/react-query';
import { getUserName } from '../shared/utils/auth';
import { getBidReportsApi, getPurchaseReportsApi } from '../services/reports.service';

export const usePurchaseReports = () => {
  const userName = getUserName();
  
  return useQuery({
    queryKey: ['purchaseReports', userName],
    queryFn: () => getPurchaseReportsApi(userName || undefined),
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: 2,
  });
};

export const useBidReports = () => {
  const userName = getUserName();
  
  return useQuery({
    queryKey: ['bidReports', userName],
    queryFn: () => getBidReportsApi(userName || undefined),
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: 2,
  });
};