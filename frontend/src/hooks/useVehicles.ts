import { keepPreviousData, useQuery } from '@tanstack/react-query';
import { fetchVehicles } from '../services/vehicles.service';
import type { VehicleSearchParams } from '../types/vehicle.types';

export const useVehicles = (params: VehicleSearchParams) => {
  return useQuery({
    queryKey: ['vehicles', params],
    queryFn: () => fetchVehicles(params),
    staleTime: 2 * 60 * 1000, // 2 minutes
    retry: 2,
    // Keep previous data while fetching new data (good for pagination)
    placeholderData: keepPreviousData,
  });
};
