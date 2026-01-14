<!-- Use `lang="ts"` when using defineProps, otherwise it breaks vscode volar plugin -->
<!-- https://github.com/johnsoncodehk/volar/issues/353 -->
<script setup lang="ts">
defineProps({
  msg: {
    type: String,
  },
  msgDefault: {
    type: String,
    default: 'Hello World default value!',
  },
});
</script>
<template>
  <div>
    <div>
      d
      <div></div>
    </div>
  </div>
</template>
