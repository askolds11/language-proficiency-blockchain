<script setup>
import { onMounted, ref } from 'vue';
import {
  LxForm,
  LxRow,
  LxTextInput,
  LxDropDown,
  LxDateTimePicker,
  LxToggle,
  LxStateDisplay,
  lxDateUtils,
} from '@wntr/lx-ui';

import { useDemoStore } from '@/stores/useDemoStore';
import useViewStore from '@/stores/useViewStore';
import useNotifyStore from '@/stores/useNotifyStore';

const notifications = useNotifyStore();

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

function buttonClicked(actionName) {
  notifications.pushInfo('Nospiesta formas poga!', `"${actionName}"`);
}

onMounted(() => {
  viewStore.showBack();
});
</script>
<template>
  <div>
    <article class="lx-article">
      <p>
        Formas ir ļoti fleksibls un universāls rīks, strukturētās informācijas veinveidīgai
        attēlošanai un ievadei lietotājam draudzīgā veidā.
      </p>
      <p>
        Ļoti svarīgi nodrošināt, lai visā sistēmā formas "uzvedās" un izskatās lietotājam maksimāli
        paredzamā veidā. Zemāk ir aprakstīts šis veids.
      </p>
    </article>

    <article class="lx-article">
      <h2>Vienkārša forma</h2>
      <p>Šī ir vienkāršākā ievadforma:</p>
    </article>

    <LxForm :show-header="false" :show-footer="false">
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
      <LxRow label="Dzimšanas datums">
        <LxDateTimePicker v-model="testData.birthDate" :read-only="readOnly" />
      </LxRow>
    </LxForm>
    <article class="lx-article">
      <p>Formas var izmantot ne tikai datu aizpildīšanai, bet arī attēlošanai:</p>
      <LxToggle v-model="readOnly">Skatīšanas režīms</LxToggle>
    </article>
    <article class="lx-article">
      <h2>Formas galvene</h2>
      <p>Formas galvene ir svarīga tās sastāvdaļa.</p>
      <p>
        Atkarībā no formas lauku skaita, formas var palikt pietiekoši garas un daļas no formas var
        palikt neredzamas, lietotājam ritinot pa lapu. Formas galveni izmanto, lai attēlotu tādu
        informāciju par formas saturu, kas varētu palīdzēt lietotājam uzturēt lietošanas kontekstu,
        strādājot ar formu (palīdzētu neaizmirst svarīgu informāciju par formas kontekstu).
      </p>
      <p>
        Ritinoties pa lapu, kur atrodas forma, formas galvene paliek "piestiprināta" pie lapas
        augšējās malas un ir redzama lietotājam visu laiku, strādājot ar formu.
      </p>

      <LxToggle v-model="showHeader">Rādīt galveni</LxToggle>
      <p>
        Galvenes telpa ir ļoti ierobežota, tāpēc rūpīgi jāizvērtē, kāda informācija tur jāizvieto.
        Informāciju drīkst izkārtot tikai vienā rindā.
      </p>
      <p>Izdalām trīs formas galvenes zonas:</p>
      <ul>
        <li>
          <strong>Pamata zona</strong> ir obligāta, izmantojot galveni. Šeit izvietojam informāciju
          par to, kas ir tas, ko attēlojam šajā formā: nosaukumu, numuru, kaut kādu identificējošo
          informāciju. Sliktākajā gadījumā, ja identificējošas informācijas nav, vai nu paslēpjam
          galveni, vai nu ierakstam objekta nosaukumu ("Personas kartīte", "Rēķins").
        </li>
        <li>
          <strong>Statusa zona</strong>. Šeit izvietojam informāciju par to, kas notiek ar šo formu:
          formas <router-link :to="{ name: 'stateDisplay' }">objekta statusu</router-link>,
          izmantošanas režīmu, utt.
        </li>
        <li>
          <strong>Papildus informācijas zona</strong>. Šeit izvietojam papildus informāciju par
          formas objektu - parasti tas ir izveidošanas vai pēdējās labošanas datums, atbilstošas
          nodaļas nosaukums, saistītā pacienta vārds, uzvārds, utt.
        </li>
      </ul>

      <p>
        Detalizētu informāciju par formas objektu (piemēram, personas, kas izveidoja vai laboja
        dokumentu, utt.) vēlams izkārtot papildus informācijas zonas paskaidrē. Informāciju par
        statusu vēsturi (kurš lietotājs ir mainījis kuru statusu) var izvietot statusa zonas
        paskaidrē, ja papildus informācijas zonas paskaidrē jau tiek attēlots pārāk daudz
        informācijas.
      </p>

      <div class="columns columns-3">
        <div>
          <LxToggle v-model="showPreHeader" :disabled="!showHeader">Rādīt statusa zonu</LxToggle>
          <LxToggle v-model="showPreHeaderTooltip" :disabled="!showHeader || !showPreHeader">
            Rādīt statusa zonas paskaidri
          </LxToggle>
        </div>
        <div></div>
        <div>
          <LxToggle v-model="showPostHeader" :disabled="!showHeader">
            Rādīt papildus info zonu</LxToggle
          >
          <LxToggle v-model="showPostHeaderTooltip" :disabled="!showPostHeader || !showHeader">
            Rādīt statusa zonas paskaidri</LxToggle
          >
        </div>
      </div>
    </article>
    <LxForm
      :show-header="showHeader"
      :show-footer="false"
      :column-count="1"
      :showPreHeaderInfo="showPreHeaderTooltip"
      :showPostHeaderInfo="showPostHeaderTooltip"
    >
      <template #header> Personas kartīte #2123 </template>
      <template #pre-header v-if="showPreHeader">
        <LxStateDisplay
          value="accepted"
          :dictionary="[
            {
              value: 'accepted',
              displayName: 'Apstiprināta',
              displayType: 'green-full',
            },
          ]"
        />
      </template>
      <template #post-header v-if="showPostHeader"> {{ lxDateUtils.formatDate(today) }} </template>
      <template #pre-header-info>
        <LxRow label="Izveidota:">
          <p class="lx-data">{{ lxDateUtils.formatDateTime(creationDate) }} (Jānis Bērziņš)</p>
        </LxRow>
        <LxRow label="Apstiprināta:">
          <p class="lx-data">{{ lxDateUtils.formatDateTime(today) }} (Jānis Bērziņš)</p>
        </LxRow>
      </template>
      <template #post-header-info>
        <div class="lx-row">
          <label>Nodaļa:</label>
          <p class="lx-data">Lietvedības nodaļa</p>
        </div>
        <template v-if="!showPreHeaderTooltip">
          <LxRow label="Izveidota:">
            <p class="lx-data">{{ lxDateUtils.formatDateTime(creationDate) }} (Jānis Bērziņš)</p>
          </LxRow>
          <LxRow label="Apstiprināta:">
            <p class="lx-data">{{ lxDateUtils.formatDateTime(today) }} (Jānis Bērziņš)</p>
          </LxRow>
        </template>
      </template>
      <LxRow label="Vārds">
        <LxTextInput v-model="testData.firstName" />
      </LxRow>
      <LxRow label="Uzvārds">
        <LxTextInput v-model="testData.lastName" />
      </LxRow>
      <LxRow label="Mīļākā videospēle">
        <LxDropDown v-model="testData.favGame" :items="videoGames" kind="default" />
      </LxRow>
      <LxRow label="Dzimšanas datums">
        <LxDateTimePicker v-model="testData.birthDate" />
      </LxRow>
    </LxForm>
    <article class="lx-article">
      <p class="lx-warning">
        Attēlojot informāciju formas galvenē, jāizvairās no birkām - galvene attēlo tikai datus.
        Piemēram, tā vietā, lai attēlotu izveidošanas datumu kā
        <em>"Izveidošanas datums: {{ lxDateUtils.formatDateTime(creationDate) }}"</em>, jāattēlo
        <em>"{{ lxDateUtils.formatDateTime(creationDate) }}"</em>. Visos gadījumos, it īpaši, ja ir
        bažas, ka lietotājs varētu pārprast, kas par informāciju tiek attēlota, jāizmanto atbilstoša
        paskaidre, kur jāiekļauj arī birka. Izmantojot formu pirmo reizi, pārpratumus lietotājs
        atrisinās ar paskaidres palīdzību. Nākamreiz paskaidre vairs nebūs nepieciešama.
      </p>
      <p class="lx-warning">
        Attēlojot numuru vai kādu citu objekta identifikatoru, jāizmanto simboli
        "<strong>#</strong>" vai "<strong>№"</strong>, jāizvairās no apzīmējumiem "<em>Nr.</em>",
        "<em>Numurs</em>", "<em>ID</em>", u.c., ja vien tam nav ļoti laba pamatojuma (piemēram, ja
        to nosaka klienta juridiskās prasības).
      </p>
      <h2>Formu pogas</h2>
      <p>Ja darbības ar formu rezultējas ar kādu darbību, tam jāizmanto formas kājene.</p>
      <p>
        <strong>Rezultējošā darbība</strong> ir tāda darbība, dēļ kuras lietotājs ir atvēris šo
        formu - tā var būt formas saglabāšana, pārslēgšanās uz labošanas režīmu, eksportēšana vai
        statusa nomaiņa.
      </p>

      <p>
        Normālā gadījumā, ja forma ir labošanas režīmā, tai jābūt vismaz vienai rezultējošai
        darbībai. Dažreiz arī skatīšanas vai jauktā režīma formas var prasīt rezultējošās darbības.
      </p>
    </article>

    <LxForm
      :show-header="false"
      :show-footer="true"
      :action-definitions="[
        { id: 'save', icon: 'save', name: 'Saglabāt', kind: 'primary' },
        { id: 'cancel', icon: 'undo', name: 'Atcelt', kind: 'secondary' },
      ]"
      @buttonClick="buttonClicked"
    >
      <LxRow label="Vārds">
        <LxTextInput v-model="testData.firstName" />
      </LxRow>
      <LxRow label="Uzvārds">
        <LxTextInput v-model="testData.lastName" />
      </LxRow>
      <LxRow label="Mīļākā videospēle">
        <LxDropDown v-model="testData.favGame" :items="videoGames" kind="default" />
      </LxRow>
      <LxRow label="Dzimšanas datums">
        <LxDateTimePicker v-model="testData.birthDate" />
      </LxRow>
    </LxForm>
    <article class="lx-article">
      <p>
        Līdzīgi kā galvene, formas kājene tiek attēlota lietotājam visu laiku, piestiprinoties pie
        ekrāna apakšējās malas. Tas nozīmē, ka kājenes telpa ir ļoti vērtīga un rezultējošās
        darbības jāizkārto rūpīgi, plānojot darbību grupēšanu un prioritizēšanu.
      </p>
      <p>Formas <router-link :to="{ name: 'button' }">pogas</router-link> iedalām 4 kategorijās:</p>
      <ul>
        <li>
          <strong>Primārā poga</strong> - šī ir galvenā rezultējošā poga, tā parasti ir tikai viena
          un apzīmē galveno darbību, ko lietotājam jāizpilda esošajā kontekstā. Normālā gadījumā
          viena no pogām vienmēr ir primārā, izņemot gadījumu, kad lietotājam jāveic izvēle starp
          divām vienlīdzīgi svarīgām darbībām (piemēram "Apstiprināt" un "Noraidīt", "Balsot par" un
          "Balsot pret");
        </li>
        <li>
          <strong>Sekundārās pogas</strong> - no vienas līdz četrām (jo mazāk, jo labāk) sekundārām
          rezultējošām pogām. Pogas, kas izpilda darbības ar formu, bet nav primāras. Ja biznesa
          process paredz vairāk darbību, tad jāizdomā, ka šīs darbības sagrupēt: piemēram, ieviešot
          pogu "Citas darbības", kuru nospiežot, atvērsies iespēja izvēlēties tālākās darbības.
          Formas aizvēršanas, izmaiņu atcelšanas vai atgriešanas poga vienmēr ir sekundāra (un ir
          pēdējā no sekundārām pogām).
        </li>
        <li>
          <strong>Terciārās pogas</strong> - viena (reti - divas) papildus darbība, kurai jābūt
          pieejamai, bet kas ir bīstamas vai nevēlamas (piemēram, formas objekta dzēšana).
        </li>
        <li>
          <strong>Papildus pogas</strong> - visas pārējās darbības, kuras ir vajadzīgas ļoti retajos
          gadījumos. Šīs pogas tiek izkārtotas formas galvenē un ir ļoti neuzkrītošas. Jāpaļaujas,
          ka šīs pogas izmantos tikai tie lietotāji, kas speciāli meklē šo funkcionalitāti. Tā var
          būt, piemēram, detalizēta papildinformācija, eksotiskās eksporta iespējas, utt.
        </li>
      </ul>
    </article>
    <LxForm
      :show-header="true"
      :show-footer="true"
      :action-definitions="[
        { id: 'accept', icon: 'accept', name: 'Apstiprināt', kind: 'primary' },
        { id: 'save', icon: 'save', name: 'Saglabāt', kind: 'secondary' },
        { id: 'cancel', icon: 'undo', name: 'Atcelt', kind: 'secondary' },
        {
          id: 'info',
          icon: 'info',
          name: 'Papildus informācija',
          kind: 'additional',
        },
        {
          id: 'download',
          icon: 'download',
          name: 'Lejupielādēt',
          kind: 'additional',
        },
        {
          id: 'delete',
          icon: 'delete',
          name: 'Dzēst',
          kind: 'tertiary',
          destructive: true,
        },
      ]"
      @buttonClick="buttonClicked"
    >
      <template #header> Personas kartīte #2123 </template>
      <LxRow label="Vārds">
        <LxTextInput v-model="testData.firstName" />
      </LxRow>
      <LxRow label="Uzvārds">
        <LxTextInput v-model="testData.lastName" />
      </LxRow>
      <LxRow label="Mīļākā videospēle">
        <LxDropDown v-model="testData.favGame" :items="videoGames" kind="default" />
      </LxRow>
      <LxRow label="Dzimšanas datums">
        <LxDateTimePicker v-model="testData.birthDate" />
      </LxRow>
    </LxForm>
  </div>
</template>
