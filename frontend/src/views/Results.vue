<script setup>
import { onMounted , ref } from 'vue';
import { LxList, LxForm, LxRow, lxDateUtils } from '@wntr/lx-ui';
import useAuthStore from '@/stores/useAuthStore';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

async function toDetails(actionId, itemId) {
 router.push({ name: 'resultDetails', params:{entityId:itemId}  });
}

const items = ref([]);
const isLoading = ref(false);

function calculateIELTS(listening, reading, writing, speaking) {

  const average = (listening + reading + writing + speaking) / 4;

  const result = Math.round(average * 2) / 2;
  return result.toString();
}

function calculateTOEFL(reading, listening, speaking, writing) {
  const total = reading + listening + speaking + writing;

  // Ensure final score is an integer
  return Math.round(total).toString();
}

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
    description: `Full score: ${item.testName === 'IELTS' ? calculateIELTS(item.score[0],item.score[1],item.score[2],item.score[3]) : calculateTOEFL(item.score[0],item.score[1],item.score[2],item.score[3])}`,
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
