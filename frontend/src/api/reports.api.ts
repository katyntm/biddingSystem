import instance from "../shared/utils/axios";

export interface PurchaseReport {
  id: string;
  vehicleId: string;
  vin: string;
  make: string;
  model: string;
  year: number;
  purchasePrice: number;
  purchaseDate: string;
  saleChannel: string;
  status: 'Completed' | 'Pending' | 'Cancelled';
}

export interface BidReport {
  id: string;
  vehicleId: string;
  vin: string;
  make: string;
  model: string;
  year: number;
  bidAmount: number;
  bidDate: string;
  status: 'Active' | 'Won' | 'Lost' | 'Outbid';
  currentHighestBid?: number;
}

export const getPurchaseReportsApi = async (userId?: string): Promise<PurchaseReport[]> => {
  // Check if using mock API
  const useMock = import.meta.env.VITE_USE_MOCK_API === 'true';
  
  if (useMock) {
    // Mock data using JSONPlaceholder structure
    const mockData: PurchaseReport[] = [
      {
        id: '1',
        vehicleId: 'v1',
        vin: '1HGBH41JXMN109186',
        make: 'Honda',
        model: 'Civic',
        year: 2020,
        purchasePrice: 18500,
        purchaseDate: '2024-01-15T10:30:00Z',
        saleChannel: 'Online Auction',
        status: 'Completed'
      },
      {
        id: '2',
        vehicleId: 'v2',
        vin: '2FMPK4J94KBA12345',
        make: 'Ford',
        model: 'Escape',
        year: 2019,
        purchasePrice: 22000,
        purchaseDate: '2024-01-20T14:15:00Z',
        saleChannel: 'Buy It Now',
        status: 'Completed'
      },
      {
        id: '3',
        vehicleId: 'v3',
        vin: '3GNDA13D76S123456',
        make: 'Chevrolet',
        model: 'Tahoe',
        year: 2021,
        purchasePrice: 35000,
        purchaseDate: '2024-02-01T09:45:00Z',
        saleChannel: 'Live Auction',
        status: 'Pending'
      }
    ];
    
  
    return mockData;
  }

  const response = await instance.get(`/reports/purchases${userId ? `?userId=${userId}` : ''}`);
  return response.data;
};

export const getBidReportsApi = async (userId?: string): Promise<BidReport[]> => {
  // Check if using mock API
  const useMock = import.meta.env.VITE_USE_MOCK_API === 'true';
  
  if (useMock) {
    // Mock bid data
    const mockData: BidReport[] = [
      {
        id: '1',
        vehicleId: 'v1',
        vin: '1HGBH41JXMN109186',
        make: 'Honda',
        model: 'Civic',
        year: 2020,
        bidAmount: 17500,
        bidDate: '2024-01-14T15:30:00Z',
        status: 'Won',
        currentHighestBid: 17500
      },
      {
        id: '2',
        vehicleId: 'v4',
        vin: '4T1BF1FK5CU123456',
        make: 'Toyota',
        model: 'Camry',
        year: 2022,
        bidAmount: 24000,
        bidDate: '2024-02-05T09:15:00Z',
        status: 'Outbid',
        currentHighestBid: 24500
      }
    ];
    
    return mockData;
  }

  const response = await instance.get(`/reports/bids${userId ? `?userId=${userId}` : ''}`);
  return response.data;
};