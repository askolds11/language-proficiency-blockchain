import axios from 'axios';

export default () => {
  const http = axios.create({
    baseURL: window.config.eSignUrl,
    withCredentials: false,
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
  });
  return http;
};
