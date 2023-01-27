<template>
  <a class="skip-link bg-white rounded text-primary" href="#main">
    {{ $t("btn.skip-to-content") }}
  </a>
</template>

<script>
export default {};
</script>

<style lang="scss" scoped>
.skip-link {
  display: inline-block;
  opacity: 0;
  left: 50%;
  padding: 0.5rem 1rem;
  position: absolute;
  transform: translate(-50%, -100%);
  z-index: 4;
  transition: opacity 0.2s ease-in-out, transform 0.2s ease-in-out;

  &:focus {
    transform: translate(-50%, 0);
    opacity: 1;
  }
}
</style>
