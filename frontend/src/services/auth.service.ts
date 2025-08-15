import instance from "../shared/utils/axios";
import type { LoginRequest, LoginResponse } from "../types/auth.types";

export const loginApi = async (credentials: LoginRequest): Promise<LoginResponse> => {
  const response = await instance.post("/auth/login", credentials);
  return response.data;
};
