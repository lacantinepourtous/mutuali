<template>
  <validation-observer v-slot="{ handleSubmit }" slim>
    <form @submit.prevent="handleSubmit(onSubmit)" novalidate>
      <slot></slot>
    </form>
  </validation-observer>
</template>

<script>
export default {
  methods: {
    onSubmit: function () {
      this.$emit("submit");
      return false;
    }
  }
};
</script>
