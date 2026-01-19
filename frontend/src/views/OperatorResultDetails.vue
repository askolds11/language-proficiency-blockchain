<script setup>
import { ref, onMounted, computed  } from 'vue';
import {
  LxDateTimePicker,
  LxForm,
  LxRow,
  LxTextInput,
  LxValuePicker,
} from '@wntr/lx-ui';
import useAuthStore from '@/stores/useAuthStore';
import { useRouter } from 'vue-router';
import useNotifyStore from '@/stores/useNotifyStore';

const authStore = useAuthStore();
const router = useRouter();
const notifications = useNotifyStore();

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

const invalidFields = ref({});

const actionDefinitions = [
{
    id: 'save',
    name: 'Save',
    icon: 'save',
    kind: 'primary',
  },
  {
    id: 'cancel',
    name: 'Cancel',
    icon: 'cancel',
    kind: 'secondary',
  }
];

function nullAllExcept(obj) {
  Object.keys(obj).forEach(key => {
    if (key !== 'resultInputterCode') {
      obj[key] = null;
    }
  });
}

async function buttonClicked(actionId){
  if (actionId === 'save') {
    if (validate()) {
      const localDate = new Date(testData.value.dateOfExamination);
      let randomId = crypto.randomUUID();
      const resp = await fetch("http://localhost:5001/api/internal/test-result", {
        method: "POST",
        headers: {
          Authorization: `Bearer ${authStore.session.token}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          blockId: randomId,
          testResultId: randomId,
          testId: testData.value.type,
          studentId: testData.value.key,
          score: [Number(testData.value.readingScore), Number(testData.value.writtingScore), Number(testData.value.speakingScore), Number(testData.value.listeningScore)],
          timestamp: localDate.toISOString()
        })
      });

      if (resp.ok) {
        console.log("Request succeeded!");
        notifications.pushSuccess("Result saved!");
        router.push({ name: 'dashboard' });
      } else {
        console.log("Request failed with code:", resp.status);
        notifications.pushError("Internal server error.");
      }
    }
  } else if (actionId === 'cancel') {
    nullAllExcept(testData);
  }
}

const testTypes = [
    {
        id:'019bcd60-fa47-7d29-a7d0-df4b7f43cfef',
        name:'IELTS',
    },
    {
        id:'019bcd9d-ca1b-74f9-8a9c-d71c8b3a0565',
        name:'TOEFL',
    },
]

function validate(){
    invalidFields.value = {};
    let isValid = true;

    if (!testData.value.key) {
      invalidFields.value.key = 'Mandatory';
      isValid = false;
    }

    if (!testData.value.type) {
      invalidFields.value.type = 'Mandatory';
      isValid = false;
    }

    if (!testData.value.readingScore) {
      invalidFields.value.readingScore = 'Mandatory';
      isValid = false;
    }

    if (!testData.value.writtingScore) {
      invalidFields.value.writtingScore = 'Mandatory';
      isValid = false;
    }

    if (!testData.value.speakingScore) {
      invalidFields.value.speakingScore = 'Mandatory';
      isValid = false;
    }

    if (!testData.value.listeningScore) {
      invalidFields.value.listeningScore = 'Mandatory';
      isValid = false;
    }

    if (!testData.value.dateOfExamination) {
      invalidFields.value.dateOfExamination = 'Mandatory';
      isValid = false;
    }
    return isValid;

}

const mask = computed(() => {
  if (testData.value.type === 'IELTS') {
  return 'decimal';
  }
  return 'integer';
});

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

const finalResult = computed(() => {
  console.log(testData.value)
    if (testData.value.writtingScore && testData.value.listeningScore && testData.value.readingScore && testData.value.speakingScore) {
        if (testData.value.type === '019bcd60-fa47-7d29-a7d0-df4b7f43cfef') {
            return calculateIELTS(Number(testData.value.writtingScore),Number(testData.value.listeningScore),Number(testData.value.readingScore),Number(testData.value.speakingScore));
        } else {
            return calculateTOEFL(Number(testData.value.writtingScore),Number(testData.value.listeningScore),Number(testData.value.readingScore),Number(testData.value.speakingScore));
        }
    } else {
        return '-';
    }
});

const students = ref([]);

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

const maxDate = ref('')

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
    name: `${item.name} ${item.surname}`,
  });
});

testData.value.type = testTypes[0].id;
maxDate.value = toDateTimeOffset();

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
        <LxRow label="Name">
            <LxValuePicker 
            v-model="testData.key"
            :items="students"
            :variant="'dropdown'"
            :read-only="false"
            :invalid="invalidFields.key"
            :invalidationMessage="invalidFields.key"
        />
        </LxRow>
        <LxRow label="Type">
            <LxValuePicker
              v-model="testData.type"
              :items="testTypes"
              :variant="'tags'"
              :mask="mask"
              :read-only="false"
              :invalid="invalidFields.type"
              :invalidationMessage="invalidFields.type"
            />
        </LxRow>
        <LxRow label="Reading score">
            <LxTextInput
              v-model="testData.readingScore"
              :mask="mask"
              :read-only="false"
              :invalid="invalidFields.readingScore"
              :invalidationMessage="invalidFields.readingScore"
            />
        </LxRow>
        <LxRow label="Writting score">
            <LxTextInput 
              v-model="testData.writtingScore" 
              :mask="mask"
              :read-only="false"
              :invalid="invalidFields.writtingScore"
              :invalidationMessage="invalidFields.writtingScore"
              />
        </LxRow>
        <LxRow label="Speaking score">
            <LxTextInput
              v-model="testData.speakingScore"
              :mask="mask"
              :read-only="false"
              :invalid="invalidFields.speakingScore"
              :invalidationMessage="invalidFields.speakingScore"
            />
        </LxRow>
        <LxRow label="Listening score">
            <LxTextInput 
              v-model="testData.listeningScore"
              :mask="mask"
              :read-only="false"
              :invalid="invalidFields.listeningScore"
              :invalidationMessage="invalidFields.listeningScore"
            />
        </LxRow>
        <LxRow label="Total score">
            <LxTextInput 
              v-model="finalResult"
              :invalid="invalidFields.totalScore"
              :invalidationMessage="invalidFields.totalScore"
              :read-only="true"
            />
        </LxRow>
        <LxRow label="Date of examination">
            <LxDateTimePicker
              v-model="testData.dateOfExamination"
              :mask="mask"
              :maxDate="maxDate"
              :read-only="false"
              :invalid="invalidFields.dateOfExamination"
              :invalidationMessage="invalidFields.dateOfExamination"
            />
        </LxRow>
    </LxForm>
  </div>
</template>
