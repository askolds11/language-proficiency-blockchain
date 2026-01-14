<script setup lang="ts">
import { shallowRef } from 'vue';
import { LxContentSwitcher } from '@wntr/lx-ui';
import { useRouter } from 'vue-router';

const router = useRouter();
const routerValue = shallowRef('guidelines');
function contentSwitcherClick() {
  if (routerValue.value === 'sandbox') {
    router.push({ name: 'radioButton' });
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
    <p>
      Ja tiek piedāvāta izvēle no vairākiem variantiem un ir jāizvēlās tikai viens variants, tad
      jāizmanto radio pogu grupa. Ja izvēļu skaits ir mazāks vai vienāds ar 5, pieļaujams izmantot
      kombo-izvēlni radio pogu grupas vietā.
    </p>
    <p>
      Radio pogu grupā vienmēr jābūt izvēlētam vienam variantam, tāpēc izvēļu klāsts jāveido
      atbilstoši: piemēram, ja tam ir jēga, jāpievieno variants "Nav izvēlēts", vai izvēlēties vienu
      no variantiem kā noklusēto, kas būs izvēlēts bez lietotāja iesaistes (šajos gadījumos
      jāuzmanās no iespējamām
      <a
        target="_blank"
        rel="noopener noreferrer nofollow"
        href="https://intra.zzdats.lv/pages/viewpage.action?pageId=22857715"
      >
        lietotāju kļūdām</a
      >
      - ja variants būs izvēlēts noklusēti un tā izvēle nav acīmredzama, lietotājs var saglabāt
      vērtību, ko nemaz nav sagaidījis).
    </p>
    <p class="lx-warning">
      Radio pogas izvēle nedrīkst izraisīt nekādus procesus sistēmā - tikai kosmētiskas izmaiņas
      formā - slēpt/attēlot formas sadaļas, pogu atslēgšana, utt.
    </p>
  </article>
</template>
