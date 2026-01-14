<script setup>
import { onMounted, ref } from 'vue';
import {
  LxForm,
  LxRow,
  LxSection,
  LxTextInput,
  LxTextArea,
  LxDropDown,
  LxDateTimePicker,
  LxList,
  LxContentSwitcher,
  lxDateUtils,
} from '@wntr/lx-ui';

import { useDemoStore } from '@/stores/useDemoStore';
import useViewStore from '@/stores/useViewStore';

const demoStore = useDemoStore();
const viewStore = useViewStore();

const testData = ref(demoStore.testData);
const videoGames = ref(demoStore.videoGamesComputed);

const readOnly = ref(false);

const index = ref([
  { id: 'default', name: 'Pamatinformācija' },
  { id: 'biography', name: 'Biogrāfija' },
  { id: 'achievements', name: 'Sasniegumi' },
]);

const actionDefinitionsVal = [
  { id: 'save', icon: 'save', name: 'Saglabāt', kind: 'primary' },
  { id: 'cancel', icon: 'undo', name: 'Atcelt', kind: 'secondary' },
];
const indexType = ref('none');

onMounted(() => {
  viewStore.showBack();
});
</script>
<template>
  <div>
    <article class="lx-article">
      <p>
        Atsevišķajos gadījumos, ja forma ir pietiekoši liela, tās laukus var sagrupēt loģiskās
        sadaļās (<em>sections</em>).
      </p>
      <p>
        Katra sadaļa kalpo kā atsevišķa forma, kurai ir visi tie paši atribūti, par kurām runājām
        iepriekšējās sadaļās: katrai sadaļai var būt savs nosaukums un kolonnu skaits. Kājene un
        galvene ir tikai visai formai kopumā, bez iespējas definēt tās katrai sadaļai atsevišķi.
      </p>
    </article>
    <LxForm :column-count="2" :action-definitions="actionDefinitionsVal">
      <template #header> Personas kartīte #2123 </template>

      <LxRow label="Vārds">
        <LxTextInput v-model="testData.firstName" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Uzvārds">
        <LxTextInput v-model="testData.lastName" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Mīļākā videospēle">
        <LxDropDown
          v-model="testData.favGame"
          :items="videoGames"
          kind="default"
          :read-only="readOnly"
        />
      </LxRow>

      <template #sections>
        <LxSection id="biography" label="Biogrāfija" :column-count="2">
          <LxRow label="Dzimšanas datums">
            <LxDateTimePicker v-model="testData.birthDate" :read-only="readOnly" />
          </LxRow>
          <LxRow label="Apraksts" :row-span="2">
            <LxTextArea
              v-model="testData.description"
              kind="default"
              :read-only="readOnly"
              :rows="5"
            />
          </LxRow>
          <LxRow label="Dzimšanas vieta">
            <LxTextInput v-model="testData.birthCountry" :read-only="readOnly" />
          </LxRow>
        </LxSection>
        <LxSection id="achievements" label="Sasniegumi" :column-count="2">
          <LxRow :column-span="2" label="Čempionāti">
            <LxList
              list-type="2"
              :items="testData.achievements"
              id-attribute="id"
              primary-attribute="name"
            >
              <template #customItem="{ name, place, tier, date }">
                <div class="achievement" :title="`${place}. vieta`">
                  <div
                    class="place"
                    :class="[{ 'place-gold': place === 1 }, { 'place-silver': place === 2 }]"
                  >
                    {{ place }}
                  </div>
                  <div>
                    <p class="lx-primary">{{ name }}</p>
                    <p class="lx-secondary">{{ lxDateUtils.formatDate(date) }}, {{ tier }}</p>
                  </div>
                </div>
              </template>
            </LxList>
          </LxRow>
        </LxSection>
      </template>
    </LxForm>
    <article class="lx-article">
      <p>Ja formai nav definētas sadaļas, tās lauki pēc noklusējuma arī atrodas vienā sadaļā.</p>
      <h2>Satura rādītājs</h2>
      <p>Ja forma paliek ļoti gara, tai var iespējot t.s. "satura rādītāju":</p>
    </article>
    <LxForm :column-count="2" :action-definitions="actionDefinitionsVal" :index="index">
      <template #header> Personas kartīte #2123 </template>

      <LxRow label="Vārds">
        <LxTextInput v-model="testData.firstName" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Uzvārds">
        <LxTextInput v-model="testData.lastName" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Mīļākā videospēle">
        <LxDropDown
          v-model="testData.favGame"
          :items="videoGames"
          kind="default"
          :read-only="readOnly"
        />
      </LxRow>

      <template #sections>
        <LxSection id="biography" label="Biogrāfija" :column-count="2">
          <LxRow label="Dzimšanas datums">
            <LxDateTimePicker v-model="testData.birthDate" :read-only="readOnly" />
          </LxRow>
          <LxRow label="Apraksts" :row-span="2">
            <LxTextArea
              v-model="testData.description"
              kind="default"
              :read-only="readOnly"
              :rows="5"
            />
          </LxRow>
          <LxRow label="Dzimšanas vieta">
            <LxTextInput v-model="testData.birthCountry" :read-only="readOnly" />
          </LxRow>
        </LxSection>
        <LxSection id="achievements" label="Sasniegumi" :column-count="2">
          <LxRow :column-span="2" label="Čempionāti">
            <LxList
              list-type="2"
              :items="testData.achievements"
              id-attribute="id"
              primary-attribute="name"
            >
              <template #customItem="{ name, place, tier, date }">
                <div class="achievement" :title="`${place}. vieta`">
                  <div
                    class="place"
                    :class="[{ 'place-gold': place === 1 }, { 'place-silver': place === 2 }]"
                  >
                    {{ place }}
                  </div>
                  <div>
                    <p class="lx-primary">{{ name }}</p>
                    <p class="lx-secondary">{{ lxDateUtils.formatDate(date) }}, {{ tier }}</p>
                  </div>
                </div>
              </template>
            </LxList>
          </LxRow>
        </LxSection>
      </template>
    </LxForm>
    <article class="lx-article">
      <h2>Formas cilnes</h2>
      <p>
        Atsevišķajos gadījumos (kuriem jābūt pietiekoši retiem), forma var tikt izkārtota cilnēs.
      </p>
      <p>
        Ciļņu attēlošanai un darbībai tiek izmantots formas sadaļu mehānisms - katras cilnes saturs
        ir atsevišķa formas sadaļa.
      </p>
      <p class="lx-warning">
        Ciļnu režīms ir pēdējais, kas jāmēģina - atceramies, ka klikšķis ir "dārgāks" par
        skrullēšanu. "Satura rādītāja" režīms funkcionē tāpat, kā "Ciļņu režīms", bet piedāvā labāku
        lietojamību un papildus funckionalitāti, toties varētu izskatīties neierastāks lietotājam.
        Tas ir normāli, kaut arī prasa vairāk piepūļu klienta komunikācijā. Tāpēc, Ja nepieciešama
        navigācija pa formas sadaļām, tad "satura rādītāja" režīms ir pirmais
        <em>go to</em> risinājums un tikai tad, kad kaut kāda iemesla dēļ tas neder - izskatām ciļņu
        rēžīmu.
      </p>
      <div class="columns columns-3">
        <div></div>
        <div>
          <LxContentSwitcher
            v-model="indexType"
            id-attribute="id"
            name-attribute="name"
            :items="[
              { id: 'none', name: 'Bez grupēšanas' },
              { id: 'default', name: 'Satura rādītājs' },
              { id: 'expanders', name: 'Sakļaujamie paneļi' },
              { id: 'tabs', name: 'Cilnes' },
            ]"
          />
        </div>
        <div></div>
      </div>
    </article>

    <LxForm
      :column-count="2"
      :action-definitions="actionDefinitionsVal"
      :index="indexType !== 'none' ? index : []"
      :index-type="indexType !== 'none' ? indexType : 'default'"
    >
      <template #header> Personas kartīte #2123 </template>

      <LxRow label="Vārds">
        <LxTextInput v-model="testData.firstName" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Uzvārds">
        <LxTextInput v-model="testData.lastName" :read-only="readOnly" />
      </LxRow>
      <LxRow label="Mīļākā videospēle">
        <LxDropDown
          v-model="testData.favGame"
          :items="videoGames"
          kind="default"
          :read-only="readOnly"
        />
      </LxRow>

      <template #sections>
        <LxSection id="biography" label="Biogrāfija" :column-count="2">
          <LxRow label="Dzimšanas datums">
            <LxDateTimePicker v-model="testData.birthDate" :read-only="readOnly" />
          </LxRow>
          <LxRow label="Apraksts" :row-span="2">
            <LxTextArea
              v-model="testData.description"
              kind="default"
              :read-only="readOnly"
              :rows="5"
            />
          </LxRow>
          <LxRow label="Dzimšanas vieta">
            <LxTextInput v-model="testData.birthCountry" :read-only="readOnly" />
          </LxRow>
        </LxSection>
        <LxSection id="achievements" label="Sasniegumi" :column-count="2">
          <LxRow :column-span="2" label="Čempionāti">
            <LxList
              list-type="2"
              :items="testData.achievements"
              id-attribute="id"
              primary-attribute="name"
            >
              <template #customItem="{ name, place, tier, date }">
                <div class="achievement" :title="`${place}. vieta`">
                  <div
                    class="place"
                    :class="[{ 'place-gold': place === 1 }, { 'place-silver': place === 2 }]"
                  >
                    {{ place }}
                  </div>
                  <div>
                    <p class="lx-primary">{{ name }}</p>
                    <p class="lx-secondary">{{ lxDateUtils.formatDate(date) }}, {{ tier }}</p>
                  </div>
                </div>
              </template>
            </LxList>
          </LxRow>
        </LxSection>
      </template>
    </LxForm>
    <div style="height: 100vh"></div>
  </div>
</template>
