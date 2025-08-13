export interface LoginRequest {
  userName: string; 
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
  message: string;
}

export interface User {
  id: string;
  username: string;
  email: string;
  balance: number;
}

export interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}