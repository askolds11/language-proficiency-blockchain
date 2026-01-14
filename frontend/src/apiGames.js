import axios from 'axios';
import { getSessionKey } from '@/apiAuth';

export default () => {
  const http = axios.create({
    baseURL: window.config.serviceUrl,
    withCredentials: false,
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
      Authorization: `Bearer ${getSessionKey()}`,
    },
  });
  return http;
};
