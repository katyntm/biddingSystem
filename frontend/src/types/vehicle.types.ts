export interface Vehicle {
  id: string;
  vin: string;
  make: string;
  modelYear: number; 
  fuelType: string;
  modelType: string; 
  transmission: string;
  bodyStyle: string;
  color: string;
  grade: number;
  price: number;
  location: string;
  createdAt: string;
  updatedAt: string;
  vehicleImages: VehicleImage[];

  // Additional frontend-only fields for auction functionality
  bidPrice?: number;
  buyItNowPrice?: number;
  saleChannel?: string;
  startTime?: string;
  endTime?: string;
  numberOfBids?: number;
  isActive?: boolean;
  isSold?: boolean;
}

export interface VehicleImage {
  id: string;
  vehicleId: string;
  url: string;
  createdAt: string;
}

export interface VehicleSearchParams {
  page: number;
  pageSize: number;
  keyword?: string;
  make?: string;
  modelType?: string; 
  yearFrom?: number;
  yearTo?: number;
  priceFrom?: number;
  priceTo?: number;
  vin?: string;
  fuelType?: string;
  bodyStyle?: string;
  transmission?: string;
  location?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export interface VehicleSearchResponse {
  items: Vehicle[];
  metadata: {
    totalCount: number;
    currentPage: number;
    totalPages: number;
    pageSize: number;
  };
}

export interface BidRequest {
  vehicleId: string;
  bidAmount: number;
}