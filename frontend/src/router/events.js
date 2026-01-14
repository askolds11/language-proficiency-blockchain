import useAuthStore from '@/stores/useAuthStore';
import useAppStore from '@/stores/useAppStore';
import { lxFlowUtils } from '@wntr/lx-ui';

export default (router) => {
  router.beforeEach(async (to, from, next) => {
    const authStore = useAuthStore();
    const appStore = useAppStore();
    await lxFlowUtils.beforeEach(to, from, next, appStore, authStore);
  });
  router.afterEach(async (to, from) => {
    const appStore = useAppStore();
    await lxFlowUtils.afterEach(to, from, appStore);
  });
};
