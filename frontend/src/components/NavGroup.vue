<script setup lang="ts">
import { LxButton } from '@wntr/lx-ui';
import { computed } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();

const currentPage = computed(() => {
  let ret = null;
  switch (route.path) {
    case '/dashboard':
      ret = 'dashboard';
      break;
    case '/utility':
      ret = 'utility';
      break;
    case '/sandbox':
      ret = 'tools';
      break;
    case '/user/profile':
      ret = 'settings';
      break;
    case '/icons':
      ret = 'icons';
      break;
    default:
      return ret;
  }

  return ret;
});
</script>

<template>
  <ul class="lx-nav-group">
    <li :class="{ 'lx-selected': currentPage === 'dashboard' }">
      <LxButton
        :label="$t('pages.dashboard.title')"
        icon="dashboard"
        :href="{ name: 'dashboard' }"
      />
    </li>

    <li :class="{ 'lx-selected': currentPage === 'tools' }">
      <LxButton :label="$t('pages.sandbox.title')" icon="tools" :href="{ name: 'sandbox' }" />
    </li>
    <li :class="{ 'lx-selected': currentPage === 'utility' }">
      <LxButton
        :label="$t('pages.utility.title')"
        icon="registry-settings"
        :href="{ name: 'utility' }"
      />
    </li>
    <li :class="{ 'lx-selected': currentPage === 'icons' }">
      <LxButton :label="$t('pages.icons.title')" icon="icons" :href="{ name: 'icons' }" />
    </li>
  </ul>
</template>
