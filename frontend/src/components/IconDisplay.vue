<script setup lang="ts">
import { LxIcon, LxSearchableText, lxStringUtils } from '@wntr/lx-ui';
import { computed, ref } from 'vue';

const props = defineProps({
  id: {
    type: String,
  },
  icon: {
    type: String,
  },
  description: {
    type: String,
  },
  hasCds: {
    type: Boolean,
    default: false,
  },
  hasBrand: {
    type: Boolean,
    default: false,
  },
  hasMaterial: {
    type: Boolean,
    default: false,
  },
  query: {
    type: String,
    default: '',
  },
});

const cdsIcon = ref(null);
const materialIcon = ref(null);
const brandIcon = ref(null);

const show = computed(() =>
  props.query && props.icon
    ? lxStringUtils.textSearch(props.query, `${props.icon} ${props.description}`)
    : true
);

function downloadIcon(iconName: string, iconSet: string)  {
 let svgElement: HTMLElement | null = null;

  switch(iconSet) {
    case 'cds':
      svgElement = cdsIcon.value.$el;
      break;
    case 'material':
      svgElement = materialIcon.value.$el;
      break;
    case 'brand':
      svgElement = brandIcon.value.$el;
      break;

    default:
      break;
  }

 if (svgElement) {
    const clonedSvgElement = svgElement.cloneNode(true) as HTMLElement;
    const svgContent = new XMLSerializer().serializeToString(clonedSvgElement);
    const svgBlob = new Blob([svgContent], { type: 'image/svg+xml' });
    const downloadLink = document.createElement('a');
    downloadLink.href = URL.createObjectURL(svgBlob);
    downloadLink.download = `${iconName}.svg`;
    downloadLink.click();
  }
};

const setIconTitle = (hasIcon) => {
  return hasIcon ? 'Lejupielādēt ikonu' : '';
};
</script>

<template>
  <div class="icon-display" :id="id" v-show="show">
    <div class="icon" :class="[{ 'lx-empty': !hasCds }]" :title="setIconTitle(hasCds)" >
      <LxIcon ref="cdsIcon" customClass="lx-icon" v-if="hasCds" :value="icon" icon-set="cds" @click="downloadIcon(icon, 'cds')"/>
      <p v-else class="lx-data">&mdash;</p>
    </div>
    <div class="icon" :class="[{ 'lx-empty': !hasMaterial }]"  :title="setIconTitle(hasMaterial)">
      <LxIcon ref="materialIcon" customClass="lx-icon" v-if="hasMaterial" :value="icon" icon-set="material" @click="downloadIcon(icon, 'material')" />
      <p v-else class="lx-data">&mdash;</p>
    </div>
    <div class="icon" :class="[{ 'lx-empty': !hasBrand }]" :title="setIconTitle(hasBrand)">
      <LxIcon ref="brandIcon" customClass="lx-icon" v-if="hasBrand" :value="icon" icon-set="brand" @click="downloadIcon(icon, 'brand')"/>
      <p v-else class="lx-data">&mdash;</p>
    </div>
    <div class="icon-name">
      <code><LxSearchableText :value="icon" :search-string="query" /></code>
    </div>
    <div class="icon-description">
      <p class="lx-secondary"><LxSearchableText :value="description" :search-string="query" /></p>
    </div>
  </div>
</template>
