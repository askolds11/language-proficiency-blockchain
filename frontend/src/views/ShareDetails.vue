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
    code: null,
    key: null,
    name: null,
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
    disabled: testData.value.name === null
  }),
  actions.push({
    id: 'get',
    name: 'Get results',
    icon: 'check',
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

const selectedStudent = ref({})

async function getResults(){
    const resp = await fetch(`http://localhost:5001/api/internal/test-results/shared/${testData.value.code}`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${authStore.session.token}`,
        "Content-Type": "application/json",
      },
    });

    if (resp.ok) {
      console.log("Request succeeded!");
      notifications.pushSuccess("Results are displayed!");

      const data = await resp.json();
      testData.value = data;

      selectedStudent.value = students.value.find((obj)=>obj.id===testData.value.studentId);
      testData.value.name = `${selectedStudent.value.name} ${selectedStudent.value.surname}`

    } else {
      console.log("Request failed with code:", resp.status);
      notifications.pushError(`No test available for code ${testData.value.code}`);
    }
}

function buttonClicked(actionId){
if (actionId ==='back') {
  router.push({ name: 'dashboard' });
} else if (actionId === 'verify') {
 verify()
} else if (actionId === 'get') {
  getResults();
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

const finalResult = computed(() => {
    if (testData.value.writtingScore && testData.value.listeningScore && testData.value.readingScore && testData.value.speakingScore) {
        if (testData.value.type === 'IELTS') {
            calculateIELTS(Number(testData.value.writtingScore),Number(testData.value.listeningScore),Number(testData.value.readingScore),Number(testData.value.speakingScore));
        } else {
            calculateTOEFL(Number(testData.value.writtingScore),Number(testData.value.listeningScore),Number(testData.value.readingScore),Number(testData.value.speakingScore));
        }
    } else {
        return '-';
    }
});

async function verify(){

  const resp = await fetch("http://localhost:5001/api/internal/test-results/verify", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${authStore.session.token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        testResultId: testData.value.testResultId,
        testId: testData.value.testId,
        score: testData.value.score,
        prevHashHex: testData.value.prevBlockHash
      })
    });

    if (resp.ok) {
      console.log("Request succeeded!");

      const data = await resp.json();

      if(data.isValid){
        notifications.pushSuccess("Result is legitimate!");
      } else {
        notifications.pushSuccess("Result is not legitimate!");
      }
    } else {
      console.log("Request failed with code:", resp.status);
      notifications.pushError("Internal server error.");
    }

}

const students = ref([]);

onMounted(async () => {

const resp = await fetch("http://localhost:5001/api/internal/students", {
  method: "GET",
  headers: {
    Authorization: `Bearer ${authStore.session.token}`,
  },
});

const data = await resp.json();



data.forEach(item => {
  students.value.push({
    id: item.id,
    name: item.name,
    surname: item.surname
  });
});

});

</script>

<template>
  <div>
    <LxRow label="Code:">
            <LxTextInput
              v-model="testData.code"
            />
        </LxRow>
        <br/>
    <LxForm
      :column-count="1"
      :show-header="false"
      :action-definitions="actionDefinitions"
      @buttonClick="buttonClicked"
    >
    
        <LxRow label="Name">
            <LxTextInput
              v-model="testData.name"
              :read-only="true"
            />
            </LxRow>
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
    </LxForm>
  </div>
</template>
