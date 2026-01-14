<script setup>
import { ref, onMounted, watch, computed, shallowRef } from 'vue';
import { useI18n } from 'vue-i18n';
import useViewStore from '@/stores/useViewStore';
import {
  LxDataGrid,
  LxButton,
  LxContentSwitcher,
  LxTextInput,
  LxFilters,
  LxRow,
  LxDateTimeRange,
  LxToggle,
  LxRatings,
} from '@wntr/lx-ui';
import useNotifyStore from '@/stores/useNotifyStore';
import { useRouter } from 'vue-router';

import gamesService from '@/services/gamesService';
import useErrors from '@/hooks/useErrors';
import useAuthStore from '@/stores/useAuthStore';

const authStore = useAuthStore();
const errors = useErrors();
const router = useRouter();

const translate = useI18n();
const viewStore = useViewStore();
const notificationStore = useNotifyStore();

const label = ref('Steam spēļu dati');
const description = ref('Dati no Steam spēļu veikala uz 2018. gadu');

const showHeader = ref(true);
const showToolbar = ref(true);
const showItemsCountSelector = ref(false);
const hasPaging = ref(true);
const hasSorting = ref(true);
const hasSelecting = ref(false);
const sortingIgnoreEmpty = ref(true);
const loading = ref(false);
const busy = ref(false);
const showActions = ref(true);

const rowCodeAttributeName = ref('id');

const skeletonRowCount = ref(10);
const integerSkeletonRowCount = computed(() => Number(skeletonRowCount.value));
const pageCurrent = ref(0);
const itemsPerPage = ref(10);

const actionDefinitions = ref([
  {
    id: 'open',
    name: 'Atvērt',
    icon: 'open',
    destructive: false,
  },
]);

const sampleColumnDefinition = shallowRef([
  {
    id: 'fullName',
    code: 'fullName',
    name: 'Nosaukums',
    attributeName: 'fullName',
    type: 'default',
    kind: 'clickable',
    size: '*',
  },
  {
    id: 'legal',
    code: 'legal',
    name: 'Juridiskā info',
    attributeName: 'legal',
    kind: 'extra',
    size: 'xl',
  },
  {
    id: 'popularity',
    code: 'popularity',
    name: 'Spēlē',
    attributeName: 'popularity',
    title: 'Spēlētāju skaits',
    type: 'number',
    size: 'm',
  },
  {
    id: 'recommendations',
    code: 'recommendations',
    name: 'Rekomendē',
    title: 'Rekomendāciju skaits',
    attributeName: 'recommendations',
    type: 'number',
    kind: 'extra',
    size: 's',
  },

  {
    id: 'price',
    code: 'price',
    name: 'Cena',
    title: 'Cena, USD',
    attributeName: 'price',
    type: 'decimal',
    kind: 'extra',
    size: 's',
  },
  {
    id: 'releaseDate',
    code: 'releaseDate',
    name: 'Datums',
    attributeName: 'releaseDate',
    title: 'Izlaišanas datums',
    type: 'date',
    kind: 'extra',
    size: 's',
  },
  {
    id: 'multiplayer',
    code: 'multiplayer',
    name: 'MP',
    attributeName: 'multiplayer',
    title: 'Daudzspēlētāju režīms',
    type: 'bool',
    kind: 'extra',
    size: 's',
  },
  {
    id: 'rating',
    code: 'rating',
    name: 'Novērtējums',
    attributeName: 'rating',
    title: 'Rating',
    type: 'rating',
    kind: 'secondary',
    size: 's',
  },
]);

const eventActionClick = shallowRef({});
const eventSelectPage = shallowRef({});

const sortingColumn = ref();
const sortingDirection = ref();
const eventItemsPerPage = shallowRef({});

const games = ref([]);
const itemsTotal = ref(0);

const filters = ref({
  page: pageCurrent,
  pageSize: itemsPerPage,
  sort: 'releaseDate',
  order: 'asc',
  filters: {},
});

const temp = ref();

function getUrl(row) {
  if (row.website && row.website !== 'None') return row.website;
  if (row.supportUrl && row.supportUrl !== 'None') return row.supportUrl;
  return null;
}

