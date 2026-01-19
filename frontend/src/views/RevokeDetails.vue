<script setup>
import { ref, onMounted, computed  } from 'vue';
import {
  LxDateTimePicker,
  LxForm,
  LxRow,
  LxTextInput,
  LxModal,
} from '@wntr/lx-ui';
import useAuthStore from '@/stores/useAuthStore';
import { useRouter, useRoute } from 'vue-router';
import useNotifyStore from '@/stores/useNotifyStore';

const notifications = useNotifyStore();
const authStore = useAuthStore();
const router = useRouter();

const testData = ref({
    code: null,
  }
);

const actionDefinitions = computed(() => {
  const actions = [];

if (authStore.session.roles === 'Student') {
 actions.push({
    id: 'revoke',
    name: 'Revoke',
    icon: 'block',
    kind: 'primary',
  })
}

  actions.push({
    id: 'back',
    name: 'Back',
    icon: 'back',
    kind: 'primary',
  })

 return actions;
});

function buttonClicked(actionId){
  if (actionId ==='back') {
    router.push({ name: 'results' });
  } else if (actionId === 'revoke') {
    revoke()
  }
}

async function revoke(){

  const resp = await fetch(`http://localhost:5001/api/internal/test-results/share/${testData.value.code}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${authStore.session.token}`,
        "Content-Type": "application/json",
      }
    });

    if (resp.ok) {
      console.log("Request succeeded!");
      notifications.pushSuccess(`Viewing is revoked for code: ${testData.value.code}!`);
      testData.value.code = null;

    } else {
      console.log("Request failed with code:", resp.status);
      notifications.pushError("Internal server error.");
    }

}
</script>

<template>
  <div>
    <LxForm
      :column-count="1"
      :show-header="false"
      :action-definitions="actionDefinitions"
      @buttonClick="buttonClicked"
    >
        <LxRow label="Code:">
            <LxTextInput
              v-model="testData.code"
            />
        </LxRow>
    </LxForm>
  </div>
</template>
