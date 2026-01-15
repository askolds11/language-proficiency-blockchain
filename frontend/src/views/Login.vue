<script setup>
import { LxButton } from '@wntr/lx-ui';

</script>

<template>
  <LxButton
    :href="{ name: 'authorization' }"
    :label="'Authorize'"
    icon="arrow-right"
  />
</template>
