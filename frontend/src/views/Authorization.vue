<script setup>
import { ref } from 'vue';
import router from '@/router';
import { LxButton, LxForm, LxRow, LxTextInput } from '@wntr/lx-ui';

const user = ref({
  key: '',
});

async function actionClicked() {

fetch("http://localhost:5001/api/auth/login", {
  method: "POST",
  headers: {
    "Content-Type": "application/json"
  },
  body: JSON.stringify({
    email: "test@test.com",
    password: "password123"
  })
});


  fetch("http://localhost:5001/api/internal/ping")
  .then(r => r.text())
  .then(console.log)
  .catch(console.error);
  // TODO: Authstore ar lomas pieglabāšanu un apmeklējamā id pieglabāšanu

  router.push({ name: 'dashboard' });

}

</script>

<template>
  <LxForm
    :show-header="false"
    :show-footer="false"
  >
    <LxRow label="Key">
      <LxTextInput v-model="user.key"/>
    </LxRow>
    <LxButton label="Authorize" @click="actionClicked"/>
  </LxForm>
</template>
