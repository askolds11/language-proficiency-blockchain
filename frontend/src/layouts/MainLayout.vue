<script setup>
import { computed, ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { LxShell, LxIcon, lxDateUtils } from '@wntr/lx-ui';
import { invoke, until, useIdle, useIntervalFn } from '@vueuse/core';


import LoginView from '@/views/Login.vue';
import useErrors from '@/hooks/useErrors';
import useAuthStore from '@/stores/useAuthStore';
import useAppStore from '@/stores/useAppStore';
import useNotifyStore from '@/stores/useNotifyStore';
import useConfirmStore from '@/stores/useConfirmStore';
import useViewStore from '@/stores/useViewStore';
import CoverBackground from '@/components/CoverBackground.vue';

const authStore = useAuthStore();
const notify = useNotifyStore();
const viewStore = useViewStore();
const errors = useErrors();
const router = useRouter();
const confirmStore = useConfirmStore();
const appStore = useAppStore();

const secondsToIdle = 10;
const secondsCheckApiInterval = 30;

const { idle } = useIdle(secondsToIdle * 1000);

const idleModalOpened = ref(false);

// ToDo: develop login & get session
// eslint-disable-next-line no-unused-vars

const i18n = useI18n();
const { t } = useI18n();
const route = useRoute();
const routes = router.getRoutes();
const shellMode = computed(() => {
  let ret = 'public';
  if (route.name === 'home') {
    ret = 'cover';
  }
  return ret;
});


const systemName = computed(() => i18n.t('title.shortName'));

const pageTitle = computed(() => {
  if (typeof route.meta.title === 'function') {
    return route.meta.title(i18n);
  }
  if (typeof route.meta.title === 'string') {
    return viewStore?.pageTitle || i18n.t(route.meta.title);
  }
  return '';
});

const pageDescription = computed(() => {
  if (typeof route.meta.description === 'function') {
    return route.meta.description(i18n);
  }
  if (typeof route.meta.description === 'string') {
    return viewStore?.pageDescription || i18n.t(route.meta.description);
  }
  return '';
});


const nav = computed(() => {
  const items = [];
  if (authStore.session.st !== 'authorized') {
    return items;
  }
  items.push({
    label: 'Dashboard',
    icon: 'dashboard',
    to: { name: 'dashboard' },
  })
  if (authStore.session.roles === 'Student' ) {
    items.push({
    label: 'Results',
    to: { name: 'results' },
  })
  }
  if (authStore.session.roles === 'Operator') {
    items.push({
    label: 'Add test',
    to: { name: 'operatorResultDetails' },
  })
  }
  if (authStore.session.roles === 'Verificator') {
    items.push({
    label: 'Results by code',
    to: { name: 'share' },
  })
  }
  if (authStore.session.roles === 'Student') {
    items.push({
    label: 'Revoke viewing',
    to: { name: 'revoke' },
  })
  }
  return items;
});

const breadcrumbs = computed(() => {
  const ret = [];

  if (route.meta.breadcrumbs) {
    // @ts-ignore
    route.meta.breadcrumbs.forEach((item) => {
      ret.push({
        label: viewStore?.backRouteName || i18n.t(item.text),
        to: item.to,
      });
    });
  }
  return ret;
});

const showBackButton = computed(() =>
  viewStore?.canGoBack === false
    ? viewStore.canGoBack
    : breadcrumbs.value.length > 0,
);

const selectedNavItems = computed(() => {
  const ret = {};
  ret[router.currentRoute.value.name] = true;
  if (route.meta?.breadcrumbs) {
    // @ts-ignore
    route.meta?.breadcrumbs.forEach((item) => {
      ret[item.to?.name] = true;
    });
  }
  return ret;
});

function goBack(path) {
  if (path !== -1) {
    router.push(path);
  } else {
    router.back();
  }
}
function goHome(path) {
  router.push(path);
}

onMounted(() => {
  if (authStore.session.active) {
    authStore.keepAlive();
  }
});

const userInfo = computed(() => {
  if (authStore.isAuthorized) {
    return {
      firstName: authStore.session?.email,
      lastName: authStore.session?.roles,
    };
  }
  return null;
});

const closeModal = () => {
  idleModalOpened.value = false;
};

const openModal = () => {
  idleModalOpened.value = true;
};

async function logout() {
  try {
    const resp = await authStore.logout();
    if (resp?.status === 200 && resp?.data) {
      window.location.href = resp.data;
    } else {
      notify.pushSuccess('Signed out');
    }
  } catch (err) {
    const error = errors.get(err);
    if (error.status !== 401 && error.data) {
      notify.pushError(error.data);
    }
  } finally {
    closeModal();
    router.push({ name: 'home' });
  }
  console.log('start')
  console.log(authStore.session)
}

function primary() {
  logout();
  confirmStore.$state.isOpen = false;
}
function secondary() {
  confirmStore.$state.isOpen = false;
}

function openConfirmModal() {
  confirmStore.push(
    "Sign out",
    "Really want to sign out?",
    "Yes",
    "No",
    primary,
    secondary,
  );
}

function confirmModalClosed() {
  confirmStore.$state.isOpen = false;
}

async function getSession() {
  try {
    await authStore.fetchSession();
  } catch (err) {
    const error = errors.get(err);
    if (error.status === 401) {
      logout();
    } else if (error.data) {
      notify.pushError(error.data);
    }
  }
}

async function callKeepAlive() {
  try {
    await authStore.keepAlive();
  } catch (err) {
    const error = errors.get(err);
    if (error.status === 401) {
      logout();
    } else if (error.data) {
      notify.pushError(error.data);
    }
  }
}

const checkApiSession = () => {
  if (idle.value || idleModalOpened.value) {
    getSession();
  } else {
    callKeepAlive();
  }
};

useIntervalFn(() => {
  if (!authStore.session.active) {
    if (idleModalOpened.value) {
      closeModal();
      router.push({ name: 'sessionTimeout' });
    }
    return;
  }
  if (authStore.session.secondsToLive < 1) {
    logout();
    closeModal();
    return;
  }
  if (authStore.session.secondsToLive < authStore.session.secondsToCountdown) {
    if (!idleModalOpened.value) {
      openModal();
    }
  } else if (idleModalOpened.value) {
    closeModal();
    return;
  }
  const refreshIntervals =
    authStore.session.secondsToLive % secondsCheckApiInterval === 0;
  const refreshBeforeWarn =
    authStore.session.secondsToLive - 3 <
      authStore.session.secondsToCountdown && !idle.value;
  const refreshBeforeLogout = authStore.session.secondsToLive === 3;
  if (refreshIntervals || refreshBeforeWarn || refreshBeforeLogout) {
    checkApiSession();
  }
  authStore.session.secondsToLive -= 1;
}, 1000);

async function continueSession() {
  try {
    await authStore.keepAlive();
    notify.pushSuccess(i18n.t('shell.notifications.sessionContinued'));
  } catch (err) {
    notify.pushError(i18n.t('shell.notifications.sessionContinuedFailed'));
    if (err.response.status === 401) {
      logout();
    }
  } finally {
    closeModal();
  }
}

invoke(async () => {
  // @ts-ignore
  await until(() => authStore.showSessionEndCountdown).toBe(true);
  notify.pushWarning(i18n.t('shell.notifications.sessionEndingSoon'));
});

function idleModalPrimary() {
  continueSession();
}
function idleModalSecondary() {
  logout();
}
</script>
<template>
  <div>
    <div>
      <LxShell
        :system-name="i18n.t('title.fullName')"
        :system-subheader="i18n.t('title.subheader')"
        :system-name-short="systemName"
        :user-info="userInfo"
        :mode="shellMode"
        :nav-items="nav"
        :nav-items-selected="selectedNavItems"
        :page-label="pageTitle"
        :pageDescription="pageDescription"
        :page-back-button-visible="showBackButton"
        :page-breadcrumbs="breadcrumbs"
        :page-index-path="{ name: 'home' }"
        :has-cover-logo="true"
        :cover-image="null"
        :cover-image-dark="null"
        :cover-logo="null"
        :has-theme-picker="true"
        :navigating="appStore.$state.isNavigating"
        :showIdleModal="idleModalOpened"
        :showIdleBadge="
          authStore.session.secondsToLive <
            authStore.session.secondsToCountdown &&
          !authStore.session.isSessionExtendable
        "
        :secondsToLive="authStore.session.secondsToLive"
        :confirmDialogData="confirmStore"
        :confirmPrimaryButtonBusy="false"
        :confirmPrimaryButtonDestructive="true"
        v-model:notifications="notify.notifications"
        :hideNavBar="!viewStore?.isNavBarShown"
        :headerNavDisable="viewStore.blockNav"
        :hideHeaderText="!viewStore?.isHeaderShown"
        @confirmModalClosed="confirmModalClosed"
        @go-home="goHome"
        @go-back="goBack"
        @log-out="openConfirmModal"
        @idleModalPrimary="idleModalPrimary"
        @idleModalSecondary="idleModalSecondary"
      >
        <template #backdrop>
          <CoverBackground />
        </template>
        <template #coverArea>
          <div class="lx-button-set">
            <LoginView />
          </div>
        </template>
        <router-view />
      </LxShell>
    </div>
  </div>
</template>
