<script setup lang="ts">
import { shallowRef, computed } from 'vue';
import { LxContentSwitcher, LxList, LxToggle, LxFlag, LxIcon, LxButton } from '@wntr/lx-ui';
import { useRouter } from 'vue-router';
import { useDemoStore } from '@/stores/useDemoStore';
import useAuthStore from '@/stores/useAuthStore';

const authStore = useAuthStore();
const demoStore = useDemoStore();
const router = useRouter();
const routerValue = shallowRef('guidelines');

const listTypes = [
  { id: '1', name: '1 kolonna' },
  { id: '2', name: '2 kolonnas' },
  { id: '3', name: '3 kolonnas' },
];

const listType = shallowRef('1');
const useCategories = shallowRef(false);
const useCustomTemplate = shallowRef(false);
const useNavigate = shallowRef(true);
const useInfo = shallowRef(false);
const useDelete = shallowRef(false);

const actions = computed(() => {
  const ret = [];
  if (useInfo.value) {
    ret.push({ id: 'info', name: 'Vairāk informācijas', icon: 'info' });
  }
  if (useDelete.value) {
    ret.push({
      id: 'delete',
      name: 'Dzēst ierakstu',
      icon: 'delete',
      destructive: true,
    });
  }
  return ret;
});

const itemsNonClickable = computed(() => {
  let ret = demoStore.videoGames.map((item) => ({
    ...item,
    href: null,
    clickable: null,
    country: typeof item.country === 'string' ? item.country.toString() : 'US',
  }));
  if (!useCategories.value) {
    ret = ret.map((item) => ({ ...item, category: null }));
  }
  return ret.sort((a, b) => a.name.localeCompare(b.name));
});
const itemsClickable = computed(() => {
  let ret = demoStore.videoGames.map((item) => ({
    ...item,
    href: null,
    clickable: useNavigate.value,
    icon: 'next',
    description: null,
    country: typeof item.country === 'string' ? item.country.toString() : 'US',
  }));
  if (!useCategories.value) {
    ret = ret.map((item) => ({ ...item, category: null }));
  }
  return ret.sort((a, b) => a.name.localeCompare(b.name));
});

const formatter = new Intl.NumberFormat('lv-LV', {
  minimumFractionDigits: 0,
  maximumFractionDigits: 0,
});

const items3simple = demoStore.videoGames
  .sort((a, b) => a.popularity.toString().localeCompare(b.popularity.toString()))
  .map((item) => ({
    ...item,
    href: null,
    clickable: null,
    category: null,
    country: typeof item.country === 'string' ? item.country.toString() : 'US',
    fancyName: `${item.name} (${
      typeof item.country === 'string' ? item.country.toString().toUpperCase() : 'US'
    })`,
    fancyDescription: `${formatter.format(item.popularity * 1000)}  spēlētāju; ${
      item.multiplayer ? 'daudzspēlētāju režīms' : 'viena lietotāja režims'
    }`,
  }))
  .slice(0, 3);
