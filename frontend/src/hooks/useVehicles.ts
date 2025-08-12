// import { useQuery } from '@tanstack/react-query';
// import { getVehicleByIdApi, getVehiclesApi } from '../api/vehicles.api';

// export const useVehicles = () => {
//   return useQuery({
//     queryKey: ['vehicles'],
//     queryFn: getVehiclesApi,
//     staleTime: 2 * 60 * 1000, // 2 minutes
//     retry: 2,
//   });
// };

// export const useVehicle = (id: string) => {
//   return useQuery({
//     queryKey: ['vehicle', id],
//     queryFn: () => getVehicleByIdApi(id),
//     enabled: !!id,
//     staleTime: 5 * 60 * 1000, // 5 minutes
//     retry: 2,
//   });
// };

import instance from "../shared/utils/axios";

export interface Vehicle {
  id: string;
  vin: string;
  make: string;
  model: string;
  year: number;
  mileage: number;
  condition: string;
  startingBid: number;
  currentBid: number;
  buyItNowPrice?: number;
  description: string;
  images: string[];
  status: 'Active' | 'Sold' | 'Pending';
  auctionEndDate: string;
  location: string;
  saleChannel: string;
  numberOfBids: number;
  timeRemaining: string;
}

export interface VehicleSearchParams {
  search?: string;
  make?: string[];
  model?: string[];
  yearFrom?: number;
  yearTo?: number;
  priceFrom?: number;
  priceTo?: number;
  condition?: string[];
  status?: string[];
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

export interface PaginatedVehicleResponse {
  items: Vehicle[];
  totalCount: number;
  totalPages: number;
  currentPage: number;
  pageSize: number;
}

export const getVehiclesApi = async (searchParams?: VehicleSearchParams): Promise<PaginatedVehicleResponse> => {
  // Check if using mock API
  const useMock = import.meta.env.VITE_USE_MOCK_API === 'true';
  
  if (useMock) {
    // Mock vehicle data using JSONPlaceholder posts as base
    const response = await instance.get('/posts');
    const posts = response.data.slice(0, 30); // Get more posts for pagination

    type Post = { id: number; title: string; body: string; userId: number };

    let mockVehicles: Vehicle[] = posts.map((post: Post, index: number) => ({
      id: post.id.toString(),
      vin: `1HGBH41JXMN10${String(post.id).padStart(4, '0')}`,
      make: ['Honda', 'Ford', 'Toyota', 'Chevrolet', 'BMW', 'Audi', 'Mercedes', 'Lexus'][index % 8],
      model: ['Civic', 'Escape', 'Camry', 'Tahoe', 'X3', 'A4', 'C-Class', 'RX'][index % 8],
      year: 2018 + (index % 6),
      mileage: 15000 + (index * 1000),
      condition: ['Excellent', 'Good', 'Fair', 'Poor'][index % 4],
      startingBid: 15000 + (index * 1500),
      currentBid: 15000 + (index * 1500) + Math.floor(Math.random() * 5000),
      buyItNowPrice: index % 3 === 0 ? 20000 + (index * 2500) : undefined,
      description: post.title,
      images: [
        `/images/vehicle${(index % 10) + 1}.jpg`,
        `/images/vehicle${(index % 10) + 1}_2.jpg`,
        `/images/vehicle${(index % 10) + 1}_3.jpg`
      ],
      status: ['Active', 'Active', 'Active', 'Sold'][index % 4] as Vehicle["status"],
      auctionEndDate: new Date(Date.now() + (index + 1) * 24 * 60 * 60 * 1000).toISOString(),
      location: ['New York, NY', 'Los Angeles, CA', 'Chicago, IL', 'Houston, TX', 'Phoenix, AZ'][index % 5],
      saleChannel: ['Online Auction', 'Live Auction', 'Buy It Now'][index % 3],
      numberOfBids: Math.floor(Math.random() * 25) + 1,
      timeRemaining: `${Math.floor(Math.random() * 7) + 1} days`
    }));

    // Apply search filter
    if (searchParams?.search) {
      const searchTerm = searchParams.search.toLowerCase();
      mockVehicles = mockVehicles.filter(vehicle => 
        vehicle.make.toLowerCase().includes(searchTerm) ||
        vehicle.model.toLowerCase().includes(searchTerm) ||
        vehicle.vin.toLowerCase().includes(searchTerm) ||
        vehicle.description.toLowerCase().includes(searchTerm)
      );
    }

    // Apply make filter
    if (searchParams?.make && searchParams.make.length > 0) {
      mockVehicles = mockVehicles.filter(vehicle => 
        searchParams.make!.includes(vehicle.make)
      );
    }

    // Apply model filter
    if (searchParams?.model && searchParams.model.length > 0) {
      mockVehicles = mockVehicles.filter(vehicle => 
        searchParams.model!.includes(vehicle.model)
      );
    }

    // Apply year filter
    if (searchParams?.yearFrom) {
      mockVehicles = mockVehicles.filter(vehicle => vehicle.year >= searchParams.yearFrom!);
    }
    if (searchParams?.yearTo) {
      mockVehicles = mockVehicles.filter(vehicle => vehicle.year <= searchParams.yearTo!);
    }

    // Apply price filter
    if (searchParams?.priceFrom) {
      mockVehicles = mockVehicles.filter(vehicle => vehicle.currentBid >= searchParams.priceFrom!);
    }
    if (searchParams?.priceTo) {
      mockVehicles = mockVehicles.filter(vehicle => vehicle.currentBid <= searchParams.priceTo!);
    }

    // Apply condition filter
    if (searchParams?.condition && searchParams.condition.length > 0) {
      mockVehicles = mockVehicles.filter(vehicle => 
        searchParams.condition!.includes(vehicle.condition)
      );
    }

    // Apply status filter
    if (searchParams?.status && searchParams.status.length > 0) {
      mockVehicles = mockVehicles.filter(vehicle => 
        searchParams.status!.includes(vehicle.status)
      );
    }

    // Apply sorting
    if (searchParams?.sortBy) {
      mockVehicles.sort((a, b) => {
        let aValue: any = a[searchParams.sortBy as keyof Vehicle];
        let bValue: any = b[searchParams.sortBy as keyof Vehicle];

        if (typeof aValue === 'string') {
          aValue = aValue.toLowerCase();
          bValue = bValue.toLowerCase();
        }

        if (searchParams.sortOrder === 'desc') {
          return aValue > bValue ? -1 : aValue < bValue ? 1 : 0;
        } else {
          return aValue < bValue ? -1 : aValue > bValue ? 1 : 0;
        }
      });
    }

    // Apply pagination
    const page = searchParams?.page || 1;
    const pageSize = searchParams?.pageSize || 12;
    const startIndex = (page - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const paginatedVehicles = mockVehicles.slice(startIndex, endIndex);

    // Simulate network delay
    await new Promise(resolve => setTimeout(resolve, 800));

    return {
      items: paginatedVehicles,
      totalCount: mockVehicles.length,
      totalPages: Math.ceil(mockVehicles.length / pageSize),
      currentPage: page,
      pageSize: pageSize
    };
  }

  // Real API call would include query parameters
  const params = new URLSearchParams();
  if (searchParams?.search) params.append('search', searchParams.search);
  if (searchParams?.make) searchParams.make.forEach(m => params.append('make', m));
  if (searchParams?.model) searchParams.model.forEach(m => params.append('model', m));
  if (searchParams?.yearFrom) params.append('yearFrom', searchParams.yearFrom.toString());
  if (searchParams?.yearTo) params.append('yearTo', searchParams.yearTo.toString());
  if (searchParams?.priceFrom) params.append('priceFrom', searchParams.priceFrom.toString());
  if (searchParams?.priceTo) params.append('priceTo', searchParams.priceTo.toString());
  if (searchParams?.condition) searchParams.condition.forEach(c => params.append('condition', c));
  if (searchParams?.status) searchParams.status.forEach(s => params.append('status', s));
  if (searchParams?.sortBy) params.append('sortBy', searchParams.sortBy);
  if (searchParams?.sortOrder) params.append('sortOrder', searchParams.sortOrder);
  if (searchParams?.page) params.append('page', searchParams.page.toString());
  if (searchParams?.pageSize) params.append('pageSize', searchParams.pageSize.toString());

  const response = await instance.get(`/vehicles?${params.toString()}`);
  return response.data;
};

export const getVehicleByIdApi = async (id: string): Promise<Vehicle> => {
  const response = await instance.get(`/vehicles/${id}`);
  return response.data;
};