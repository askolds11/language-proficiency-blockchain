<script setup>
import { onMounted, ref, computed } from 'vue';
import {
  LxForm,
  LxList,
  LxFormBuilder,
  LxContentSwitcher,
  LxToggle,
  LxModal,
  LxTextArea,
} from '@wntr/lx-ui';

import { useDemoStore } from '@/stores/useDemoStore';
import useViewStore from '@/stores/useViewStore';
import useNotifyStore from '@/stores/useNotifyStore';

const demoStore = useDemoStore();
const viewStore = useViewStore();
const notificationStore = useNotifyStore();

const testData = ref(demoStore.testData);
const videoGames = ref(demoStore.videoGamesComputed);

const readOnly = ref(false);

const example = ref('exampleStandard');

const list = ref([
  { id: 'help', name: 'LxFormBuilder apraksts', clickable: true, icon: 'help' },
  { id: 'json', name: 'JSON schema specifikācija', clickable: true, icon: 'help' },
  { id: 'source', name: 'Kods', clickable: true, icon: 'gitea', iconSet: 'brand' },
]);

const actionDefinitionsVal = [
  { id: 'validate', name: 'Validēt', kind: 'primary', icon: 'accept' },
  { id: 'edit', name: 'Labot shēmu', kind: 'secondary', icon: 'edit-item' },
  { id: 'resetSchemas', name: 'Atiestatīt shēmas', kind: 'additional', icon: 'reset' },
  { id: 'reset', name: 'Nonullēt modeli', kind: 'additional', icon: 'remove-item' },
];

function clicked(action, id) {
  console.log(id);
  if (id === 'help') {
    window.open('https://git.zzdats.lv/lx/ui/src/branch/master/docs/FormBuilder.md', '_blank');
  }
  if (id === 'json') {
    window.open('https://json-schema.org/', '_blank');
  }
  if (id === 'source') {
    window.open(
      'https://git.zzdats.lv/lx/ui/src/branch/master/src/components/forms/FormBuilder.vue',
      '_blank'
    );
  }
}
const builder = ref();

const schemas = ref([
  {
    $schema: 'http://json-schema.org/draft-07/schema#',
    type: 'object',
    properties: {
      animalName: {
        type: 'string',
        title: 'Dzīvnieka vārds',
      },
      animalType: {
        type: 'string',
        title: 'Veids',
      },
      birthDate: {
        type: 'string',
        format: 'date',
        title: 'Dzimšanas datums',
      },
      weight: {
        type: 'number',
        exclusiveMinimum: 0.1,
        maximum: 100,
        title: 'Svars',
        description: 'Dzīvnieka svars pilnajos kilogramos, apaļojot uz leju',
      },
      isRegistered: {
        type: 'boolean',
        title: 'Dzīvnieks ir reģistrēts',
        default: false,
      },

      microchipNumber: {
        type: 'string',
        title: 'Mikročipa numurs',
      },

      ownerFirstName: {
        type: 'string',
        title: 'Saimnieka pilnais vārds',
        minLength: 6,
      },
      previousOwners: {
        type: 'array',
        title: 'Iepriekšējie saimnieki',
        items: {
          type: 'object',
          properties: {
            ownerName: { type: 'string', title: 'Saimnieka pilnais vārds' },
            ownerContact: { type: 'string', title: 'Saimnieka kontaktinformācija' },
          },
        },
      },
    },
    required: ['animalName', 'animalType', 'birthDate'],
  },
  {
    $schema: 'http://json-schema.org/draft-07/schema#',
    type: 'object',
    properties: {
      animalName: {
        type: 'string',
        title: 'Dzīvnieka vārds',
        lx: {
          order: 1,
          columnSpan: 2,
        },
      },

      animalType: {
        type: 'string',
        title: 'Veids',
        default: 'other',
        lx: {
          variant: 'dropdown',

          items: [
            { id: 'cat', name: 'Kaķis' },
            { id: 'dog', name: 'Suns' },
            { id: 'horse', name: 'Zirgs' },
            { id: 'other', name: 'Cits' },
          ],
          order: 2,
        },
      },
      birthDate: {
        type: 'string',
        format: 'date',
        title: 'Dzimšanas datums',
        lx: {
          order: 3,
        },
      },
      weight: {
        type: 'number',
        exclusiveMinimum: 1,
        maximum: 100,
        title: 'Svars',
      },
      microchipNumber: {
        type: 'string',
        title: 'Mikročipa numurs',
        pattern: /^(\d{3})#(\d{2})$/,
        description: 'Numuram jābūt formātā: "nnn#nn", kur n ir jebkāds cipars',
      },
      favoriteFood: {
        type: 'string',
        title: 'Mīļākais ēdiens',
        lx: {
          kind: 'multiline',
          rowSpan: 2,
          order: 7,
        },
      },

      ownerFirstName: {
        type: 'string',
        title: 'Saimnieka vārds',
        minLength: 3,
        lx: {
          order: 5,
        },
      },
      ownerLastName: {
        type: 'string',
        title: 'Saimnieka uzvārds',
        lx: {
          order: 6,
        },
      },
    },
    required: ['animalName', 'animalType', 'birthDate'],
  },
]);
let backupSchemas = [];
const schema = computed(() => {
  if (example.value === 'exampleStandard') {
    return schemas.value[0];
  }
  if (example.value === 'exampleLx') {
    return schemas.value[1];
  }
  return {};
});

