<script setup lang="ts">
import { shallowRef } from 'vue';
import { LxContentSwitcher } from '@wntr/lx-ui';
import { useRouter } from 'vue-router';

const router = useRouter();
const routerValue = shallowRef('guidelines');
function contentSwitcherClick() {
  if (routerValue.value === 'sandbox') {
    router.push({ name: 'checkbox' });
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
      Ja tiek piedāvāta izvēle no vairākiem variantiem un ir jāizvēlās
      <b>neviens, viens, vai vairāki</b>
      varianti, tad jāizmanto izvēles rūtiņas.
    </p>
    <p>
      Ja grupā pastāv varianti, kas ir izvēlēti, bet lietotājam nav iespējas to izķeksēt (piemēram,
      lietotājam nav tiesību, vai arī process neļauj šo izvēli neizvēlēties), tad šis variants ir
      [jāatslēdz], pārējos variantus atstājot interaktīvus.
    </p>
    <p class="lx-warning">
      Jāizvairās no dažādu grupu izvēļu ievietošanas vienā izvēles rūtiņu grupā - tātad, izvēlēm
      jābūt loģiski saistītiem savā starpā.
    </p>
    <p class="lx-warning">
      Jāizvairās no darbībām, kas izpildīsies, veicot izvēles rūtiņas izvēli. Izvēles rūtiņas
      atķeksēšana nedrīkst mainīt neko sistēmā, ja darbības ir nepieciešamas, tām jānotiek kādas
      citas pogas nospiešanas rezultātā.
    </p>
  </article>
</template>
