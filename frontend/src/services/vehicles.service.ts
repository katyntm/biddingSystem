import type { VehicleSearchParams, VehicleSearchResponse } from "../types/vehicle.types";
import axiosInstance from "../shared/utils/axios";

const USE_MOCK_API = true;

export const fetchVehicles = async (params: VehicleSearchParams): Promise<VehicleSearchResponse> => {
  if (USE_MOCK_API) {
    return getMockVehicles(params);
  }

  try {
    const response = await axiosInstance.get("/vehicles", { params });
    return response.data;
  } catch (error) {
    console.error("Error fetching vehicles:", error);
    throw error;
  }
};

// Mock function to simulate API response
const getMockVehicles = (params: VehicleSearchParams) => {
  const mockVehicles = generateMockVehicles(50);

  // Apply filtering
  let filtered = [...mockVehicles];
  if (params.keyword) {
    const keyword = params.keyword.toLowerCase();
    filtered = filtered.filter((v) => v.make.toLowerCase().includes(keyword) || v.model.toLowerCase().includes(keyword) || v.vin.toLowerCase().includes(keyword));
  }

  if (params.make) {
    filtered = filtered.filter((v) => v.make === params.make);
  }

  if (params.model) {
    filtered = filtered.filter((v) => v.model === params.model);
  }

  if (params.yearFrom) {
    filtered = filtered.filter((v) => v.year >= params.yearFrom!);
  }

  if (params.yearTo) {
    filtered = filtered.filter((v) => v.year <= params.yearTo!);
  }

  if (params.priceFrom) {
    filtered = filtered.filter((v) => v.currentPrice >= params.priceFrom!);
  }

  if (params.priceTo) {
    filtered = filtered.filter((v) => v.currentPrice <= params.priceTo!);
  }

  // Apply sorting
  if (params.sortBy) {
    const sortField = params.sortBy;
    const sortOrder = params.sortOrder || "asc";

    filtered.sort((a, b) => {
      const aValue = a[sortField as keyof typeof a];
      const bValue = b[sortField as keyof typeof b];

      // Handle undefined values
      if (aValue === undefined && bValue === undefined) return 0;
      if (aValue === undefined) return sortOrder === "asc" ? -1 : 1;
      if (bValue === undefined) return sortOrder === "asc" ? 1 : -1;

      if (aValue < bValue) return sortOrder === "asc" ? -1 : 1;
      if (aValue > bValue) return sortOrder === "asc" ? 1 : -1;
      return 0;
    });
  }

  // Apply pagination
  const totalCount = filtered.length;
  const startIdx = (params.page - 1) * params.pageSize;
  const endIdx = startIdx + params.pageSize;
  const paginatedVehicles = filtered.slice(startIdx, endIdx);

  return {
    items: paginatedVehicles,
    metadata: {
      totalCount,
      pageSize: params.pageSize,
      currentPage: params.page,
      totalPages: Math.ceil(totalCount / params.pageSize),
    },
  };
};
// Helper function to generate mock vehicles
const generateMockVehicles = (count: number) => {
  // Define a type for the makes to use as keys
  type MakeType = "Toyota" | "Honda" | "Ford" | "BMW" | "Mercedes" | "Audi" | "Tesla" | "Chevrolet";

  const makes: MakeType[] = ["Toyota", "Honda", "Ford", "BMW", "Mercedes", "Audi", "Tesla", "Chevrolet"];
  const models: Record<MakeType, string[]> = {
    Toyota: ["Camry", "Corolla", "RAV4", "Highlander"],
    Honda: ["Civic", "Accord", "CR-V", "Pilot"],
    Ford: ["F-150", "Mustang", "Explorer", "Escape"],
    BMW: ["3 Series", "5 Series", "X3", "X5"],
    Mercedes: ["C-Class", "E-Class", "GLC", "S-Class"],
    Audi: ["A4", "A6", "Q5", "Q7"],
    Tesla: ["Model 3", "Model S", "Model X", "Model Y"],
    Chevrolet: ["Silverado", "Equinox", "Tahoe", "Camaro"],
  };

  const transmissions = ["Automatic", "Manual", "CVT", "DCT"];
  const colors = ["Black", "White", "Silver", "Gray", "Red", "Blue"];
  const locations = ["New York, NY", "Los Angeles, CA", "Chicago, IL", "Houston, TX", "Phoenix, AZ"];
  const saleChannels = ["Online Auction", "Buy It Now", "Live Auction"];

  return Array.from({ length: count }, (_, i) => {
    const make = makes[Math.floor(Math.random() * makes.length)] as MakeType;
    const model = models[make][Math.floor(Math.random() * models[make].length)];
    const year = Math.floor(Math.random() * 10) + 2013; // 2013-2023
    const basePrice = Math.floor(Math.random() * 50000) + 10000;
    const now = new Date();
    const endTime = new Date(now);
    endTime.setHours(endTime.getHours() + Math.floor(Math.random() * 72) + 1);

    return {
      id: `veh-${i + 1}`,
      make,
      model,
      year,
      trim: ["SE", "LE", "XLE", "Sport", "Limited"][Math.floor(Math.random() * 5)],
      mileage: Math.floor(Math.random() * 100000),
      vin: `VIN${Math.random().toString(36).substring(2, 10).toUpperCase()}`,
      engineDetails: ["2.0L I4", "2.5L I4", "3.0L V6", "3.5L V6", "5.0L V8"][Math.floor(Math.random() * 5)],
      transmission: transmissions[Math.floor(Math.random() * transmissions.length)],
      exteriorColor: colors[Math.floor(Math.random() * colors.length)],
      location: locations[Math.floor(Math.random() * locations.length)],
      // images: Array.from({ length: Math.floor(Math.random() * 6) + 3 }, (_, j) => `https://source.unsplash.com/random/800x600?car,${make},${j}`),
      images: Array.from({ length: Math.floor(Math.random() * 6) + 3 }, (_, j) => `https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRIOsQgOKrn_Qf4OMVjI0dU75e_hZlgEiUnKg&s`),
      currentPrice: basePrice,
      buyItNowPrice: Math.random() > 0.5 ? basePrice * 1.2 : undefined, // Changed null to undefined
      numberOfBids: Math.floor(Math.random() * 20),
      viewCount: Math.floor(Math.random() * 200) + 50,
      saleChannel: saleChannels[Math.floor(Math.random() * saleChannels.length)],
      startTime: now.toISOString(), // Convert to string
      endTime: endTime.toISOString(), // Convert to string
      isSold: Math.random() > 0.8,
    };
  });
};
