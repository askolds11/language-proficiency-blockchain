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
const route = useRoute();

const sharingModal = ref({});

const testData = ref({
    key: null,
    type: null,
    readingScore: null,
    writtingScore: null,
    speakingScore: null,
    listeningScore: null,
    totalScore: null,
    dateofExamination: null,
    resultInputterCode: null,
  }
);

const modalData = ref({
  expiration:null,
  code:null,
})

const actionDefinitions = computed(() => {
  const actions = [];

  if (authStore.session.roles === 'Verificator') {
    actions.push({
        id: 'verify',
        name: 'Verify',
        icon: 'check',
        kind: 'tertiary',
      })
  }

  if (authStore.session.roles === 'Student') {
  actions.push({
      id: 'generate',
      name: 'Get share code',
      icon: 'share',
      kind: 'tertiary',
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
  } else if (actionId === 'generate') {
    modalData.value.expiration = null;
    modalData.value.code = null;
    showCode.value = false;
    sharingModal.value.open();
  }
}

function calculateIELTS(listening, reading, writing, speaking) {
  const average = (listening + reading + writing + speaking) / 4;

  // Round to nearest 0.5
  return Math.round(average * 2) / 2;
}

function calculateTOEFL(reading, listening, speaking, writing) {
  const total = reading + listening + speaking + writing;

  // Ensure final score is an integer
  return Math.round(total).toString();
}

const showCode = ref(false);

async function generate(){

  const localDate = new Date(modalData.value.expiration);

  const resp = await fetch("http://localhost:5001/api/internal/test-results/share", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${authStore.session.token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        testResultId: route.params.entityId,
        expiresAt: localDate.toISOString(),
      })
    });

    if (resp.ok) {
      console.log("Request succeeded!");
      notifications.pushSuccess("Code is generated and displayed!");
      showCode.value = true;

      const data = await resp.json();
      modalData.value.code = data.code;


    } else {
      console.log("Request failed with code:", resp.status);
      notifications.pushError("Internal server error.");
    }

}

const minDate = ref('');

function toDateTimeOffset(date = new Date()) {
  const pad = (n) => String(n).padStart(2, "0");

  const year = date.getFullYear();
  const month = pad(date.getMonth() + 1);
  const day = pad(date.getDate());

  const hours = pad(date.getHours());
  const minutes = pad(date.getMinutes());
  const seconds = pad(date.getSeconds());

  const offsetMinutes = -date.getTimezoneOffset();
  const sign = offsetMinutes >= 0 ? "+" : "-";
  const offsetHours = pad(Math.floor(Math.abs(offsetMinutes) / 60));
  const offsetMins = pad(Math.abs(offsetMinutes) % 60);

  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}${sign}${offsetHours}:${offsetMins}`;
}

const finalResult = ref(null);

onMounted(async () => {

  const resp = await fetch("http://localhost:5001/api/internal/test-results/my", {
      method: "GET",
      headers: {
        Authorization: `Bearer ${authStore.session.token}`,
        "Content-Type": "application/json",
      },
    });

    const data = await resp.json();

    testData.value= data.find((obj) => obj.testResultId === route.params.entityId);
    testData.value.readingScore = testData.value.score[0];
    testData.value.writtingScore = testData.value.score[1];
    testData.value.speakingScore = testData.value.score[2];
    testData.value.listeningScore = testData.value.score[3];

    if (testData.value.testName === 'IELTS'){
      finalResult.value = calculateIELTS(testData.value.score[0],testData.value.score[1],testData.value.score[2],testData.value.score[3])
    } else {
      finalResult.value = calculateTOEFL(testData.value.score[0],testData.value.score[1],testData.value.score[2],testData.value.score[3])
    }

    minDate.value = toDateTimeOffset();
});

</script>

<template>
  <div>
    <LxForm
      :column-count="1"
      :show-header="false"
      :action-definitions="actionDefinitions"
      @buttonClick="buttonClicked"
    >
        <LxRow label="Type">
            <LxTextInput
              v-model="testData.testName"
              :read-only="true"
            />
        </LxRow>
        <LxRow label="Reading score">
            <LxTextInput
              v-model="testData.readingScore"
              :read-only="true"
            />
        </LxRow>
        <LxRow label="Writting score">
            <LxTextInput 
              v-model="testData.writtingScore" 
              :read-only="true"
              />
        </LxRow>
        <LxRow label="Speaking score">
            <LxTextInput
              v-model="testData.speakingScore"
              :read-only="true"
            />
        </LxRow>
        <LxRow label="Listening score">
            <LxTextInput 
              v-model="testData.listeningScore"
              :read-only="true"
            />
        </LxRow>
        <LxRow label="Total score">
            <LxTextInput 
              v-model="finalResult"
              :read-only="true"
            />
        </LxRow>
        <LxRow label="Date of examination">
            <LxDateTimePicker
              v-model="testData.timestamp"
              :read-only="true"
            />
        </LxRow>
        <LxModal
          ref="sharingModal"
          :label="'Sharable code generation'"
          :button-primary-label="'Generate'"
          :button-primary-visible="true"
          :button-secondary-label="'Back'"
          :button-secondary-visible="true"
          :button-secondary-is-cancel="false"
          :disable-closing="true"
          @primary-action="generate()"
          @secondary-action="sharingModal.close()"
        >
    <LxRow label="Expiration date and time:">
      <LxDateTimePicker
        :minDate="minDate"
        v-model="modalData.expiration"
        :kind="'date-time'"
      />
      <LxRow v-if="showCode" label="Code:">
            <LxTextInput 
              v-model="modalData.code"
              :read-only="true"
            />
        </LxRow>
    </LxRow>
    </LxModal>
    </LxForm>
  </div>
</template>
