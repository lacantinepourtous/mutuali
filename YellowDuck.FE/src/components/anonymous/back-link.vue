<template>
  <div class="text-right my-4">
    <RouterLink :to="backLink" class="back-link">
      <svg
        class="back-link__arrow mr-1"
        width="18"
        height="14"
        viewBox="0 0 18 14"
        fill="currentColor"
        transform="rotate(180)"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path d="M0.75 6.08301H14.958V7.91604H0.75V6.08301Z" />
        <path
          d="M11.0163 13.1419L9.73242 11.858L14.5913 6.99997L9.73242 2.14193L11.0163 0.858032L17.1574 6.99999L11.0163 13.1419Z"
        />
      </svg>
      {{ $t("nav.back") }}
    </RouterLink>
  </div>
</template>

<script>
export default {
  props: {
    backLink: {
      type: [String, Object],
      default() {
        return { name: this.$consts.urls.URL_LANDING };
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.back-link {
  font-weight: 600;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  transition: color 0.2s ease-in-out;

  &:hover,
  &:focus,
  &:active {
    text-decoration: none;

    .back-link__arrow {
      transform: translate(-4px, 0) rotate(180deg);
    }
  }

  &__arrow {
    position: relative;
    width: 16px;
    transition: transform 0.2s ease;
  }
}
</style>
