<script setup>
import { ref, shallowRef, onMounted, watch, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import useViewStore from '@/stores/useViewStore';
import { LxIcon, LxList, LxContentSwitcher, lxDateUtils } from '@wntr/lx-ui';

import { useRouter } from 'vue-router';
import useNotifyStore from '@/stores/useNotifyStore';
import gamesService from '@/services/gamesService';
import useErrors from '@/hooks/useErrors';

const errors = useErrors();

const router = useRouter();

const translate = useI18n();
const viewStore = useViewStore();
const notificationStore = useNotifyStore();

const hasSearch = ref(true);
const busyValue = shallowRef(false);
const loading = ref(false);

const icon = ref('open');
const itemIdAttributeName = ref('id');
const itemPrimaryAttributeName = ref('name');
const itemSecondaryAttributeName = ref('description');
const itemHrefAttributeName = ref('href');
const itemGroupAttributeName = ref('group');
const itemClickableAttributeName = ref('clickable');
const itemIconAttributeName = ref('icon');
const itemIconSetAttributeName = ref('cds');
const itemCategoryAttributeName = ref('category');

const showActions = ref(false);
const hideFilteredItemsValue = shallowRef(false);
const actionDefinitions = ref([]);

const searchString = ref('');

const texts = ref({
  clear: 'Notīrīt',
  placeholder: 'Ievadiet nosaukuma vai apraksta daļu, lai sameklētu ierakstus',
  notFoundSearch: 'Nav atrasts: ',
  noItems: 'Nav neviena ieraksta',
  noItemsDescription: 'Precizējiet meklēšanas kritērijus un mēģiniet vēlreiz!',
  loadMore: 'Ielādēt vēl',
  search: 'Meklēt',
});
const eventActionClick = shallowRef({});

function formatPopularity(val) {
  if (!val) return null;
  if (val < 1000) return null;
  if (val < 1000000) return `${Math.round(val / 1000)}K`;
  if (val < 1000000000) return `${Math.round(val / 1000000)}M`;
}
const games = ref([]);

function actionClicked(actionName, itemId) {
  if (actionName === 'click') {
    const val = games.value.find((i) => i.id === itemId)?.url;
    if (val) {
      window.open(val, '_blank');
      return;
    }
    notificationStore.pushWarning('Spēlei nav nodefinēta saite!');
  }
}

watch(
  () => showActions.value,
  () => {
    if (!showActions.value) {
      actionDefinitions.value = [];
    } else {
      actionDefinitions.value = [
        {
          id: 'open',
          name: 'Open item',
          icon: 'open',
          enableByAttributeName: 'canEdit',
          destructive: false,
        },
        {
          id: 'edit',
          name: 'Edit item',
          icon: 'edit',
          enableByAttributeName: 'canEdit',
          destructive: false,
        },
        {
          id: 'delete',
          name: 'Delete item',
          icon: 'delete',
          enableByAttributeName: 'canEdit',
          destructive: true,
        },
      ];
    }
  }
);

const pageCurrent = ref(0);
const dataOnPage = ref(20);
const itemsTotal = ref(0);
const filters = ref({
  page: pageCurrent,
  pageSize: dataOnPage,
  sort: 'releaseDate',
  order: 'asc',
  filters: {},
});

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
      if (resp.data) {
        games.value = resp.data.map((i) => ({
          id: i.id,
          name: i.fullName,
          description: i.shortDescription,
          popularity: formatPopularity(i.steamSpyOwners),
          popularityFull: i.steamSpyOwners
            ? new Intl.NumberFormat('lv-LV').format(i.steamSpyOwners)
            : null,
          multiplayer: i.categoryMultiplayer,
          image: i.headerImage,
          rating: i.metacriticScore,
          price: i.isFree ? null : i.priceFinal,
          year: i.releaseDate ? i.releaseDate.split('-')[0] : null,
          releaseDate: i.releaseDate,
          url: getUrl(i),
          clickable: getUrl(i),
        }));
      } else {
        games.value = [];
      }
      itemsTotal.value = Number(resp.headers['x-total-count']);
    } else {
      return;
    }
  } catch (err) {
    errors.pushWithNotification(err, router);
  } finally {
    loading.value = false;
  }
}

