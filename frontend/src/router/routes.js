const routes = [
  {
    path: '/',
    name: 'home',
    component: () => import('@/layouts/MainLayout.vue'),
    meta: {
      title: 'pages.home.title',
    },
    children: [
      {
        path: '/dashboard',
        name: 'dashboard',
        meta: { title: 'Start' },
        component: () => import('@/views/Dashboard.vue'),
      },
      {
        path: '/results',
        name: 'results',
        meta: {
          title: 'Tests',
          anonymous: true,
          breadcrumbs: [
            {
              text: 'Dashboard',
              to: { name: 'dashboard' },
            },
          ],
        },
        component: () => import('@/views/Results.vue'),
      },
      {
        path: '/result-details/:entityId',
        name: 'resultDetails',
        meta: {
          title: 'Test list',
          anonymous: true,
          breadcrumbs: [
            {
              text: 'Results',
              to: { name: 'results' },
            },
          ],
        },
        component: () => import('@/views/ResultDetails.vue'),
      },
      {
        path: '/operator-result-details',
        name: 'operatorResultDetails',
        meta: {
          title: 'Result input',
          breadcrumbs: [
            {
              text: 'Dashboard',
              to: { name: 'dashboard' },
            },
          ],
        },
        component: () => import('@/views/OperatorResultDetails.vue'),
      },
      {
        path: '/share',
        name: 'share',
        meta: {
          title: 'Results by code',
          anonymous: true,
          breadcrumbs: [
            {
              text: 'Dashboard',
              to: { name: 'dashboard' },
            },
          ],
        },
        component: () => import('@/views/ShareDetails.vue'),
      },
      {
        path: '/revoke',
        name: 'revoke',
        meta: {
          title: 'Revoke code',
          anonymous: true,
          breadcrumbs: [
            {
              text: 'Dashboard',
              to: { name: 'dashboard' },
            },
          ],
        },
        component: () => import('@/views/RevokeDetails.vue'),
      },
      {
        path: '/authorization',
        name: 'authorization',
        meta: { title: 'Authorization' },
        component: () => import('@/views/Authorization.vue'),
      },
      {
        path: '/login',
        name: 'login',
        meta: { title: 'pages.login.title' },
        component: () => import('@/views/Login.vue'),
      },
      {
        path: '/dashboard',
        name: 'dashboard',
        meta: {
          title: 'Dashboard',
          anonymous: true,
        },
        component: () => import('@/views/Dashboard.vue'),
      },
    ],
  },
];

export default routes;
