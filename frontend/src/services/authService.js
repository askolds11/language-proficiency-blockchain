import apiAuth from '@/apiAuth';

export function session() {
  return apiAuth().get('/api/1.0/session');
}

export function keepAlive() {
  return apiAuth().get('/api/1.0/session/keep-alive');
}

export function roles() {
  return apiAuth().get('/api/1.0/session/roles');
}

export function setRole(roleCode) {
  return apiAuth().patch('/api/1.0/session', { role: roleCode });
}

export function authorize() {
  const params = new URLSearchParams({
    client_id: window.config.clientId,
    scope: 'vpm',
  });
  window.location.href = `${window.config.authUrl}authorize?${params.toString()}`;
}

export function status() {
  return apiAuth().get('/api/1.0/session');
}

export async function logout() {
  await apiAuth().delete('/api/1.0/session');
}
