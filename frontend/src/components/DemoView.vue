<script setup>
import { ref, shallowRef } from 'vue';
import { LxButton, LxModal, LxToggle, LxForm, LxRow } from '@wntr/lx-ui';

const modal = ref();

const props = defineProps({
  label: {
    type: String,
    default: '',
  },
  showModalButton: {
    type: Boolean,
    default: true,
  },
  showValue: {
    type: Boolean,
    default: true,
  },
  gitLink: {
    type: String,
    default: '',
  },
  kind: {
    type: String,
    default: 'm',
  },
});

const openModal = () => {
  modal.value.open();
};

const showRegion = shallowRef(true);
const showMenu = shallowRef(false);

function toggleMenu() {
  showMenu.value = !showMenu.value;
}
function goToIssues() {
  window.open('https://git.zzdats.lv/lx/ui/issues', 'issues');
}
function goToGit() {
  window.open(props.gitLink, '_blank');
}
</script>

<template>
  <div class="demo-view">
    <div class="toolbar">
      <div class="group">
        <div class="group responsive-mobile">
          <div class="lx-divider"></div>
          <LxToggle label="Disabled" v-model="showRegion" />
          <LxButton v-if="showModalButton" icon="open" kind="ghost" @click="openModal" />
          <div class="lx-divider"></div>
          <LxButton
            icon="gitea"
            title="Pirmkods"
            icon-set="brand"
            kind="ghost"
            @click="goToGit"
            v-if="gitLink"
          />
          <LxButton icon="bug" title="Ziņot par kļūdu" kind="ghost" @click="goToIssues" />
        </div>
        <div class="lx-divider responsive"></div>
        <div class="responsive">
          <LxButton icon="menu" :active="showMenu" kind="ghost" @click="toggleMenu" />
        </div>
      </div>
    </div>
    <div class="scene" :class="[{ 'lx-region': showRegion }]">
      <div
        class="display"
        :class="[
          { 'lx-demo-display-m': kind === 'm' },
          { 'lx-demo-display-l': kind === 'l' },
          { 'lx-demo-display-s': kind === 's' },
        ]"
      >
        <div></div>
        <slot />
        <div></div>
      </div>
    </div>

    <div :class="[{ 'with-value': showValue }]">
      <div class="value" v-if="showValue">
        <lx-form :show-header="false" :show-footer="false" :column-count="2">
          <lx-row label="v-model" column-span="2">
            <slot name="value" />
          </lx-row>
        </lx-form>
      </div>
    </div>
    <div class="menu" :class="[{ open: showMenu }]">
      <div class="toolbar toolbar-mobile">
        <div class="group responsive-mobile">
          <div class="lx-divider"></div>
          <LxToggle label="Disabled" v-model="showRegion" />
          <LxButton v-if="showModalButton" icon="open" kind="ghost" @click="openModal" />
          <div class="lx-divider"></div>
          <LxButton
            icon="gitea"
            title="Pirmkods"
            icon-set="brand"
            kind="ghost"
            @click="goToGit"
            v-if="gitLink"
          />
          <LxButton icon="bug" title="Ziņot par kļūdu" kind="ghost" @click="goToIssues" />
        </div>
      </div>
      <slot name="menu" />
    </div>

    <LxModal ref="modal" :size="kind">
      <div class="modal-scene">
        <div class="modal-display">
          <slot />
        </div>
      </div>
    </LxModal>
  </div>
</template>
