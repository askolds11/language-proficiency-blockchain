<script setup>
import { ref, onMounted, computed  } from 'vue';
import {
  LxList,
  LxForm,
  LxRow,
  LxTextInput,
  LxValuePicker,
} from '@wntr/lx-ui';
import useAuthStore from '@/stores/useAuthStore';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

const entity = ref({
  selectedItems: {},
  id: null,
  time: null
});

const testList = computed(() =>
   [
    {
        id: '1',
        label: 'IELTS 2021.01.01',
        description: 'Full score: 201',
        clickable: true,
    }
  ]
);

const actionDefinitions = [
  {
    id: 'allow',
    name: 'Allow',
    icon: 'check',
    kind: 'primary',
  }
];

function buttonClicked(actionId){
console.log(entity.value)
}


onMounted(async () => {

    // TODO: Saraksta ielāde

});

</script>

<template>
  <div>
    <LxForm
      :column-count="2"
      :show-header="false"
      :action-definitions="actionDefinitions"
      @buttonClick="buttonClicked"
    >
    <LxRow label="Test list:" :column-span="2">
      <LxList
        v-model:selectedItems="entity.selectedItems"
        :items="testList"
        :hasSelecting="true"
        selectingKind="multiple"
        @actionClick="toDetails"
        idAttribute="id"
        primaryAttribute="label"
        secondaryAttribute="description"
      />
    </LxRow>
    <LxRow label="Send to:">
      <LxTextInput v-model="entity.id"></LxTextInput>
    </LxRow>
    <LxRow label="For how long:">
      <LxTextInput v-model="entity.time"></LxTextInput>
    </LxRow>
    </LxForm>
  </div>
</template>
