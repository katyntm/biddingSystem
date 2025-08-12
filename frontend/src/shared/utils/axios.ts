import axios, {
   AxiosError,
   type AxiosRequestConfig,
   type AxiosRequestHeaders,
   HttpStatusCode,
   type InternalAxiosRequestConfig,
} from "axios";

// Can switch between JSONPlaceholder and your real API
const API_BASE_URL = import.meta.env.VITE_USE_MOCK_API === 'true' 
  ? 'https://jsonplaceholder.typicode.com'
  : import.meta.env.VITE_API_BASE_URL || 'http://localhost:5291/api';

const instance = axios.create({
   baseURL: API_BASE_URL,
});


//---------------------------- Auth token helpers --------------------------

const getAccessToken = () => {
  return localStorage.getItem('accessToken');
};

const removeTokens = () => {
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken'); // Keep for cleanup but won't be used
  localStorage.removeItem('role');
  localStorage.removeItem('userName');
  localStorage.removeItem('email');
};

const requestAuthInterceptor = (req: AxiosRequestConfig): InternalAxiosRequestConfig => {
   const token = getAccessToken();

   if (token) {
      return {
         ...req,
         headers: {
            ...req.headers,
            Authorization: `Bearer ${token}`,
         } as AxiosRequestHeaders,
      };
   }

   return req as InternalAxiosRequestConfig;
};

const responseAuthErrorInterceptor = async (error: AxiosError) => {
   const { response } = error;
   const status = response?.status;

   // If unauthorized, simply redirect to login without refresh attempt
   if (status === HttpStatusCode.Unauthorized) {
      removeTokens();
      window.location.href = "/auth/login";
      return Promise.reject(error);
   }

   return Promise.reject(error);
};

instance.interceptors.request.use(requestAuthInterceptor);
instance.interceptors.response.use((response) => response, responseAuthErrorInterceptor);

export default instance;