import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { removeTokens, setAccessToken, setEmail, setUserName, setUserId, setBalance } from '../shared/utils/auth';
import { toast } from 'react-toastify';
import { AxiosError } from 'axios';
import { loginApi, logoutApi, type ApiErrorResponse, type LoginResponse } from '../services/auth.service';

// Hook for login with React Query
export const useLogin = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: loginApi,
    onSuccess: (data: LoginResponse) => {
      if (data.success) {
        // Store tokens and user info
        setAccessToken(data.data.token);
        setUserName(data.data.username);
        setEmail(data.data.email);
        setUserId(data.data.userId);
        setBalance(data.data.balance);

        // Clear any existing queries
        queryClient.clear();

        toast.success('Login successful!');
        navigate('/reports/bids');
      } else {
        throw new Error(data.message || 'Login failed');
      }
    },
    onError: (error: AxiosError<ApiErrorResponse> | Error) => {
      console.error('Login failed:', error);
      
      let errorMessage = 'Login failed. Please check your credentials.';
      
      if (error instanceof AxiosError) {
        errorMessage = error.response?.data?.message || 
                     error.message || 
                     'Login failed. Please check your credentials.';
      } else if (error instanceof Error) {
        errorMessage = error.message;
      }
      
      toast.error(errorMessage);
    },
  });
};

// Hook for logout
export const useLogout = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: logoutApi,
    onSuccess: () => {
      removeTokens();
      queryClient.clear();
      navigate('/auth/login');
      toast.info('You have been logged out.');
    },
    onError: (error) => {
      console.error('Logout failed:', error);
      // Even if logout API fails, clear local storage
      removeTokens();
      queryClient.clear();
      navigate('/auth/login');
      toast.info('You have been logged out.');
    },
  });
};

// Simple logout function without API call (for immediate logout)
export const useSimpleLogout = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  return () => {
    removeTokens();
    queryClient.clear();
    navigate('/auth/login');
    toast.info('You have been logged out.');
  };
};

// Re-export the API functions for direct use where hooks can't be used
export { loginApi, logoutApi } from '../services/auth.service';