async function load() {
  loading.value = true;
  try {
    const resp = await gamesService.postList(filters.value);
    if (resp.status >= 200 && resp.status < 300) {
      console.dir(resp.data);
      if (resp.data) {
        games.value = resp.data.map((i) => ({
          id: i.id,
          name: i.name,
          fullName: i.fullName,
          legal: i.legalNotice,
          popularity: i.steamSpyOwners ? i.steamSpyOwners : null,
          recommendations: i.recommendationCount ? i.recommendationCount : null,
          multiplayer: i.categoryMultiplayer,
          rating: i.metacriticScore ? Math.floor(i.metacriticScore / 20) : null,
          releaseDate: i.releaseDate,
          price: i.isFree ? null : i.priceFinal,
          url: getUrl(i),
        }));
        itemsTotal.value = Number(resp.headers['x-total-count']);
      } else {
        games.value = [];
      }
    } else {
      return;
    }
  } catch (err) {
    errors.pushWithNotification(err, router);
  } finally {
    loading.value = false;
  }
}
function actionClicked(actionName, rowCode, actionAdditionalParameter) {
  const val = games.value.find((i) => i.id === rowCode)?.url;
  if (val) {
    window.open(val, '_blank');
    return;
  }
  notificationStore.pushWarning('Spēlei nav nodefinēta saite!');
}

function selectPage(pageNum) {
  eventSelectPage.value = { pageNum };
  pageCurrent.value = pageNum;
  load();
}

function sortingChanged(sortInfo) {
  sortingColumn.value = sortInfo.columnCode;
  sortingDirection.value = sortInfo.direction;
}
function itemsPerPageChanged(value) {
  eventItemsPerPage.value = value;
  itemsPerPage.value = value;
  pageCurrent.value = 0;
  load();
}
const actionAdditionalParameter = ref('Darbības papildu parametrs');
watch(
  () => showActions.value,
  () => {
    if (showActions.value === false) {
      actionDefinitions.value = [];
    } else {
      actionDefinitions.value = [
        {
          id: 'open',
          name: 'Open item',
          icon: 'open',
          enableByAttributeName: 'wasEren',
          destructive: false,
        },
        {
          id: 'edit',
          name: 'Edit item',
          icon: 'edit',
          enableByAttributeName: 'wasEren',
          destructive: false,
        },
        {
          id: 'delete',
          name: 'Delete item',
          icon: 'delete',
          enableByAttributeName: 'wasEren',
          destructive: true,
        },
      ];
    }
  }
);
const texts = ref({
  itemsSelected: {
    singular: 'Izvēlēts',
    plural: 'Izvēlēti',
    endingWith234: 'Izvēlēti',
    endingWith1: 'Izvēlēts',
  },
  valueYes: 'Jā',
  valueNo: 'Nē',
  items: {
    singular: 'ieraksts',
    plural: 'ieraksti',
    endingWith234: 'ieraksti',
    endingWith1: 'ieraksts',
  },
  ofItems: {
    label: 'Ieraksti',
    singular: 'ieraksta',
    plural: 'ierakstiem',
    endingWith234: 'ierakstiem',
    endingWith1: 'ieraksta',
  },
  selected: {
    singular: 'Izvēlēts',
    plural: 'Izvēlēti',
    endingWith234: 'Izvēlēti',
    endingWith1: 'Izvēlēts',
  },
  of: 'no',
  nextPage: 'Nākamā lapa',
  previousPage: 'Iepriekšējā lapa',
  clear: 'Attīrīt izvēles',
  itemsPerPage: 'Ierakstu skaits vienā lapā:',
  itemsPerPageLabel: 'ieraksti lapā',
  selectAllRows: 'Izvēlēties visu',
  noItems: 'Nav neviena ieraksta',
  noItemsDescription: 'Precizējiet meklēšanas kritērijus un mēģiniet vēlreiz!',
});

const routerValue = ref('');
function contentSwitcherClick() {
  if (routerValue.value === 'sandbox') {
    router.push({ name: 'dataGrid' });
  }
}
const itemsNav = [
  { id: 'sandbox', name: 'Smilškaste' },
  { id: '', name: 'Piemērs' },
];

const showAllColumns = shallowRef(false);

onMounted(() => {
  load();
  viewStore.goBack = true;
  viewStore.description = translate.t('pages.dataGrid.description');
});
const emptyStateActionDefinitions = ref([
  {
    id: 'add',
    name: 'Pievienot saraksta elementu',
    icon: 'add-item',
    destructive: false,
  },
]);
function emptyStateActionClicked(actionName) {
  notificationStore.pushSuccess(`Empty state action was pressed: ${actionName}`);
}
const isFilterUsed = computed(
  () => !Object.values(filters.value.filters)?.every((x) => x === null || x === '' || x === '0.00')
);

