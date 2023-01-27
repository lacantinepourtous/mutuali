<template>
  <span>
    <span v-for="item in items" :key="item.text">
      <template v-if="item.to">
        <router-link v-if="item.to" :to="item.to" class="btn-link">{{ item.text }}</router-link> /
      </template>
      <span v-else class="text-muted">{{ item.text }}</span>
    </span>
  </span>
</template>

<script>
export default {
  props: {
    items: Array
  }
};
</script>