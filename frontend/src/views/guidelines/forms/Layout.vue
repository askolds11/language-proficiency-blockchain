<script setup>
import { onMounted, ref } from 'vue';
import {
  LxForm,
  LxRow,
  LxTextInput,
  LxTextArea,
  LxDropDown,
  LxDateTimePicker,
  LxToggle,
  LxContentSwitcher,
} from '@wntr/lx-ui';

import { useDemoStore } from '@/stores/useDemoStore';
import useViewStore from '@/stores/useViewStore';

const demoStore = useDemoStore();
const viewStore = useViewStore();

const testData = ref(demoStore.testData);
const videoGames = ref(demoStore.videoGamesComputed);

const readOnly = ref(false);

const showHeader = ref(true);
const showPreHeader = ref(false);
const showPreHeaderTooltip = ref(false);
const showPostHeader = ref(false);
const showPostHeaderTooltip = ref(false);

const today = ref(new Date());
const creationDate = ref(new Date(2023, 7, 29, 9, 41));

const columnCount = ref(1);

const gridLines = ref(false);

const actionDefinitionsVal = [
  { id: 'save', icon: 'save', name: 'Saglabāt', kind: 'primary' },
  { id: 'cancel', icon: 'undo', name: 'Atcelt', kind: 'secondary' },
];

onMounted(() => {
  viewStore.showBack();
});
</script>
<template>
  <div :class="[{ 'show-gridlines': gridLines }]">
    <article class="lx-article">
      <p>
        Lauki formā izkārtojas tabulas veidā, katrā tabulas šūnā - viens formas lauks. Katram laukam
        jābūt savai birkai.
      </p>
      <p>
        Formas izkārtojuma tabula pēc noklusējuma izvieto visus laukus vienā kolonnā, secīgi, vienu
        zem otra, bet atsevišķajā gadījumā laukus var izkārtot arī vairākās kolonnās:
      </p>
      <p class="lx-warning">
        Izkārtojumi vairāk nekā 4 kolonnās ir iespējami, bet noteikti nav rekomendēti. Iebūvētie
        formas mehānismi paļaujās, ka vairāk par 4 kolonnām netiks izmantotas, kas nozīmē, ka šādos
        gadījumos uzvedība ir jāprogrammē atsevišķi.
      </p>
      <div class="columns columns-3">
        <LxContentSwitcher
          id-attribute="id"
          name-attribute="name"
          :items="[
            { id: 1, name: '1' },
            { id: 2, name: '2' },
            { id: 3, name: '3' },
            { id: 4, name: '4' },
          ]"
          v-model="columnCount"
        />
        <div></div>
        <LxToggle v-model="gridLines">Iekrāsot formas lauku šūnu robežas</LxToggle>
      </div>
    </article>
    <LxForm
      :show-header="true"
      :show-footer="true"
      :column-count="columnCount"
      :action-definitions="actionDefinitionsVal"
    >
      <template #header> Personas kartīte #2123 </template>
      <LxRow label="Vārds">
        <LxTextInput v-model="testData.firstName" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Uzvārds">
        <LxTextInput v-model="testData.lastName" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Apraksts">
        <LxTextArea v-model="testData.description"> </LxTextArea>
      </LxRow>
      <LxRow label="Tituls">
        <LxTextInput v-model="testData.title" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Mīļākā videospēle">
        <LxDropDown
          v-model="testData.favGame"
          :items="videoGames"
          kind="default"
          :read-only="readOnly"
        />
      </LxRow>
      <LxRow label="Dzimšanas datums">
        <LxDateTimePicker v-model="testData.birthDate" :read-only="readOnly" />
      </LxRow>
    </LxForm>
    <article class="lx-article">
      <h2>Sarežģītais tabulas režģis</h2>
      <p>Tabulas struktūra ļauj izkārtot laukus praktiski jebkādā veidā:</p>
    </article>

    <LxForm
      :show-header="true"
      :show-footer="true"
      :column-count="4"
      :action-definitions="actionDefinitionsVal"
    >
      <template #header> Personas kartīte #2123 </template>
      <LxRow label="Vārds">
        <LxTextInput v-model="testData.firstName" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Uzvārds">
        <LxTextInput v-model="testData.lastName" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Apraksts" :column-span="2" :row-span="2">
        <LxTextArea v-model="testData.description" :rows="4"> </LxTextArea>
      </LxRow>
      <LxRow label="Tituls" :column-span="2">
        <LxTextInput v-model="testData.title" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Mīļākā videospēle" :column-span="2">
        <LxDropDown
          v-model="testData.favGame"
          :items="videoGames"
          kind="default"
          :read-only="readOnly"
        />
      </LxRow>
      <LxRow label="Dzimšanas datums">
        <LxDateTimePicker v-model="testData.birthDate" :read-only="readOnly" />
      </LxRow>
    </LxForm>
    <article class="lx-article">
      <p class="lx-warning">
        Jāņem vērā, ka neizmantojot noklusēto formas izkārtojumu, ir atsevišķi jāpievērš uzmanība
        tam, kā formas lauki pārkārtojas uz mazākiem ekrāniem.
      </p>
    </article>
  </div>
</template>
