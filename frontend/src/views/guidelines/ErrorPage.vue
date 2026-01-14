<script setup lang="ts">
import { shallowRef } from 'vue';
import { LxContentSwitcher } from '@wntr/lx-ui';
import { useRouter } from 'vue-router';

const router = useRouter();
const routerValue = shallowRef('guidelines');
function contentSwitcherClick() {
  if (routerValue.value === 'sandbox') {
    router.push({ name: 'errorPage' });
  }
}
</script>
<template>
  <LxContentSwitcher
    :items="[
      { id: 'sandbox', name: 'Smilškaste' },
      { id: 'guidelines', name: 'Vadlīnijas' },
    ]"
    v-model="routerValue"
    @click="contentSwitcherClick"
  />
  <article class="lx-article"></article>
</template>
