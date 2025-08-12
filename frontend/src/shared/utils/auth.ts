export const AUTH_KEY = {
  ACCESS_TOKEN: "accessToken",
  USER_NAME: "userName",
  EMAIL: "email",
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

export const removeTokens = () => {
  localStorage.removeItem(AUTH_KEY.ACCESS_TOKEN);
  localStorage.removeItem(AUTH_KEY.USER_NAME);
  localStorage.removeItem(AUTH_KEY.EMAIL);
};

export const isAuthenticated = () => {
  return !!getAccessToken();
};