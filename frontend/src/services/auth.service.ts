import instance from "../shared/utils/axios";
import type { LoginRequest, LoginResponse } from "../types/auth.types";

export const loginApi = async (credentials: LoginRequest): Promise<LoginResponse> => {
  const response = await instance.post("/auth/login", credentials);
  return response.data;
};

export const logoutApi = async (): Promise<void> => {
  await instance.post("/auth/logout");
};

// Re-export types for convenience
export type { LoginRequest, LoginResponse, User, ApiErrorResponse } from "../types/auth.types";