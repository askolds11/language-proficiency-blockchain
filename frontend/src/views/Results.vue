<script setup>
import { shallowRef, onMounted , computed } from 'vue';
import { LxList } from '@wntr/lx-ui';
import useAuthStore from '@/stores/useAuthStore';
import useAppStore from '@/stores/useAppStore';
import router from '@/router';
import { useI18n } from 'vue-i18n';
import useViewStore from '@/stores/useViewStore';

const loading = shallowRef(true);
const busy = shallowRef(false);
const authStore = useAuthStore();
const viewStore = useViewStore();
const appStore = useAppStore();
const { t } = useI18n();

async function toDetails() {
 router.push({ name: 'resultDetails' });
}

const roleList = computed(() =>
   [
    {
        id: '1',
        label: 'IELTS 2021.01.01',
        description: 'Full score: 201',
        clickable: true,
    }
  ]
);

onMounted(async () => {
  // TODO:Load list
});
</script>
<template>
  <div>
    <LxList
      :items="roleList"
      @actionClick="toDetails"
      idAttribute="id"
      primaryAttribute="label"
      secondaryAttribute="description"
    />
  </div>
</template>
