import instance from "../shared/utils/axios";

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  user: {
    id: string;
    username: string;
    email: string;
    role: string;
  };
}

export const loginApi = async (credentials: LoginRequest): Promise<LoginResponse> => {
  // Check if using mock API
  const useMock = import.meta.env.VITE_USE_MOCK_API === 'true';
  
  if (useMock) {
    // Mock login for JSONPlaceholder
    const response = await instance.get('/users/1');
    return {
      accessToken: 'mock-access-token',
     
      user: {
        id: '1',
        username: credentials.username,
        email: response.data.email,
        role: 'dealer'
      }
    };
  }

  const response = await instance.post("/auth/login", credentials);
  return response.data;
};
