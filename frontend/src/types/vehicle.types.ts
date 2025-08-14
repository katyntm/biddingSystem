export interface Vehicle {
  id: string;
  vin: string;
  make: string;
  model: string;
  year: number;
  trim?: string;
  exteriorColor: string;
  mileage: number;
  engineDetails: string;
  transmission: string;
  fuelType?: string;
  condition?: string;
  images: string[];
  currentPrice: number;
  bidPrice?: number;
  buyItNowPrice?: number;
  saleChannel: string;
  startTime: string;
  endTime: string;
  numberOfBids: number;
  viewCount?: number;
  location?: string;
  isActive?: boolean;
  isSold: boolean;
}

export interface VehicleSearchParams {
  page: number;
  pageSize: number;
  keyword?: string;
  make?: string;
  model?: string;
  yearFrom?: number;
  yearTo?: number;
  priceFrom?: number;
  priceTo?: number;
  vin?: string;
  condition?: string;
  saleChannel?: string;
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