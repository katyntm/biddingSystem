import instance from "../shared/utils/axios";

export interface LoginRequest {
  userName: string;  // Changed to match backend DTO
  password: string;
}

export interface LoginResponse {
  success: boolean;
  data: {
    token: string;
    username: string;
    email: string;
    userId: string;
    balance: number;
  };
  message?: string;
}

export interface User {
  id: string;
  username: string;
  email: string;
  balance: number;
}

export const loginApi = async (credentials: LoginRequest): Promise<LoginResponse> => {
  const response = await instance.post("/auth/login", credentials);
  return response.data;
};

export const logoutApi = async (): Promise<void> => {
  await instance.post("/auth/logout");
};