const model = ref();
const modal = ref();
const editedSchema = ref();
const validationMessages = ref([]);

function actionClicked(action) {
  if (action === 'reset') {
    model.value = {};
  }
  if (action === 'resetSchemas') {
    schemas.value = [...backupSchemas];
  }
  if (action === 'validate') {
    validationMessages.value = builder.value.validateModel();
    if (!validationMessages.value || validationMessages.value.length === 0) {
      notificationStore.pushSuccess('Formas lauki ir atbilstoši nosacījumiem');
    } else {
      notificationStore.pushError(
        'Formas lauku validācija nav izdevusies',
        `Konstatētas ${validationMessages.value.length} kļūdas`
      );
    }
  }
  if (action === 'edit') {
    editedSchema.value = JSON.stringify(schema.value, null, 2);
    modal.value.open();
  }
}

function editSchema() {
  if (example.value === 'exampleStandard') schemas.value[0] = JSON.parse(editedSchema.value);
  if (example.value === 'exampleLx') schemas.value[1] = JSON.parse(editedSchema.value);
  modal.value.close();
}

onMounted(() => {
  backupSchemas = [...schemas.value];
  viewStore.showBack();
});
</script>
<template>
  <div>
    <article class="lx-article">
      <p>
        Iepriekš apskatītās formas tika būvētas, izmantojot
        <a
          href="https://git.zzdats.lv/lx/ui/src/branch/master/src/components/forms/Row.vue"
          target="_blank"
          ref="noopener noreferrer"
        >LxRow</a
        >
        ar <RouterLink :to="{ name: 'sandbox' }">atbilstošām ievades komponentēm</RouterLink>.
      </p>
      <p>
        Formu būvētājs ir komponente, kas nodrošina veidu, kā būvēt formas deklaratīvā veidā.
        Būvētājs izmanto
        <a href="https://json-schema.org/" target="_blank" ref="noopener noreferrer">JSON schema</a>
        - standartu, kas nodrošina JSON struktūru apraksta notāciju. Derīgas JSON shēmas tiek
        izmantotas, lai deklaratīvā veidā aprakstītu formas modeli un tās izskatu, ģenerējot
        aprakstam atbilstošas datu ievades un attēlošanas komponetnes automātiski.
      </p>
      <LxList :items="list" @action-click="clicked" />
    </article>

    <article class="lx-article">
      <h2>Piemēri</h2>

      <div class="columns columns-3">
        <div><LxToggle v-model="readOnly">Skatīšanas režīms</LxToggle></div>
        <div>
          <LxContentSwitcher
            v-model="example"
            :items="[
              { id: 'exampleStandard', name: 'Standarta shēma' },
              { id: 'exampleLx', name: 'Shēma ar LX parametriem' },
            ]"
          />
        </div>
        <div></div>
      </div>
    </article>

    <LxForm
      :column-count="2"
      :action-definitions="actionDefinitionsVal"
      @button-click="actionClicked"
    >
      <template #header> FormBuilder būvēta forma </template>
      <LxFormBuilder ref="builder" v-model="model" :schema="schema" :readOnly="readOnly" />
    </LxForm>
    <LxModal
      ref="modal"
      button-primary-label="Saglabāt izmaiņas"
      :button-primary-visible="true"
      button-secondary-label="Atcelt izmaiņas"
      :button-secondary-visible="true"
      @primary-action="editSchema"
    >
      <LxTextArea v-model="editedSchema" :rows="20" />
    </LxModal>

    <article class="lx-article">
      <h3>Izmantotā shēma:</h3>
      <pre>
        <code>{{ schema }}</code>
      </pre>
    </article>
    <article class="lx-article">
      <h3>Izmantotais modelis:</h3>
      <pre>
        <code>{{ model }}</code>
      </pre>
    </article>
    <article class="lx-article">
      <h3>Validācijas paziņojumi:</h3>
      <pre>
        <code>{{ validationMessages }}</code>
      </pre>
    </article>
  </div>
</template>
