import axios from 'axios';

const KEY_TOKEN_SESSION = 'demo-sessionkey';

export const setSessionKey = (key) => {
  sessionStorage.setItem(KEY_TOKEN_SESSION, key);
};

export const getSessionKey = () => sessionStorage.getItem(KEY_TOKEN_SESSION);

export const removeSessionKey = () => sessionStorage.removeItem(KEY_TOKEN_SESSION);

export default () => {
  const http = axios.create({
    baseURL: window.config.authUrl,
    withCredentials: false,
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
      Authorization: `Bearer ${getSessionKey()}`,
    },
  });
  return http;
};
