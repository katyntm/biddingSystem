export const AUTH_KEY = {
  ACCESS_TOKEN: "accessToken",
  USER_NAME: "userName",
  EMAIL: "email",
  USER_ID: "userId",
  BALANCE: "balance",
};

export const setAccessToken = (token: string) => {
  localStorage.setItem(AUTH_KEY.ACCESS_TOKEN, token);
};

export const getAccessToken = () => {
  return localStorage.getItem(AUTH_KEY.ACCESS_TOKEN);
};

export const setUserName = (userName: string) => {
  localStorage.setItem(AUTH_KEY.USER_NAME, userName);
};

export const getUserName = () => {
  return localStorage.getItem(AUTH_KEY.USER_NAME);
};

export const setEmail = (email: string) => {
  localStorage.setItem(AUTH_KEY.EMAIL, email);
};

export const getEmail = () => {
  return localStorage.getItem(AUTH_KEY.EMAIL);
};

export const setUserId = (userId: string) => {
  localStorage.setItem(AUTH_KEY.USER_ID, userId);
};

export const getUserId = () => {
  return localStorage.getItem(AUTH_KEY.USER_ID);
};

export const setBalance = (balance: number) => {
  localStorage.setItem(AUTH_KEY.BALANCE, balance.toString());
};

export const getBalance = () => {
  const balance = localStorage.getItem(AUTH_KEY.BALANCE);
  return balance ? parseFloat(balance) : 0;
};

export const removeTokens = () => {
  localStorage.removeItem(AUTH_KEY.ACCESS_TOKEN);
  localStorage.removeItem(AUTH_KEY.USER_NAME);
  localStorage.removeItem(AUTH_KEY.EMAIL);
  localStorage.removeItem(AUTH_KEY.USER_ID);
  localStorage.removeItem(AUTH_KEY.BALANCE);
};

export const isAuthenticated = () => {
  return !!getAccessToken();
};

export const getCurrentUser = () => {
  if (!isAuthenticated()) return null;
  
  return {
    id: getUserId() || '',
    username: getUserName() || '',
    email: getEmail() || '',
    balance: getBalance(),
  };
};