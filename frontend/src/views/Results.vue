<script setup>
import { shallowRef, onMounted , ref } from 'vue';
import { LxList, LxForm, LxRow, lxDateUtils } from '@wntr/lx-ui';
import useAuthStore from '@/stores/useAuthStore';
import useAppStore from '@/stores/useAppStore';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import useViewStore from '@/stores/useViewStore';

const loading = shallowRef(true);
const busy = shallowRef(false);
const authStore = useAuthStore();
const router = useRouter();
const appStore = useAppStore();
const { t } = useI18n();

async function toDetails(actionId, itemId) {
 router.push({ name: 'resultDetails', params:{entityId:itemId}  });
}

const items = ref([]);
const isLoading = ref(false);

onMounted(async () => {
  isLoading.value=true;
   const resp = await fetch("http://localhost:5001/api/internal/test-results/my", {
      method: "GET",
      headers: {
        Authorization: `Bearer ${authStore.session.token}`,
        "Content-Type": "application/json",
      },
    });

    const data = await resp.json();

    data.forEach(item => {
    items.value.push({
    id: item.testResultId,
    label: `${item.testName} - ${lxDateUtils.formatDate(item.timestamp)}`,
    description: `Full score: ${item.score}`,
    clickable: true,
  });
});
isLoading.value=false;
});
</script>
<template>
  <div>
    <LxForm
      :column-count="2"
      :show-header="false"
      :show-footer="false"
    >
    <LxRow :column-span="2" label="Test list:">
      <LxList
        :items="items"
        listType="1"
        @actionClick="toDetails"
        idAttribute="id"
        primaryAttribute="label"
        secondaryAttribute="description"
        :loading="isLoading"
      />
    </LxRow>
    </LxForm>
  </div>
</template>
