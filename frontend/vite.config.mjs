/* eslint-disable no-console -- for debugging */
/* eslint-disable no-restricted-imports -- vite doesn`t use aliases in imports */
/* eslint-disable import/no-extraneous-dependencies -- vite mostly should use dev dependencies */
import { defineConfig, loadEnv } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'path';
import { createHtmlPlugin } from 'vite-plugin-html';
import mkcert from 'vite-plugin-mkcert';
import { fileURLToPath } from 'url';
import mockServer from 'vite-plugin-mock-server';
import dns from 'dns';
import packageJson from './package.json';

// set ip4 as default dns lookup in order to support localhost
// https://vitejs.dev/config/server-options.html#server-host
dns.setDefaultResultOrder('ipv4first');

const CONFIG = {
  appName: 'Language proficiency test result system',
  appDescription: 'All results in one place',
  defaultUrl: 'https://localhost:44341/',
  defaultClient: 'LPTRS',
  defaultClientDebug: 'LPTRS',
};

/**
 * @param { ReturnType<getEnvVariables> } env
 * @returns { import('vite').ServerOptions }
 */
const devServerSettings = (env) => {
  const url = new URL(env.BASE_URL);
  return {
    port: url.port,
    https: url.protocol === 'https:',
    proxy: {
      '/api': {
        target: env.VUE_APP_SERVICE_URL_PROXY,
        changeOrigin: true,
        secure: false,
        rewrite: (p) => p.replace(/^\/api/, ''),
      },
      '/idauth': {
        target: env.VUE_APP_AUTH_URL_PROXY,
        changeOrigin: true,
        secure: false,
        rewrite: (p) => p.replace(/^\/idauth/, ''),
      },
    },
  };
};

const getEnvVariables = (mode, serving) => {
  // Load env file based on `mode` in the current working directory.
  // Set the third parameter to '' to load all env regardless of the `VITE_` prefix.
  const env = loadEnv(mode, process.cwd(), '');
  const envVariables = {
    VUE_APP_VERSION: `${packageJson.version}`,
    VUE_APP_ENVIRONMENT: serving ? 'local' : env.NODE_ENV,
    VUE_APP_NAME: CONFIG.appName,
    VUE_APP_DESCRIPTION: CONFIG.appDescription,
    VUE_APP_AUTH_URL: env.AUTH_URL,
    VUE_APP_SERVICE_URL: env.SERVICE_URL,
    VUE_APP_E_SIGN_URL: env.E_SIGN_URL,
    VUE_APP_CLIENT_ID: env.CLIENT_ID,
    VUE_APP_LX_UI_VERSION: `${packageJson.dependencies['@wntr/lx-ui']}`,
    VUE_APP_VUE_VERSION: `${packageJson.dependencies.vue}`,
    VUE_APP_PINIA_VERSION: `${packageJson.dependencies.pinia}`,
    VUE_APP_AXIOS_VERSION: `${packageJson.dependencies.axios}`,
    VUE_APP_VITE_VERSION: `${packageJson.devDependencies.vite}`,
    VUE_APP_VUE_ROUTER_VERSION: `${packageJson.dependencies['vue-router']}`,
    VUE_APP_PKIP_CLIENT_ID: env.PKIP_CLIENT_ID || CONFIG.defaultPKIPClientId,
    BASE_PATH: env.BASE_PATH,
    BASE_URL: env.PUBLIC_URL,

    GIT_LX_UI_URL: env.GIT_LX_UI_URL || 'https://github.com/wntrtech/lx-ui',
    // dev only
    VUE_APP_SERVICE_URL_PROXY: '',
    MOCK_SERVICE: false,
    VUE_APP_AUTH_URL_PROXY: '',
    MOCK_AUTH: false,
  };
  if (serving) {
    envVariables.BASE_URL = env.PUBLIC_URL || CONFIG.defaultUrl;
    envVariables.BASE_PATH = envVariables.BASE_PATH || '/';
    envVariables.VUE_APP_VERSION += '-serve';
    envVariables.VUE_APP_SERVICE_URL = '/api/';
    envVariables.VUE_APP_SERVICE_URL_PROXY = env.SERVICE_URL;
    envVariables.MOCK_SERVICE = !env.SERVICE_URL;
    if (envVariables.MOCK_SERVICE) {
      envVariables.VUE_APP_SERVICE_URL = '/mock-api/';
    }
    envVariables.VUE_APP_AUTH_URL_PROXY = env.AUTH_URL;
    envVariables.MOCK_AUTH = !env.AUTH_URL;
    if (envVariables.MOCK_AUTH) {
      envVariables.VUE_APP_AUTH_URL = '/mock-auth/';
    }
    envVariables.VUE_APP_CLIENT_ID = envVariables.VUE_APP_CLIENT_ID || CONFIG.defaultClientDebug;
  } else {
    envVariables.GIT_LX_UI_URL = '{{GIT_LX_UI_URL}}';
    envVariables.VUE_APP_ENVIRONMENT = '{{ENVIRONMENT}}';
    envVariables.VUE_APP_E_SIGN_URL = '{{E_SIGN_URL}}';
    envVariables.VUE_APP_SERVICE_URL = envVariables.VUE_APP_SERVICE_URL || '{{SERVICE_URL}}';
    envVariables.VUE_APP_AUTH_URL = envVariables.VUE_APP_AUTH_URL || '{{AUTH_URL}}';
    envVariables.BASE_PATH = envVariables.BASE_PATH || './';
    envVariables.BASE_URL = envVariables.BASE_URL || 'DatZ1000/';
    envVariables.VUE_APP_CLIENT_ID = envVariables.VUE_APP_CLIENT_ID || CONFIG.defaultClient;
  }
  return envVariables;
};

/**
 * @returns { import('vite').UserConfigExport }
 * @description https://vitejs.dev/config/
 */
export default defineConfig((command) => {
  const serving = command?.command === 'serve' && command?.mode === 'development';
  const envVariables = getEnvVariables(command.mode, serving);
  const mockUrlPrefixes = serving
    ? [
        envVariables.MOCK_SERVICE && envVariables.VUE_APP_SERVICE_URL,
        envVariables.MOCK_AUTH && envVariables.VUE_APP_AUTH_URL,
      ].filter(Boolean)
    : [];
  const useMockServer = serving && mockUrlPrefixes.length > 0;
  return {
    base: envVariables.BASE_PATH,
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
        '/lx-fonts': fileURLToPath(
          new URL('./node_modules/@wntr/lx-ui/dist/lx-fonts', import.meta.url)
        ),
      },
    },
    plugins: [
      vue(),
      createHtmlPlugin({
        minify: true,
        inject: {
          data: envVariables,
        },
      }),
      useMockServer &&
        mockServer({
          logLevel: 'info',
          urlPrefixes: mockUrlPrefixes,
          mockRootDir: './mock',
          mockJsSuffix: '.mock.js',
          noHandlerResponse404: true,
        }),
      serving && mkcert(),
    ],
    build: {
      // https://vitejs.dev/config/#build-target
      target: ['es2020'],
      outDir: './dist',
      sourcemap: false,
    },
    server: serving ? devServerSettings(envVariables) : {},
    test: {
      globals: true,
      setupFiles: [path.resolve(__dirname, './tests/setup.js')],
      environment: 'jsdom',
      isolate: false,
      include: ['tests/unit/**/*.js'],
    },
  };
});
