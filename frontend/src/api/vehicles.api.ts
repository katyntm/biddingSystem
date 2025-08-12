// import instance from "../shared/utils/axios";

// export interface Vehicle {
//   id: string;
//   vin: string;
//   make: string;
//   model: string;
//   year: number;
//   mileage: number;
//   condition: string;
//   startingBid: number;
//   buyItNowPrice?: number;
//   description: string;
//   images: string[];
//   status: 'Active' | 'Sold' | 'Pending';
//   auctionEndDate: string;
// }

// export const getVehiclesApi = async (): Promise<Vehicle[]> => {
//   // Check if using mock API
//   const useMock = import.meta.env.VITE_USE_MOCK_API === 'true';
  
//   if (useMock) {
//     // Mock vehicle data using JSONPlaceholder posts as base
//     const response = await instance.get('/posts');
//     const posts = response.data.slice(0, 10); // Get first 10 posts

//     type Post = { id: number; title: string; body: string; userId: number };

//     const mockVehicles: Vehicle[] = posts.map((post: Post, index: number) => ({
//       id: post.id.toString(),
//       vin: `1HGBH41JXMN10${String(post.id).padStart(4, '0')}`,
//       make: ['Honda', 'Ford', 'Toyota', 'Chevrolet', 'BMW'][index % 5],
//       model: ['Civic', 'Escape', 'Camry', 'Tahoe', 'X3'][index % 5],
//       year: 2018 + (index % 5),
//       mileage: 25000 + (index * 1000),
//       condition: ['Excellent', 'Good', 'Fair'][index % 3],
//       startingBid: 15000 + (index * 2000),
//       buyItNowPrice: index % 3 === 0 ? 20000 + (index * 2500) : undefined,
//       description: post.title,
//       images: [`/images/vehicle${index + 1}.jpg`],
//       status: ['Active', 'Active', 'Active', 'Sold'][index % 4] as Vehicle["status"],
//       auctionEndDate: new Date(Date.now() + (index + 1) * 24 * 60 * 60 * 1000).toISOString()
//     }));
    
//     return mockVehicles;
//   }

//   const response = await instance.get("/vehicles");
//   return response.data;
// };

// export const getVehicleByIdApi = async (id: string): Promise<Vehicle> => {
//   const response = await instance.get(`/vehicles/${id}`);
//   return response.data;
// };

import { useQuery } from '@tanstack/react-query';
import { getVehicleByIdApi, getVehiclesApi, type VehicleSearchParams } from '../hooks/useVehicles';

export const useVehicles = (searchParams?: VehicleSearchParams) => {
  return useQuery({
    queryKey: ['vehicles', searchParams],
    queryFn: () => getVehiclesApi(searchParams),
    staleTime: 2 * 60 * 1000, // 2 minutes
    retry: 2,
  });
};

export const useVehicle = (id: string) => {
  return useQuery({
    queryKey: ['vehicle', id],
    queryFn: () => getVehicleByIdApi(id),
    enabled: !!id,
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: 2,
  });
};