function loadMoreFunc() {
  dataOnPage.value += 20;
  load();
}

const canLoadMore = computed(() => itemsTotal.value > games.value.length);

const routerValue = ref('');
function contentSwitcherClick() {
  if (routerValue.value === 'sandbox') {
    router.push({ name: 'list' });
  }
  if (routerValue.value === 'guidelines') {
    router.push({ name: 'listGuidelines' });
  }
}
const itemsNav = [
  { id: 'sandbox', name: 'Smilškaste' },
  { id: '', name: 'Piemērs' },
  { id: 'guidelines', name: 'Vadlīnijas' },
];

const emptyStateActionDefinitions = ref([
  {
    id: 'add',
    name: 'Pievienot elementu',
    icon: 'add-item',
    destructive: false,
  },
]);
function emptyStateActionClicked(actionName) {
  notificationStore.pushSuccess(`Empty state action was pressed: ${actionName}`);
}

onMounted(() => {
  load();
  viewStore.goBack = true;
  viewStore.description = translate.t('pages.list.description');
});

function searched(searchRequest) {
  filters.value.filters.name = searchRequest;
  load();
}
</script>
<template>
  <div>
    <LxContentSwitcher :items="itemsNav" v-model="routerValue" @click="contentSwitcherClick()" />

    <div class="lx-divider"></div>
    <LxList
      id="id"
      :items="games"
      :has-search="hasSearch"
      :icon="icon"
      id-attribute="id"
      :primary-attribute="itemPrimaryAttributeName"
      :secondary-attribute="itemSecondaryAttributeName"
      :href-attribute-="itemHrefAttributeName"
      :group-attribute="itemGroupAttributeName"
      :clickable-attribute="itemClickableAttributeName"
      :icon-attribute="itemIconAttributeName"
      :icon-set-attribute="itemIconSetAttributeName"
      :category-attribute="itemCategoryAttributeName"
      listType="2"
      :texts="texts"
      :hide-filtered-items="hideFilteredItemsValue"
      searchSide="server"
      @action-click="actionClicked"
      v-model:search-string="searchString"
      :busy="busyValue"
      :loading="loading"
      :showLoadMore="canLoadMore"
      @load-more="loadMoreFunc"
      @searched="searched"
    >
      <template
        #customItem="{
          image,
          name,
          popularity,
          popularityFull,
          rating,
          price,
          year,
          releaseDate,
          multiplayer,
        }"
      >
        <div class="game">
          <div class="img" :style="image ? `background-image: url(${image});` : null"></div>
          <p class="lx-primary">{{ name }}</p>
          <div class="tags">
            <div
              class="price"
              :class="[{ empty: !price }]"
              :title="price ? `Cena: ${price} \$` : null"
            >
              <template v-if="price">
                <span>$ {{ price }}</span>
              </template>
              <template v-else><span class="none">—</span></template>
            </div>
            <div
              class="rating"
              :class="[{ empty: !rating }]"
              :title="rating ? `Meta critic score: ${rating}` : null"
            >
              <template v-if="rating">
                <LxIcon value="star" /><span>{{ rating }}</span>
              </template>
              <template v-else><span class="none">—</span></template>
            </div>
            <div
              class="popularity"
              :class="[{ empty: !popularity }]"
              :title="popularityFull ? `Lejupielāžu skaits: ${popularityFull}` : null"
            >
              <template v-if="popularity">
                <LxIcon :value="multiplayer ? 'users' : 'person'" /><span>{{ popularity }}</span>
              </template>
              <template v-else><span class="none">—</span></template>
            </div>
            <div
              class="year"
              :class="[{ empty: !year }]"
              :title="year ? `Izlaišanas datums: ${lxDateUtils.formatDate(releaseDate)}` : null"
            >
              <template v-if="year">
                <LxIcon value="calendar" /><span>{{ year }}</span>
              </template>
              <template v-else><span class="none">—</span></template>
            </div>
          </div>
        </div>
      </template>
    </LxList>
  </div>
</template>
