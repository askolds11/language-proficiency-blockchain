<script setup>
import { ref } from 'vue';
import { LxIcon } from '@wntr/lx-ui';
import useNotifyStore from '@/stores/useNotifyStore';

const { lxVersions } = window.config;
const selectedReadme = ref('intro');
const notifications = useNotifyStore();

function sanitizeVersion(version) {
  return version.replace(/[^0-9.]/g, '');
}

function goTo(step) {
  if (step === 'bad') {
    notifications.pushError(
      'Notika kļūda, apstrādājot pieprasījumu',
      'Lūdzu, padomājiet vēlreiz un izvēlaties citu variantu!'
    );
  }
  selectedReadme.value = step;
}
</script>

<template>
  <div v-if="selectedReadme === 'intro'" id="readme-intro" class="instructions">
    <div class="placeholder"></div>
    <p>Izvēlies lietošanas scenāriju:</p>
    <ul class="routes">
      <li tabindex="0" @click="goTo('new')" @keyup.enter="goTo('new')" @keyup.space="goTo('new')">
        <label>Man ir jātaisa <strong>jauns</strong> projekts, kurā gribu izmantot LX</label
        ><LxIcon value="next" />
      </li>
      <li
        tabindex="0"
        @click="goTo('existing')"
        @keyup.enter="goTo('existing')"
        @keyup.space="goTo('existing')"
      >
        <label>Man jau ir <strong>eksistējošs</strong> projekts, kurā gribu izmantot LX</label
        ><LxIcon value="next" />
      </li>

      <li
        class="bad-choise"
        tabindex="0"
        @click="goTo('bad')"
        @keyup.enter="goTo('bad')"
        @keyup.space="goTo('bad')"
      >
        <label>Negribu izmantot LX</label><LxIcon value="next" />
      </li>
    </ul>
    <p></p>
    <p>
      Uzsākot jaunu projektu, kas izmanto LX, neaizmirsi palielīties par to
      <a href="https://mms.zzdats.lv/lx/channels/lx-ui" rel="noopener" target="_blank"
        >#LX/UI Mattermost kanālā</a
      >!
    </p>
  </div>
  <div v-if="selectedReadme === 'new'" class="instructions">
    <ul class="routes back">
      <li
        tabindex="0"
        @click="goTo('intro')"
        @keyup.enter="goTo('intro')"
        @keyup.space="goTo('intro')"
      >
        <LxIcon value="back" /><label>Man ir jātaisa jauns projekts, kurā gribu izmantot LX</label>
      </li>
    </ul>
    <p>Lai izveidotu jaunu LX portālu:</p>
    <ol>
      <li>
        <p>
          Nokopē
          <a href="https://git.zzdats.lv/lx/demo" rel="noopener" target="_blank"
            ><strong>lx/demo</strong> repozitoriju</a
          >;
        </p>
      </li>
      <li>
        Pārliecinies, ka sanāk to uzbūvēt, izmantojot
        <a
          href="https://git.zzdats.lv/lx/demo/src/branch/develop/README.md"
          rel="noopener"
          target="_blank"
          >README.md</a
        >
        instrukcijas;
      </li>
      <li>
        <p>
          Pielāgo savu
          <a
            href="https://git.zzdats.lv/lx/demo/src/branch/develop/src/layouts/MainLayout.vue"
            rel="noopener"
            target="_blank"
            >MainLayout.vue</a
          >
          projekta vajadzībām;
        </p>
      </li>
      <li>
        <p>
          Pārliecinies, ka
          <a href="https://git.zzdats.lv/lx/ui/releases" rel="noopener" target="_blank">lx-ui</a>
          atjaunots uz jaunākām versijām, palaižot terminālī šo komandu:
        </p>
        <code>pnpm i -w @wntr/lx-uisanitizeVersion(lxVersions.ui) }}</code>
      </li>
      <li>
        Par trūkumiem un uzlabojumu ierosinājumiem ziņo
        <a href="https://git.zzdats.lv/lx/ui/issues" rel="noopener" target="_blank"
          >lx/ui issues sadaļā</a
        >.
      </li>
    </ol>
  </div>
  <div v-if="selectedReadme === 'existing'" id="readme-intro" class="instructions">
    <ul class="routes back">
      <li
        tabindex="0"
        @click="goTo('intro')"
        @keyup.enter="goTo('intro')"
        @keyup.space="goTo('intro')"
      >
        <LxIcon value="back" /><label
        >Man jau ir eksistējošs projekts, kurā gribu izmantot LX</label
        >
      </li>
    </ul>
    <p>Izvēlies LX lietošanas scenāriju:</p>
    <ul class="routes">
      <li
        tabindex="0"
        @click="goTo('existing-vue')"
        @keyup.enter="goTo('existing-vue')"
        @keyup.space="goTo('existing-vue')"
      >
        <label>Mans projekts izmanto Vue.js 3</label><LxIcon value="next" />
      </li>
      <li
        tabindex="0"
        @click="goTo('existing-other')"
        @keyup.enter="goTo('existing-other')"
        @keyup.space="goTo('existing-other')"
      >
        <label>Man ir projekts citās tehnoloģijās un gribu izmantot LX komponentes</label
        ><LxIcon value="next" />
      </li>
      <li
        tabindex="0"
        @click="goTo('existing-styles')"
        @keyup.enter="goTo('existing-styles')"
        @keyup.space="goTo('existing-styles')"
      >
        <label>Man ir projekts citās tehnoloģijās un gribu, lai tas izskatās pēc LX</label
        ><LxIcon value="next" />
      </li>
      <li tabindex="0" @click="goTo('api')" @keyup.enter="goTo('api')" @keyup.space="goTo('api')">
        <label>Mana projekta API nav veidots Go valodā, bet es gribu izmantot LX</label
        ><LxIcon value="next" />
      </li>
    </ul>
  </div>
  <div v-if="selectedReadme === 'existing-vue'" id="readme-intro" class="instructions">
    <ul class="routes back">
      <li
        tabindex="0"
        @click="goTo('existing')"
        @keyup.enter="goTo('existing')"
        @keyup.space="goTo('existing')"
      >
        <LxIcon value="back" /><label>Mans projekts izmanto Vue.js 3</label>
      </li>
    </ul>
    <p>Lai izmantotu LX komponentes portālos, kas jau izmanto Vue.js 3:</p>
    <ol>
      <li>
        <p>
          Pievieno savai lietotnei
          <a href="https://git.zzdats.lv/lx/ui/releases" rel="noopener" target="_blank">lx-ui</a>
          pakotņu jaunāko versiju, izmantojot komandu:
        </p>
        <code>pnpm i -w @wntr/lx-ui@{{ sanitizeVersion(lxVersions.ui) }}</code>
      </li>
      <li>
        <p>Tagad vari veidot skatus un komponentes, izmantojot LX!</p>
      </li>
      <li>
        Par trūkumiem un uzlabojumu ierosinājumiem ziņo
        <a href="https://git.zzdats.lv/lx/ui/issues" rel="noopener" target="_blank"
          >lx/ui issues sadaļā</a
        >.
      </li>
    </ol>
  </div>
  <div v-if="selectedReadme === 'existing-other'" class="instructions">
    <ul class="routes back">
      <li
        tabindex="0"
        @click="goTo('existing')"
        @keyup.enter="goTo('existing')"
        @keyup.space="goTo('existing')"
      >
        <LxIcon value="back" /><label>Man ir jātaisa jauns projekts, kurā gribu izmantot LX</label>
      </li>
    </ul>
    <p>Lai izmantotu LX komponentes portālos, kas neizmanto Vue.js 3:</p>
    <ol>
      <li>
        <p>
          <a
            href="https://vuejs.org/guide/quick-start.html#creating-a-vue-application"
            target="_blank"
            >Izveido Vue.js 3 lietotni</a
          >
          iekš sava portāla;
        </p>
      </li>
      <li>
        <p>
          Pārliecinies, ka lietotne izmanto
          <a href="https://nodejs.org/en/about" rel="noopener" target="_blank">node.js</a> un
          <a href="https://pnpm.io/installation" rel="noopener" target="_blank">pnpm</a>
        </p>
      </li>
      <li>
        <p>
          Pievieno jaunizveidotai Vue.js 3 lietotnei
          <a href="https://git.zzdats.lv/lx/ui/releases" rel="noopener" target="_blank">lx-ui</a>
          pakotņu jaunāko versiju, izmantojot komandu:
        </p>
        <code>pnpm i -w @wntr/lx-ui@{{ sanitizeVersion(lxVersions.ui) }}</code>
      </li>
      <li>
        <p>Tagad vari veidot skatus un komponentes, izmantojot LX!</p>
      </li>
      <li>
        Par trūkumiem un uzlabojumu ierosinājumiem ziņo
        <a href="https://git.zzdats.lv/lx/ui/issues" rel="noopener" target="_blank"
          >lx/ui issues sadaļā</a
        >.
      </li>
    </ol>
  </div>
  <div v-if="selectedReadme === 'existing-styles'" class="instructions">
    <ul class="routes back">
      <li
        tabindex="0"
        @click="goTo('existing')"
        @keyup.enter="goTo('existing')"
        @keyup.space="goTo('existing')"
      >
        <LxIcon value="back" /><label
        >Man ir projekts citās tehnoloģijās un gribu, lai tas izskatās pēc LX</label
        >
      </li>
    </ul>
    <p>
      Ja gribi, lai tavs portāls izskatās pēc LX, Tev būs jāizmanto CSS datnes no lx/ui bibliotēkas:
    </p>
    <ol>
      <li>
        <p>
          Pievieno savam projektam
          <a href="https://git.zzdats.lv/lx/ui/releases" rel="noopener" target="_blank">lx-ui</a>
          pakotnes jaunāko versiju, izmantojot komandu:
        </p>
        <code>pnpm i -w @wntr/lx-ui@{{ sanitizeVersion(lxVersions.ui) }}</code>
      </li>
      <li>
        <p>
          Piesaisti LX ekspertus, lai izdomātu, kā var izmantot LX stilus tavā projektā, ierakstot
          ziņu
          <a href="https://mms.zzdats.lv/lx/channels/lx-ui" rel="noopener" target="_blank"
            >#LX/UI Mattermost kanālā</a
          >
        </p>
      </li>
    </ol>
  </div>
  <div v-if="selectedReadme === 'api'" class="instructions">
    <ul class="routes back">
      <li
        tabindex="0"
        @click="goTo('existing')"
        @keyup.enter="goTo('existing')"
        @keyup.space="goTo('existing')"
      >
        <LxIcon value="back" /><label
        >Mana projekta API nav veidots Go valodā, bet es gribu izmantot LX</label
        >
      </li>
    </ul>
    <p>Tā vispār nav problēma.</p>
    <p>LX/UI ir komponentes un stili, kas ir domāti <em>front-end</em> lietotnēm.</p>
    <p>
      Skati un portāli, kas ir veidoti, izmantojot LX, spēj strādāt ar praktiski jebkādās
      tehnoloģijās veidotiem API.
    </p>
    <p>
      Ja Tavs API ir veidots, implementējot
      <a href="https://www.openapis.org/" rel="noopener" target="_blank">OpenAPI specifikāciju</a>,
      tad LX portāls visdrīzāk pat nepamanīs, ka viņa API nav veidots Go valodā (mums pat ir
      veiksmīgi piemēri, kad API ir veidots uz <em>.NET Core</em> bāzes).
    </p>
    <p>
      Tiesa, cīnoties par standartizēšanu, pārizmantojamību un piemēru esamību, rekomendējam klienta
      API veidot, izmantojot Go un OpenAPI.
    </p>
  </div>
  <div v-if="selectedReadme === 'bad'" class="instructions">
    <ul class="routes back">
      <li
        tabindex="0"
        @click="goTo('intro')"
        @keyup.enter="goTo('intro')"
        @keyup.space="goTo('intro')"
      >
        <LxIcon value="back" /><label>Negribu izmantot LX</label>
      </li>
    </ul>
    <div class="really"></div>
  </div>
</template>
