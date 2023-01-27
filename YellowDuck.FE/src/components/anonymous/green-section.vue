<template>
  <div class="green-lighter green-section w-full text-white">
    <div class="section section--lg">
      <slot></slot>
      <div v-if="$slots.form" class="green-section__form">
        <slot name="form"></slot>
      </div>
    </div>
  </div>
</template>

<style lang="scss">
$form-width: 440px;

.green-section {
  background-color: $green-dark;
  padding: $spacer * 2 0;

  @include media-breakpoint-up(sm) {
    padding: $spacer * 4 0;
  }

  @include media-breakpoint-up(md) {
    padding: $spacer * 8 0 $spacer * 6;
  }

  &__form {
    max-width: $form-width;
  }

  & .section > :first-child {
    margin-top: 0;
  }

  & .section > :last-child {
    margin-bottom: 0;
  }

  /* Exceptions de style pour ce bloc */

  h2 {
    margin-top: $spacer * 3;
    margin-bottom: $spacer * 1.5;
    color: $white;
  }

  ul {
    padding-left: $spacer * 1.5;
  }

  li {
    margin-bottom: $spacer;
  }

  .btn {
    margin: $spacer * 1 0;
  }

  .form-text .text-info {
    color: $white !important;
  }

  .invalid-feedback {
    border-radius: 4px;
    padding: 4px 8px;
    color: $white;
    background-color: $red;
  }
}
</style>
