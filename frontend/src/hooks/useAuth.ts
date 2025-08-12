import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { removeTokens, setAccessToken, setEmail, setUserName } from '../shared/utils/auth';
import { toast } from 'react-toastify';

export const useLogin = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: loginApi,
    onSuccess: (data) => {
      // Store tokens and user info
      setAccessToken(data.accessToken);
      setUserName(data.user.username);
      setEmail(data.user.email);

      // Clear any existing queries
      queryClient.clear();

      toast.success('Login successful!');
      navigate('/reports/purchases');
    },
    onError: (error: { response?: { data?: { message?: string } } }) => {
      console.error('Login failed:', error);
      const errorMessage =
        error?.response?.data?.message || 'Login failed. Please check your credentials.';
      toast.error(errorMessage);
    },
  });
};

export const useLogout = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  return () => {
    removeTokens();
    queryClient.clear();
    navigate('/auth/login');
    toast.info('You have been logged out.');
  };
};

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
