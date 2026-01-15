<script setup>
import { ref, onMounted } from 'vue';
import router from '@/router';
import { LxButton, LxForm, LxRow, LxTextInput } from '@wntr/lx-ui';
import useAuthStore from '@/stores/useAuthStore';

const authStore = useAuthStore();

const user = ref({
  email: '',
  password: ''
});

const isLoading = ref(false);

async function authorize() {
isLoading.value = true;
    try {
    // Send login request
    const resp = await fetch("http://localhost:5001/api/auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        email: "test@test.com",
        password: "password123"
      })
    });

    // Read response body as JSON
    const data = await resp.json();

    console.log("Raw response:", data);

    if (!resp.ok) {
      console.error("Login failed", data);
      return;
    }

    // Update auth store
    authStore.session.st = 'authorized';
    authStore.session.name = data.roles[0];
    authStore.session.token = data.token;
    authStore.session.userId = data.userId;
    authStore.session.email = data.email;
    authStore.session.roles = data.roles[0];
    authStore.session.expiresAt = data.expiresAt;

    console.log("Auth store:", authStore.session);

    // Navigate to dashboard
    router.push({ name: 'dashboard' });

  } catch (err) {
    console.error("Network or parsing error:", err);
  }

  isLoading.value = false;
/*
fetch("http://localhost:5001/api/auth/register", {
  method: "POST",
  headers: {
    "Content-Type": "application/json"
  },
  body: JSON.stringify({
    email: "test@test.com",
    password: "password123"
  })
});
*/
/*
fetch("http://localhost:5001/api/internal/assign-role", {
  method: "POST",
  headers: {
    "Content-Type": "application/json"
  },
  body: JSON.stringify({
    userId: "d4bdbe73-f089-4e0c-b74a-bd296cd87100",
    role: "Student"
  })
});
*/

}

onMounted(async () => {
 if(authStore.session.st === 'authorized') {
  router.push({ name: 'dashboard' });
 }
});

</script>

<template>
  <LxForm
    :show-header="false"
    :show-footer="false"
  >
    <LxRow label="E-mail">
      <LxTextInput v-model="user.email"/>
    </LxRow>
    <LxRow label="Password">
      <LxTextInput kind="password" v-model="user.password"/>
    </LxRow>
    <LxButton label="Authorize" @click="authorize" icon="arrow-right" :loading="isLoading" :busy="isLoading"/>
  </LxForm>
</template>
