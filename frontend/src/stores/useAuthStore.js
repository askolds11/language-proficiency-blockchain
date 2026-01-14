import { defineStore } from 'pinia';
import { LxAuthStore, LxAuthService } from '@wntr/lx-ui';

export default defineStore(
  'authStore',
  LxAuthStore(
    LxAuthService,
    window.config.authUrl,
    window.config.publicUrl,
    window.config.clientId,
    'vpm',
    'demo-sessionkey'
  )
);
