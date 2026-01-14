<script setup lang="ts">
import { shallowRef } from 'vue';
import { LxContentSwitcher } from '@wntr/lx-ui';
import { useRouter } from 'vue-router';

const router = useRouter();
const routerValue = shallowRef('guidelines');
function contentSwitcherClick() {
  if (routerValue.value === 'sandbox') {
    router.push({ name: 'textInput' });
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
    <p>Teksta ievades lauku izmantojam, kad lietotājam jāievada tekstuāla vērtība manuāli.</p>
    <p>
      Ja ir iespējams ļaut lietotājam izvēlēties vērtību no klasifikatora vai kāda saraksta -
      jāizmanto citas komponentes. Tikai, ja vērtība jāvada "ar roku", izmantojam teksta ievades
      lauku.
    </p>
    <p>
      Ja ir jāļauj lietotājam ievadīt tekstuālo vērtību vairākās rindās, izmantojam
      <RouterLink :to="{ name: 'textArea' }">LxTextArea</RouterLink> vai arī
      <RouterLink :to="{ name: 'markdownTextArea' }">LxMarkdownArea</RouterLink> formatētā teksta
      gadījumā.
    </p>
    <h2>Maskas</h2>
    <p>
      Lai minimizētu kļūdas iespēju ievadītajā vērtībā, teksta ievades laukam var piemērot masku.
      Maskas ierobežo atļautos simbolus, tādejādi nepieļaujot ievades kļūdas.
    </p>
    <p>Pašlaik ir pieejamas šādas maskas:</p>
    <ul>
      <li><strong>default</strong> - noklusētā maska, kas pieļauj jebkādu tekstu;</li>
      <li><strong>integer</strong> - ciparu, veselo skaitļu ievadīšana;</li>
      <li><strong>decimal</strong> - ciparu, daļskaitļu ievadīšanai;</li>
      <li><strong>gpslat</strong> - koordinātes platuma daļas ievades maska;</li>
      <li><strong>gpslon</strong> - koordinātes garuma daļas ievades maska;</li>
      <li><strong>currency</strong> - eiro vērtības ievades maska;</li>
      <li><strong>personCodeLv</strong> - Latvijas personas koda ievades maska;</li>
      <li><strong>newbornId</strong> - Latvijas jaundzmušā koda ievades maska;</li>
    </ul>
  </article>
</template>
