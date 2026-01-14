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
        },
        component: () => import('@/views/Results.vue'),
      },
      {
        path: '/resultDetails',
        name: 'resultDetails',
        meta: {
          title: 'Test list',
          anonymous: true,
          breadcrumbs: [
            {
              text: 'results',
              to: { name: 'results' },
            },
          ],
        },
        component: () => import('@/views/ResultDetails.vue'),
      },
      {
        path: '/authorization',
        name: 'authorization',
        meta: { title: 'authorization' },
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
          title: 'pages.dashboard.title',
          anonymous: true,
        },
        component: () => import('@/views/Dashboard.vue'),
      },

      {
        path: '/sandbox/icon/guidelines',
        name: 'iconGuidelines',
        meta: {
          title: 'pages.icon.title',
          anonymous: true,
          breadcrumbs: [
            {
              text: 'pages.sandbox.title',
              to: { name: 'sandbox' },
            },
          ],
        },
        component: () => import('@/views/guidelines/Icon.vue'),
      },

      {
        path: '/sandbox/data-grid/example',
        name: 'apiDataGrid',
        meta: {
          title: 'pages.dataGrid.title',
          scope: ['game/list'],
          state: 'authorized',
          breadcrumbs: [
            {
              text: 'pages.sandbox.title',
              to: { name: 'sandbox' },
            },
          ],
        },
        component: () => import('@/views/APIData/DataGrid.vue'),
      },
      {
        path: '/sandbox/data-grid/guidelines',
        name: 'dataGridGuidelines',
        meta: {
          title: 'pages.dataGrid.title',
          anonymous: true,
          breadcrumbs: [
            {
              text: 'pages.sandbox.title',
              to: { name: 'sandbox' },
            },
          ],
        },
        component: () => import('@/views/guidelines/DataGrid.vue'),
      },
    ],
  },
];

export default routes;
