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

const authStore = useAuthStore();
const router = useRouter();

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


function buttonClicked(actionId){
if (actionId === 'save') {
  if (validate()) {
    //TODO Save call
  }
} else if (actionId === 'cancel') {
  nullAllExcept(testData);
}
}

const testTypes = [
    {
        id:'IELTS',
        name:'IELTS',
    },
    {
        id:'TOEFL',
        name:'TOEFL',
    },
]



function validate(){

    invalidFields.value = {};
    let isValid = true;

    if (!testData.key) {
      invalidFields.value.key = 'Mandatory';
      isValid = false;
    }

    if (!testData.type) {
      invalidFields.value.type = 'Mandatory';
      isValid = false;
    }

    if (!testData.readingScore) {
      invalidFields.value.readingScore = 'Mandatory';
      isValid = false;
    }

    if (!testData.writtingScore) {
      invalidFields.value.writtingScore = 'Mandatory';
      isValid = false;
    }

    if (!testData.speakingScore) {
      invalidFields.value.speakingScore = 'Mandatory';
      isValid = false;
    }

    if (!testData.listeningScore) {
      invalidFields.value.listeningScore = 'Mandatory';
      isValid = false;
    }

    if (!testData.dateOfExamination) {
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



onMounted(async () => {
testData.value.type = testTypes[0].id;
    // TODO: Lomas pārbaude
// TODO: Ja ir kāds, kas skatās rezultātus, tad šeit notiek datu ielāde
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
        <LxRow label="Key">
            <LxTextInput 
            v-model="testData.key"
            :mask="mask"
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
              :mask="mask"
              :invalid="invalidFields.totalScore"
              :invalidationMessage="invalidFields.totalScore"
              :read-only="true"
            />
        </LxRow>
        <LxRow label="Date of examination">
            <LxDateTimePicker
              v-model="testData.dateOfExamination"
              :mask="mask"
              :read-only="false"
              :invalid="invalidFields.dateOfExamination"
              :invalidationMessage="invalidFields.dateOfExamination"
            />
        </LxRow>
    </LxForm>
  </div>
</template>
