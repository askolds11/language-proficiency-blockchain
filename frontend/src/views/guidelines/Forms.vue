<script setup>
import { ref } from 'vue';
import { LxList } from '@wntr/lx-ui';

const navItems = ref([
  {
    id: 'anatomy',
    name: 'Anatomija',
    description: 'Formas uzbūve un tās satāvdaļas',
    href: { name: 'formsAnatomy' },
    icon: 'next',
  },
  {
    id: 'layout',
    name: 'Lauku izkārtojums',
    description: 'Formas lauku izkārtošana un struktūra',
    href: { name: 'formsLayout' },
    icon: 'next',
  },
  {
    id: 'index',
    name: 'Sadaļas',
    description: 'Sadaļas un satura rādītājs',
    href: { name: 'formsIndex' },
    icon: 'next',
  },
  {
    id: 'builder',
    name: 'Būvētājs',
    description: 'Deklaratīva formu būvēšana',
    href: { name: 'formBuilder' },
    icon: 'next',
  },
]);
</script>
<template>
  <div>
    <article class="lx-article">
      <p>
        Formas ir ļoti fleksibls un universāls rīks, strukturētas informācijas vienveidīgai
        attēlošanai un ievadei lietotājam draudzīgā veidā.
      </p>
      <p>
        Ļoti svarīgi nodrošināt, lai visā sistēmā formas "uzvedās" un izskatās lietotājam maksimāli
        paredzamā veidā. Zemāk ir aprakstīts šis veids.
      </p>
    </article>
    <LxList list-type="3" :items="navItems" />
  </div>
</template>
