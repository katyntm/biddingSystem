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

  // Remove all filtering logic - just use the original mock vehicles
  let filtered = [...mockVehicles];

  // Remove all sorting logic - keep original order
  
  // Apply pagination only
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

// Helper function to generate mock vehicles based on CSV data
const generateMockVehicles = (count: number) => {
  // Real data from the CSV
  const mockData = [
    { vin: "FBCEE9F621A44499A", make: "Tesla", modelYear: 2023, fuelType: "Electric", modelType: "Model 3", transmission: "Manual", bodyStyle: "Hatchback", color: "Gray", grade: 3.4, price: 53566, location: "BMW of San Francisco" },
    { vin: "15C70FF54A6E43A98", make: "BMW", modelYear: 2020, fuelType: "Electric", modelType: "X5", transmission: "AWD Automatic", bodyStyle: "Coupe", color: "Red", grade: 3.1, price: 26170, location: "Ford of Detroit" },
    { vin: "2371E2DE26954C9AB", make: "Toyota", modelYear: 2024, fuelType: "Gasoline", modelType: "Highlander", transmission: "Automatic", bodyStyle: "Truck", color: "Green", grade: 4.3, price: 55421, location: "Honda of Chicago" },
    { vin: "09028AC21DF348BB9", make: "BMW", modelYear: 2021, fuelType: "Diesel", modelType: "3 Series", transmission: "Manual", bodyStyle: "Truck", color: "Silver", grade: 3.5, price: 34081, location: "Toyota of Seattle" },
    { vin: "51C3E6D380DB4586A", make: "Ford", modelYear: 2020, fuelType: "Gasoline", modelType: "Mustang", transmission: "Manual", bodyStyle: "SUV", color: "Red", grade: 3.2, price: 24182, location: "Chevrolet of Houston" },
    // Add more entries as needed...
  ];

  const makes = ["Tesla", "BMW", "Toyota", "Ford", "Honda", "Chevrolet", "Audi", "Mercedes-Benz", "Volvo", "Hyundai"];
  const fuelTypes = ["Electric", "Gasoline", "Hybrid", "Diesel"];
  const transmissions = ["Manual", "Automatic", "AWD Automatic"];
  const bodyStyles = ["Sedan", "SUV", "Hatchback", "Coupe", "Truck"];
  const colors = ["Black", "White", "Silver", "Gray", "Red", "Blue", "Green", "Onyx Black"];
  const locations = [
    "BMW of San Francisco", "Ford of Detroit", "Honda of Chicago", "Toyota of Seattle",
    "Chevrolet of Houston", "Audi of Miami", "Tesla Store Los Angeles", "Mercedes-Benz of Atlanta",
    "Volvo Cars of Manhattan", "Hyundai of Dallas"
  ];

  const modelsByMake = {
    Tesla: ["Model 3", "Model S", "Model X", "Model Y"],
    BMW: ["3 Series", "5 Series", "X3", "X5"],
    Toyota: ["Camry", "Corolla", "RAV4", "Highlander"],
    Ford: ["F-150", "Mustang", "Explorer", "Escape"],
    Honda: ["Civic", "Accord", "CR-V", "Pilot"],
    Chevrolet: ["Silverado", "Equinox", "Tahoe", "Malibu"],
    Audi: ["A4", "A6", "Q5", "Q7"],
    "Mercedes-Benz": ["C-Class", "E-Class", "GLC", "GLE"],
    Volvo: ["XC60", "XC90", "S60", "V60"],
    Hyundai: ["Elantra", "Santa Fe", "Tucson", "Sonata"]
  };

  return Array.from({ length: count }, (_, i) => {
    // Use CSV data for first few entries, then generate random data
    const csvEntry = mockData[i % mockData.length];
    const useMockData = i < mockData.length;
    
    const make = useMockData ? csvEntry.make : makes[Math.floor(Math.random() * makes.length)];
    const modelType = useMockData ? csvEntry.modelType : modelsByMake[make as keyof typeof modelsByMake][Math.floor(Math.random() * modelsByMake[make as keyof typeof modelsByMake].length)];
    const vin = useMockData ? csvEntry.vin : `VIN${Math.random().toString(36).substring(2, 10).toUpperCase()}`;
    
    const now = new Date();
    const endTime = new Date(now);
    endTime.setHours(endTime.getHours() + Math.floor(Math.random() * 72) + 1);

    // Generate vehicle images based on VIN 
    const imageCount = Math.floor(Math.random() * 6) + 3;
    const vehicleImages = Array.from({ length: imageCount }, (_, j) => ({
      id: `img-${i}-${j}`,
      vehicleId: `veh-${i + 1}`,
      url: `/car_images/${vin}/${vin}_${j + 1}.jpg`,
      createdAt: now.toISOString()
    }));

    return {
      id: `veh-${i + 1}`,
      vin,
      make,
      modelYear: useMockData ? csvEntry.modelYear : Math.floor(Math.random() * 10) + 2015,
      fuelType: useMockData ? csvEntry.fuelType : fuelTypes[Math.floor(Math.random() * fuelTypes.length)],
      modelType,
      transmission: useMockData ? csvEntry.transmission : transmissions[Math.floor(Math.random() * transmissions.length)],
      bodyStyle: useMockData ? csvEntry.bodyStyle : bodyStyles[Math.floor(Math.random() * bodyStyles.length)],
      color: useMockData ? csvEntry.color : colors[Math.floor(Math.random() * colors.length)],
      grade: useMockData ? csvEntry.grade : Math.round((Math.random() * 2 + 3) * 10) / 10, // 3.0 - 5.0
      price: useMockData ? csvEntry.price : Math.floor(Math.random() * 50000) + 20000,
      location: useMockData ? csvEntry.location : locations[Math.floor(Math.random() * locations.length)],
      createdAt: now.toISOString(),
      updatedAt: now.toISOString(),
      vehicleImages,

      // Auction-specific fields
      bidPrice: Math.floor(Math.random() * 5000) + 1000,
      buyItNowPrice: Math.random() > 0.5 ? Math.floor(Math.random() * 10000) + 30000 : undefined,
      saleChannel: ["Online Auction", "Buy It Now", "Live Auction"][Math.floor(Math.random() * 3)],
      startTime: now.toISOString(),
      endTime: endTime.toISOString(),
      numberOfBids: Math.floor(Math.random() * 20),
      isActive: Math.random() > 0.2,
      isSold: Math.random() > 0.8,
    };
  });
};