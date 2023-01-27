<template>
  <picture>
    <source media="(min-width: 1440px)" :srcset="`${src}?mode=max&width=1920`" />
    <source media="(min-width: 1200px)" :srcset="`${src}?mode=max&width=1440`" />
    <source media="(min-width: 992px)" :srcset="`${src}?mode=max&width=992`" />
    <source media="(min-width: 768px)" :srcset="`${src}?mode=max&width=768`" />
    <source media="(min-width: 576px)" :srcset="`${src}?mode=max&width=576`" />
    <source :srcset="`${src}?mode=max&width=576`" />
    <b-img :src="`${src}?mode=max&width=1920`" :alt="alt" class="equipment-picture d-block" />
  </picture>
</template>

<script>
export default {
  props: {
    src: {
      type: String,
      required: true
    },
    alt: {
      type: String,
      default: ""
    }
  }
};
</script>

<style lang="scss">
.equipment-picture {
  position: absolute;
  width: 100%;
  height: 100%;
  object-fit: contain;
  object-position: center;
}
</style>
