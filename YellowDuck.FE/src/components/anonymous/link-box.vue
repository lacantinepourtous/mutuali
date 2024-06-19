<template>
  <a class="link-box py-4" :href="link.href" target="_blank">
    <h3 class="link-box__title h4 my-0 mr-6">
      {{ link.title }}
    </h3>
    <svg class="link-box__arrow" width="18" height="14" viewBox="0 0 18 14" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M0.75 6.08301H14.958V7.91604H0.75V6.08301Z" />
      <path
        d="M11.0163 13.1419L9.73242 11.858L14.5913 6.99997L9.73242 2.14193L11.0163 0.858032L17.1574 6.99999L11.0163 13.1419Z"
      />
    </svg>
  </a>
</template>

<script>
export default {
  props: {
    link: {
      type: Object,
      required: true
    }
  }
};
</script>

<style lang="scss" scoped>
.link-box {
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: relative;
  border-bottom: solid 1px $gray-900;
  transition: border 0.2s ease-in-out;
  color: $gray-900;

  &:hover {
    text-decoration: none;
    border-bottom: solid 1px black;
    color: black;

    .link-box__title {
      color: black;
    }

    .link-box__arrow {
      animation: bounce-left 1s ease-in-out;
      animation-iteration-count: 3;

      @media (prefers-reduced-motion: reduce) {
        animation: none;
      }
    }
  }

  &__title {
    color: $gray-900;
    transition: color 0.2s ease-in-out;
  }

  &__arrow {
    flex-shrink: 0;
    fill: currentColor;
    transition: color 0.2s ease-in-out;
  }
}
</style>