function filterClear() {
  filters.value.filters = {};
  load();
}
</script>
<template>
  <div class="lx-form">
    <LxContentSwitcher :items="itemsNav" v-model="routerValue" @click="contentSwitcherClick()" />
    <LxFilters
      id="billFilters"
      :disabled="loading"
      :usesFilters="isFilterUsed"
      :columnCount="3"
      @filter="load"
      @resetFilters="filterClear"
      :expanded="false"
      kind="form"
    >
      <LxRow :columnSpan="2" label="Nosaukums">
        <LxTextInput v-model="filters.filters.name" @keyup.enter="load"></LxTextInput>
      </LxRow>
      <LxRow label="Izlaišanas datums">
        <LxDateTimeRange
          ref="refDf"
          v-model:start-date="filters.filters.releaseDateFrom"
          v-model:end-date="filters.filters.releaseDateTill"
          max-date="2017-12-31"
          min-date="1997-01-01"
          @keyup.enter="load"
        />
      </LxRow>
      <LxRow label="Detalizēts apraksts" :columnSpan="2">
        <LxTextInput
          v-model="filters.filters.detailedDescription"
          @keyup.enter="load"
        ></LxTextInput>
      </LxRow>
      <LxRow label="Daudzspēlētāju režīms">
        <LxToggle v-model="filters.filters.categoryMultiplayer" @keyup.enter="load"></LxToggle>
      </LxRow>
      <LxRow label="Cena no">
        <LxTextInput
          v-model="filters.filters.priceFrom"
          mask="currency"
          @keyup.enter="load"
        ></LxTextInput>
      </LxRow>
      <LxRow label="Cena līdz">
        <LxTextInput
          v-model="filters.filters.priceTill"
          mask="currency"
          @keyup.enter="load"
        ></LxTextInput>
      </LxRow>
      <LxRow label="Bezmaksas">
        <LxToggle v-model="filters.filters.isFree" @keyup.enter="load"></LxToggle>
      </LxRow>
      <LxRow label="Rekomendāciju skaits no">
        <LxTextInput
          v-model="filters.filters.recommendationCountFrom"
          mask="integer"
          @keyup.enter="load"
        ></LxTextInput>
      </LxRow>
      <LxRow label="Rekomendāciju skaits līdz">
        <LxTextInput
          v-model="filters.filters.recommendationCountTill"
          mask="integer"
          @keyup.enter="load"
        ></LxTextInput>
      </LxRow>
      <LxRow label="Novērtējums">
        <LxRatings v-model="filters.filters.metacriticScore" @keyup.enter="load"></LxRatings>
      </LxRow>
      <LxRow label="Spēlētāju skaits no">
        <LxTextInput
          v-model="filters.filters.steamSpyOwnersFrom"
          mask="integer"
          @keyup.enter="load"
        ></LxTextInput>
      </LxRow>
      <LxRow label="Spēlētāju skaits līdz">
        <LxTextInput
          v-model="filters.filters.steamSpyOwnersTill"
          mask="integer"
          @keyup.enter="load"
        ></LxTextInput>
      </LxRow>
    </LxFilters>

    <lx-data-grid
      id="gridAPI"
      :items="games"
      :label="label"
      :description="description"
      :column-definitions="sampleColumnDefinition"
      :id-attribute="rowCodeAttributeName"
      :action-definitions="actionDefinitions"
      :action-additional-parameter="actionAdditionalParameter"
      :loading="loading"
      :busy="busy"
      :skeleton-row-count="itemsPerPage"
      :show-header="showHeader"
      :show-toolbar="showToolbar"
      :show-all-columns="showAllColumns"
      :scrollable="showAllColumns"
      :show-items-count-selector="showItemsCountSelector"
      :has-paging="hasPaging"
      :has-sorting="false"
      :has-selecting="hasSelecting"
      :sorting-ignore-empty="sortingIgnoreEmpty"
      :page-current="pageCurrent"
      :items-per-page="itemsPerPage"
      :items-total="itemsTotal"
      :texts="texts"
      :showItemsCountSelector="true"
      @action-click="actionClicked"
      @select-page="selectPage"
      @sorting-changed="sortingChanged"
      @items-per-page-changed="itemsPerPageChanged"
    >
      <template #toolbar>
        <div class="lx-justified-toolbar">
          <LxButton label="Atjaunot datus" icon="refresh" @click="load()"></LxButton>
          <LxButton
            label="Parādīt vairāk kolonnu"
            kind="ghost"
            variant="icon-only"
            :icon="showAllColumns ? 'less-grid' : 'more-grid'"
            @click="showAllColumns = !showAllColumns"
          ></LxButton>
        </div>
      </template>
    </lx-data-grid>
  </div>
</template>
