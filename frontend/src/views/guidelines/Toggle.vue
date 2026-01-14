<script setup lang="ts">
import { shallowRef } from 'vue';
import { LxContentSwitcher, LxRow, LxForm, LxToggle } from '@wntr/lx-ui';
import { useRouter } from 'vue-router';

const router = useRouter();
const routerValue = shallowRef('guidelines');
const value = shallowRef(false);
function contentSwitcherClick() {
  if (routerValue.value === 'sandbox') {
    router.push({ name: 'toggle' });
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
      <em>LxToggle</em> (Slēdzi) izmantojam, kad lietotājam ir jāveic viena binārā izvēle:
      "Jā"&nbsp;vai&nbsp;"Nē", "Ieslēgt"&nbsp;vai&nbsp;"Izslēgt",
      "Ir&nbsp;iekļauts"&nbsp;vai&nbsp;"Nav&nbsp;iekļauts"
    </p>
    <p>
      Tehniski, slēdzis nodrošina arī trešo variantu: "Nav izvēlēts", taču tas apgrūtina slēdža
      izmantošanu un nav paredzams, tāpēc gadījumos, kad lietotājam ir jāizvēlas starp trīs izvēlēm,
      izmantojam <RouterLink :to="{ name: 'valuePicker' }">LxValuePicker</RouterLink>
    </p>
    <p class="lx-warning">
      Slēdzis nav paredzēts izvēlei no saraksta! Ja ir saraksts ar elementiem, no kuriem ir
      jāizvēlās viena vai dažas vērtības, izmantojam
      <RouterLink :to="{ name: 'valuePicker' }">LxValuePicker</RouterLink>!
    </p>
    <p class="lx-warning">
      Slēdzis nav paredzēts neacīmredzami bināro vērtību izvēlei! Piemēram, ja jāizvēlas starp
      "Lietvedis" un "Administrators", izmantojam
      <RouterLink :to="{ name: 'valuePicker' }">LxValuePicker</RouterLink>, nevis slēdzi!
    </p>
    <p class="lx-warning">
      Slēdzis nav <RouterLink :to="{ name: 'button' }">poga</RouterLink>! Klikšķis uz tā nedrīkst
      izraisīt nekādas darbības sistēmā (izņemot kosmētiskas - piemēram, lauku rādīšanu, slēpšanu
      vai atslēgšanu)!
    </p>
    <h2>Slēdža birkas</h2>
    <p>
      Slēdzim ir sava birka, ko jāizmanto, lai paskaidrotu, kādu vērtību slēdzis maina. Atsevisķajos
      gadījumos ir pieļaujams izmantot vienveidīgu formas lauku birku, tad slēdža birka jāatstāj
      tukša.
    </p>
    <LxForm :show-header="false" :show-footer="false" :column-count="2">
      <LxRow><LxToggle v-model="value">Iekļauj līgumā</LxToggle></LxRow>
      <LxRow label="Iekļauj līgumā"><LxToggle v-model="value" /></LxRow>
    </LxForm>
    <p class="lx-warning">
      Izmantojot slēdzi, vienmēr jānorāda, kādu vērtību slēdzis maina! Birkai jāattēlo tā vērtība,
      kura stāsies spēkā, kad slēdzis <strong>ir ieslēgts</strong> (kad tā vērtība ir
      <em>true</em>). Retajos gadījumos ir pieļaujams mainīt birkas tekstu, atkarībā no slēdža
      vērtības. Šajos gadījumos birkai jāatēlo tā vērtība, kas <strong>pašlaik ir izvēlēta</strong>,
      nevis tā, uz kuru mainīsies ar klikšķi.
    </p>
  </article>
</template>
