<script setup lang="ts">
import { ref, watch } from 'vue';
import useNotifyStore from '@/stores/useNotifyStore';
import { LxIcon } from '@wntr/lx-ui';

const notificationStore = useNotifyStore();
const currentNotification = ref(null);
const displayedNotifications = ref([]);

const deleteNotification = (uid) => {
  notificationStore.notifications = notificationStore.notifications.filter((n) => n.uid !== uid);
};

const displayNotification = (notification) => {
  if (notification && notification.autoCloseSeconds) {
    setTimeout(() => {
      deleteNotification(notification.uid);
    }, notification.autoCloseSeconds * 1000);
  }
};

function getIcon(type) {
  let ret;
  switch (type) {
    case 'warning':
      ret = 'notification-warning';
      break;
    case 'error':
      ret = 'notification-error';
      break;
    case 'success':
      ret = 'notification-success';
      break;
    case 'info':
    default:
      ret = 'notification-info';
      break;
  }
  return ret;
}

watch(
  () => notificationStore.notifications.length,
  () => {
    notificationStore.notifications.forEach((notification) => {
      displayNotification(notification);
    });
  },
  { immediate: true }
);
</script>

<template>
  <ul class="lx-notifications-list">
    <transition-group name="slide-left">
      <li
        :id="`notification-${notification.uid}`"
        v-for="notification in notificationStore.notifications"
        :key="notification.uid"
        class="lx-notification"
        :class="[
          { 'lx-notification-info': notification.type === 'info' },
          { 'lx-notification-success': notification.type === 'success' },
          { 'lx-notification-warning': notification.type === 'warning' },
          { 'lx-notification-error': notification.type === 'error' },
        ]"
        @click="deleteNotification(notification.uid)"
      >
        <div class="lx-main">
          <LxIcon :value="getIcon(notification.type)" />
          <div>
            <p class="lx-title">{{ notification.title }}</p>
            <p class="lx-subtitle" v-if="notification.subtitle">{{ notification.subtitle }}</p>
          </div>
        </div>
      </li>
    </transition-group>
  </ul>
</template>
