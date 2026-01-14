<script setup lang="ts">
import { shallowRef, ref } from 'vue';
import { LxContentSwitcher, LxValuePicker, LxForm, LxRow } from '@wntr/lx-ui';
import { useRouter } from 'vue-router';
import { useDemoStore } from '@/stores/useDemoStore';

const demoStore = useDemoStore();
const router = useRouter();
const routerValue = shallowRef('guidelines');
const value = shallowRef();
const values = ref([]);
const selectedVariant = shallowRef('default');
const variants = shallowRef([
  { id: 'default', name: 'Noklusētais variants' },
  { id: 'dropdown', name: 'Izkrītoša izvēlne' },
  { id: 'tags', name: 'Lentas izvēlne' },
]);

const items = shallowRef(
  demoStore.videoGames
    .map((item) => ({ ...item, description: null }))
    .sort((a, b) => a.name.localeCompare(b.name))
);
const itemsShort = shallowRef(
  demoStore.videoGames.sort((a, b) => a.name.localeCompare(b.name)).slice(0, 3)
);
function contentSwitcherClick() {
  if (routerValue.value === 'sandbox') {
    router.push({ name: 'valuePicker' });
  }
}
</script>
<template>
  <LxContentSwitcher
    :items="[
      { id: 'sandbox', name: 'Smilškaste' },
      { id: 'guidelines', name: 'Vadlīnijas' },
    ]"
    v-model="routerValue"
    @click="contentSwitcherClick"
  />
  <article class="lx-article">
    <h2>Pamati</h2>
    <p>
      <em>LxValuePicker</em> (Vērtību izvēlne) tiek izmantots, lai
      <strong>izvēlētos vienu vai vairākas vērtības no predefinēta saraksta</strong>. Tas pats
      izvēlās atbilstošo komponenšu izvēli atbilstoši iestatījumiem tā, lai programmētājam nebūtu
      jādomā, kura komponente ir vislabākā priekš katras konkrētās situācijas.
    </p>
    <p class="lx-warning">
      Lietotājam nevajadzētu piespiest izvēlēties no vairāk kā 15 vērtībām vienā vērtību izvēlnē. Ja
      izvēle ir no lielāka izvēļu skaita, jāmeklē citi varianti!
    </p>
    <p>
      Ja jāizvēlās viena vērtība no <strong>nedeterminēta</strong> saraksta (saraksta ar ļoti lielu
      ierakstu skaitu vai kad visu izvēļu sarakstu dabūt klienta lietotnē nav lietderīgi) vai
      saraksta no vairāk kā 15 izvēlēm, izmantojam
      <RouterLink :to="{ name: 'autoComplete' }">LxAutoComplete</RouterLink>.
    </p>

    <h2>Varianti</h2>
    <h3>Noklusētais variants (<em>"default"</em>)</h3>
    <p>
      Noklusēti, vērtību izvēlne attēlo vērtības kā sarakstu ar
      <RouterLink :to="{ name: 'radioButton' }">radio pogām</RouterLink> vai
      <RouterLink :to="{ name: 'radioButton' }">izvēles rūtiņām</RouterLink>.
    </p>
    <p>
      Šis ir variants, ko izmantojam lielākajā daļā no gadījumiem. Tas parāda visas izvēles uzreiz,
      tādejādi atvieglojot izvēli.
    </p>
    <p class="lx-warning">
      Atceramies, ka situācija, kad radiopogu grupā nav izvēlēta neviena vērtība, nedrīkst pastāvēt!
      Tas nozīmē, ka ielādes brīdī jaizvēlās viena no vērtībām vai, kad šādu izvēli lietotāja vietā
      veikt ir bīstami, jāpievieno izvēle "Nav izvēlēts".
    </p>
    <LxForm :show-header="false" :show-footer="false" :column-count="2">
      <LxRow label="Mīļākā spēle">
        <LxValuePicker :items="items" v-model="value" :nullable="true" />
      </LxRow>
      <LxRow label="Mīļākās spēles">
        <LxValuePicker :items="items" v-model="values" :nullable="true" kind="multiple" />
      </LxRow>
    </LxForm>
  </article>
  <article class="lx-article">
    <h3>Izkrītoša izvēlne (<em>"dropdown"</em>)</h3>
    <p>
      Gadījumos, kad kaut kādu iemeslu dēļ attēlot visas izvēles nevar (piemēram, telpas taupīšanas
      nolūkos vai formas uzbūves dēļ), izvēles var attēlot izkrītošā izvēlnē.
    </p>
    <LxForm :show-header="false" :show-footer="false" :column-count="2">
      <LxRow label="Mīļākā spēle">
        <LxValuePicker :items="items" v-model="value" :nullable="true" variant="dropdown" />
      </LxRow>
      <LxRow label="Mīļākās spēles">
        <LxValuePicker
          :items="items"
          v-model="values"
          :nullable="true"
          kind="multiple"
          variant="dropdown"
        />
      </LxRow>
    </LxForm>
  </article>
  <article class="lx-article">
    <h3>Lentas izvēlne (<em>"tags"</em>)</h3>
    <p>
      Ja izvēļu skenēšana (ātra atrašana) nav prioritāte, var izmantot variantu ar lentas izvēlni -
      tas izkārto izvēles horizontālajā sarakstā klikšķināmo birku veidā.
    </p>
    <LxForm :show-header="false" :show-footer="false" :column-count="2">
      <LxRow label="Mīļākā spēle">
        <LxValuePicker :items="items" v-model="value" :nullable="true" variant="tags" />
      </LxRow>
      <LxRow label="Mīļākās spēles">
        <LxValuePicker
          :items="items"
          v-model="values"
          :nullable="true"
          kind="multiple"
          variant="tags"
        />
      </LxRow>
    </LxForm>
  </article>
  <article class="lx-article">
    <h3>Flīzes (<em>"tiles"</em>)</h3>
    <p>
      Jā izvēļu nav daudz (līdz 7) un par katru izvēli jāattēlo vairāk informācijas, kā tikai
      nosaukumu, var izmantot flīžu variantu.
    </p>
    <LxForm :show-header="false" :show-footer="false" :column-count="2">
      <LxRow label="Mīļākā spēle" :column-span="2">
        <LxValuePicker :items="itemsShort" v-model="value" variant="tiles" />
      </LxRow>
      <LxRow label="Mīļākās spēles">
        <LxValuePicker
          :items="itemsShort"
          v-model="values"
          :nullable="true"
          kind="multiple"
          variant="tiles"
        />
      </LxRow>
    </LxForm>
  </article>
  <article class="lx-article">
    <h2>Meklēšana</h2>
    <p>
      Ja izvēļu ir pietiekoši daudz, ir jēga iespējot vērtību izvēlnē iebūvēto meklēšanas iespēju.
    </p>

    <LxForm :show-header="false" :show-footer="false" :column-count="2">
      <LxRow :column-span="2" label="Variants">
        <LxContentSwitcher
          v-model="selectedVariant"
          :items="variants"
          idAttribute="id"
          nameAttribute="name"
        />
      </LxRow>
      <LxRow label="Mīļākā spēle">
        <LxValuePicker
          :items="items"
          v-model="value"
          :nullable="true"
          :variant="selectedVariant"
          :has-search="true"
        />
      </LxRow>
      <LxRow label="Mīļākās spēles">
        <LxValuePicker
          :items="items"
          v-model="values"
          :nullable="true"
          kind="multiple"
          :variant="selectedVariant"
          :has-search="true"
        />
      </LxRow>
    </LxForm>
  </article>
</template>
