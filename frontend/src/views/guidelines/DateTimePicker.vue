<script setup lang="ts">
import { shallowRef } from 'vue';
import { LxContentSwitcher, LxDateTimePicker } from '@wntr/lx-ui';
import { useRouter } from 'vue-router';

const router = useRouter();
const routerValue = shallowRef('guidelines');
function contentSwitcherClick() {
  if (routerValue.value === 'sandbox') {
    router.push({ name: 'dateTimePicker' });
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
      <em>LxDateTimePicker</em> (Datuma un laika izvēlne) tiek izmantots, lai izvēlētos pagātnes,
      tagadnes un nākotnes datumu, laiku vai datumu un laiku.
    </p>
    <p>
      Datuma un laika izvēlne aizņem tik daudz horizontālas telpas, cik ir nepieciešams vērtībai.
      Plēst to garāku, lai tā aizņem tik pat vietas, cik pārējie lauki, nevajadzētu!
    </p>
    <p>
      Izvēlnei var norādīt minimālo un maksimālo vērtību, kā arī mainīt paskaidri (nav ieteicāms -
      jādara tikai īpašajos gadījumos).
    </p>
    <p>
      Datumu perioda ievadei izmantojam
      <RouterLink :to="{ name: 'dateTimeRange' }">LxDateTimeRange</RouterLink>.
    </p>
    <h2>Varianti</h2>
    <p>
      Svarīgi pareizi izvēlēties komponentes variantu tā, lai lietotājam būtu jāizvēlas minimālā
      vērtība, kas ir nepieciešama konkrētajā gadījumā. Piemēram, dzimšanas datuma izvēlei normālā
      gadījumā nevajadzētu piedāvāt arī laika izvēli.
    </p>
    <h3>Datuma ievade</h3>
    <p>Populārākais variants, to izmanto, kas ir jāizvēlas viens konkrēts datums.</p>
    <LxDateTimePicker />
    <p>&nbsp;</p>
    <h3>Laika ievade</h3>
    <p>
      Tajos retajos gadījumos, kad lietotājam jāievada tikai laiks, bez datuma, jāizmanto šis
      variants.
    </p>
    <LxDateTimePicker kind="time" />
    <p>&nbsp;</p>
    <h3>Datuma un laika ievade</h3>
    <p>Kad lietotājam jāievada datums un laiks, jāizmanto šis variants.</p>
    <p class="lx-warning">
      Nezināmu iemeslu dēļ, dažreiz parādāš kārdinājums izmantot divus ievadlaukus: vienu datuma
      izvēlei, otru - laika izvēlei, ko noliek blakus vienu otrai. Bet nedarām tā!
    </p>
    <LxDateTimePicker kind="dateTime" />
  </article>
</template>