function contentSwitcherClick() {
  if (routerValue.value === 'sandbox') {
    router.push({ name: 'list' });
  }
  if (routerValue.value === 'apiData') {
    router.push({ name: 'apiList' });
  }
}
const navItems = [
  { id: 'sandbox', name: 'Smilškaste' },
  { id: 'apiData', name: 'Piemērs' },
  { id: 'guidelines', name: 'Vadlīnijas' },
];
</script>
<template>
  <div>
    <LxContentSwitcher :items="navItems" v-model="routerValue" @click="contentSwitcherClick" />
    <article class="lx-article">
      <h2>Pamati</h2>

      <p>
        <em>LxList</em> (Saraksts) ir standarta, noklusētā komponente, ko izmantojam strukturētu
        sarakstu attēlošanai. Tas ir viegls un ērts sarakstu attēlošanas rīks.
      </p>
      <p>
        Saraksta galvenais uzdevums ir apskatīt pieejamos objektus un izvēlēties vienu no tiem
        detalizētai apskatīšanai.
      </p>
      <p>
        Saraksts ir paredzēts informācijas <strong>attēlošanai</strong>, nevis
        <strong>ievadei</strong>. Tajā nekādā gadījumā nedrīkst parādīties ievadlauki vai
        interaktīvie elementi (izņemot pogas tām paredzētajā vietā).
      </p>
      <p>
        Tikai gadījumos, kad lietotājs ir <em>Power user</em> līmenī un lietotāja vajadzības darbā
        ar tabulu nav determinētas (nevar paredzēt, kādās kombinācijās, ar kādiem filtriem un kādā
        secībā lietotājs meklēs un darbosies ar sarakstu), izskatām iespēju izmantot
        <RouterLink :to="{ name: 'dataGrid' }">LxDataGrid</RouterLink>
      </p>
      <h2>Saraksta attēlošana</h2>
      <p>
        Sarakstu var attēlot vienā, divās vai trijās kolonnās. Jo svarīgāka skenēšana (vērtības ātrā
        atrašana "ar acīm" starp daudzām līdzīgām vērtībām), jo mazāk kolonnu jāizmanto.
      </p>
      <p>Lai vizuāli izšķirtu vērtības, var izmantot dažādus rīkus:</p>
      <ul>
        <li>
          Piešķirt vērtībām <strong>kategoriju</strong>:
          <strong>
            <span style="color: var(--color-red)">sarkanu</span>,
            <span style="color: var(--color-orange)">oranžu</span>,
            <span style="color: var(--color-green)">zaļu</span>,
            <span style="color: var(--color-blue)">zilu</span>,
            <span style="color: var(--color-purple)">violetu</span> vai
            <span style="color: var(--color-label)">pelēku</span></strong
          >. Parasti laba ideja ir pieskaņot kategoriju krāsas statusu krāsām, vai izmantot vienu
          kategoriju, lai izšķirtu tikko pievienotus un nesaglabātus ierakstus no pārējiem;
        </li>
        <li>
          Izmantot <strong>īpašu izskatu</strong> vērtībām. Šo var izmantot, kad nepieciešama
          nestandarta vizualizācija katram elementam. Šajā gadījumā var izmantot jebkādas
          komponentes skatīšanas režīmā, lai attēlotu informāciju.
        </li>
      </ul>
      <p>
        Sarakstiem jābūt sakārtotiem lietotājam ērtā un saprotamā veidā: pēc alfabēta, izveidošanas
        datuma, statusiem.
      </p>
      <div class="columns columns-3">
        <div><LxToggle v-model="useCategories">Izmantot kategorijas</LxToggle></div>
        <div>
          <LxContentSwitcher
            v-model="listType"
            :items="listTypes"
            id-attribute="id"
            name-attribute="name"
          />
        </div>

        <div><LxToggle v-model="useCustomTemplate">Izmantot īpašu izskatu</LxToggle></div>
      </div>

      <LxList :items="itemsNonClickable" :list-type="listType" v-show="!useCustomTemplate" />

      <LxList :items="itemsNonClickable" :list-type="listType" v-show="useCustomTemplate">
        <template #customItem="{ name, rating, country, popularity, multiplayer }">
          <div class="fancy-game">
            <div class="flag">
              <LxFlag :value="country" />
            </div>
            <p class="lx-primary">{{ name }}</p>
            <p class="lx-secondary">
              <LxIcon value="star" />
              <span>{{ rating }};&nbsp;</span>
              <LxIcon :value="multiplayer ? 'users' : 'user-profile'" /><span>{{
                popularity
              }}</span>
            </p>
          </div>
        </template>
      </LxList>
    </article>
    <article class="lx-article">
      <h2>Informācijas struktūra</h2>
      <p>
        Varētu likties, ka šāda saraksta komponente ļoti ierobežo pielietojumus un informācijas
        attēlošānu: ir taču tikai divi lauki, ko var izmantot informācijas attēlošanai un pat nav
        paredzētas smukas birkas!
      </p>
      <p>
        Īstenībā saraksts ir ļoti elastīgs veids, kā vienkārši attēlot praktiski jebkādas
        sarežģītības struktūru, bet labs rezultāts prasa lietotāju procesu analīzi un projektēšanu.
      </p>
      <p>Izskatīsim trīs līmeņus saraksta projektējumam uz viena un tā paša objekta piemēra.</p>
    </article>
    <article class="lx-article">
      <h3>1. līmenis: nosaukums un apraksts</h3>
      <p>
        Parasti jebkuram objektam ir kaut kāds lietotājam saprotams nosaukums (tas varētu būt
        dokumenta numurs, personas vārds, uzņēmuma nosaukums) - jebkas, ko cilvēki sarunā varētu
        izmantot, lai identificētu objektu - un kaut kāda papildus informācija, kas palīdz vairāk
        saprast, kas tas ir par objektu (personas kods, dokumenta veids, atskaites veidošanas
        datums).
      </p>
      <p>
        Šādos gadījumos attēlojam to informāciju saraksta elementa primārajā un sekundārajā laukā.
      </p>
      <LxList :items="items3simple" />
    </article>
    <article class="lx-article">
      <h3>2. līmenis: advancētais nosaukums un apraksts</h3>
      <p>
        Datu bāzes objekts ir tehnisks instruments. Parastais cilvēks izmanto biznesa objekta
        konceptuālo modeli. Tās divas lietas reti sakrīt.
      </p>
      <p>
        Tas, ko lietotājs saprot ar personas vārdu (vienu biznesa atribūtu), datu bāzē varētu būt
        divi lauki: vārds un uzvārds. Sarakstam jāattēlo informācija lietotājam, tāpēc gan
        primārais, gan sekundārais lauks var saturēt vairāk, nekā viena datu bāzes lauka
        infromāciju. Tā informācija jāformē pēc lietotāja valodas gramatikas un punktuācijas
        principiem - atdalot ar semikoliem, komatiem, utt.
      </p>
      <p>
        Atceramies, ka datiem jārunā pašiem par sevi, datu birkas nav jāiekļauj saraksta
        informācijā. Ja liekas, ka ar attēlojamo informāciju pietrūkst, lai lietotājam būtu skaidrs,
        kas tas ir - saraksta elementam var pievienot paskaidri vai paļauties, ka detalizēto
        informāciju varēs redzēt uzklikšķinot uz elementa.
      </p>
      <LxList
        :items="items3simple"
        primary-attribute="fancyName"
        secondary-attribute="fancyDescription"
      />
    </article>
    <article class="lx-article">
      <h3>3. līmenis: īpašais izskats visam elementam</h3>
      <p>
        Gadījumā, kad ieraksta saturs jāvizualizē īpašāk - ar papildus komponentēm, vai īpašo
        izkārtojumu, vai izmantot "īpašos izskatus".
      </p>
      <p>
        Nevajadzētu pārcensties. Īpašo izskatu struktūrai joprojām jābūt maksimāli vienkāršai un
        neinteraktīvai. Šo variantu izmantojam tikai, lai atvieglotu saraksta skenēšanu vai padarītu
        ierakstu informāciju vieglāk uztveramu.
      </p>
      <p class="lx-warning">
        Ja tiek izmantots šis variants, jāatcerās nodrošināt arī visas citas saraksta elementu
        piemītošas īpašības: meklēšanas rezultāta iekrāsošana, primārās informācijas "ietreknošana",
        pareizais izskats uz peles uzbraukšanas interaktivitātes gadījumā, utt. Priekš lietotāja
        dažādu stilu sarakstiem jāizskatās maksimāli pietuvinātiem.
      </p>
      <LxList :items="items3simple">
        <template #customItem="{ name, rating, country, popularity, multiplayer }">
          <div class="fancy-game">
            <div class="flag">
              <LxFlag :value="country" />
            </div>
            <p class="lx-primary">{{ name }}</p>
            <p class="lx-secondary">
              <LxIcon value="star" />
              <span>{{ rating }};&nbsp;</span>
              <LxIcon :value="multiplayer ? 'users' : 'user-profile'" /><span>{{
                popularity
              }}</span>
            </p>
          </div>
        </template>
      </LxList>
    </article>
    <article class="lx-article">
      <h2>Meklēšana un papildus darbības</h2>
      <p>Lai atvieglotu saraksta elementu atrašanu, tam var iespējot meklēšanas funkcijas.</p>
      <p>
        Meklēšana var notikt gan klienta pusē (bez pieprasījumiem uz datu bāzi), gan servera pusē.
      </p>
      <p>
        Līdzīgi, sarakstam var definēt papildus funkcijas: pogas, slēdžus, utt. Šīm darbībām
        jāattiecās uz visu sarakstu un jāatcerās par izskatu uz mazākiem ekrāniem
      </p>
      <LxList :items="itemsNonClickable" list-type="1" :has-search="true">
        <template #toolbar><LxButton icon="add" label="Pievienot" /></template>
      </LxList>
    </article>
    <article class="lx-article">
      <h2>Grupēšana</h2>
      <p>Ja sarakstā ir pārāk daudz elementu, tos var grupēt.</p>
      <p>
        Grupām jābūt loģiski paredzamām un saprotamām. Tie var būt kādi no objektu atribūtiem
        (piemēram, pilsētas, iestādes, žanri), statusi (pabeigtie darbi, melnraksti), datumi
        (mēneši, gadi) vai citu veidu loģiska grupēšana (alfabētiski, pēc nosaukuma pirmā burta)
      </p>
      <LxList
        :items="itemsNonClickable"
        list-type="1"
        :group-definitions="[
          {
            name: 'Vairākspēlētāju tiešsaistes kaujas arēnas spēles',
            id: '1',
            expander: true,
          },
          { name: 'Asa sižeta lomu spēles', id: '2', expander: true },
          { name: 'Pirmās personas šaušanas spēles', id: '3', expander: true },
          { name: 'Citu žanru spēles', id: '4', expander: true },
        ]"
      />
    </article>
    <article class="lx-article">
      <h2>Interaktivitāte</h2>
      <p>Jebkāda veida interaktivitāte ar saraksta elementiem notiek, izmantojot elementu pogas.</p>
      <p>
        Pēc noklusējuma, iespējojot interaktivitāti, pats elements paliek par interaktīvo objektu.
        Tāpēc svarīgi, lai saraksta elements nesaturētu nekādus ievadlaukus vai pogas.
      </p>
      <p>Visas papildus darbības tiek definētas papildus pogās.</p>
      <div class="columns columns-3">
        <div><LxToggle v-model="useNavigate">Iespējot pamata darbību</LxToggle></div>
        <div><LxToggle v-model="useInfo">Parādīt info darbību</LxToggle></div>
        <div><LxToggle v-model="useDelete">Parādīt dzēšanas darbību</LxToggle></div>
      </div>
      <LxList :items="itemsClickable" :actionDefinitions="actions" list-type="2" />
    </article>
  </div>
</template>
