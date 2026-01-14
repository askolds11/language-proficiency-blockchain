import axios from 'axios';

export default () => {
  const http = axios.create({
    baseURL: window.config.serviceUrl,
    withCredentials: true,
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
  });
  return http;
};