import { getUserSettings } from '@/services/user';
import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useUserStore = defineStore('userSettingsStore', () => {
  const initSettings = {
    autocompletePreselected: null,
    autocompletePreselectedFunc: {
      id: null,
      name: null,
    },
  };
  const settings = ref({ ...initSettings });

  const reset = () => {
    settings.value = { ...initSettings };
  };

  async function fetchSettings() {
    try {
      const resp = await getUserSettings();
      if (resp.status === 200) {
        settings.value = {
          ...resp.data?.savedValues,
        };
      }
    } catch (err) {
      this.reset();
      throw err;
    }
    return settings.value;
  }

  return {
    settings,
    fetchSettings,
    reset,
  };
});
