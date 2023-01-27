<template>
  <b-dropdown :id="id" :right="right" :text="label" :variant="variant" class="m-0 ui-dropdown">
    <b-dropdown-item v-for="(option, index) in computedOptions" :key="index" @click="selectOption(option)">{{
      option.text
    }}</b-dropdown-item>
  </b-dropdown>
</template>

<script>
export default {
  props: {
    id: String,
    label: String,
    options: Array,
    addDisableOptions: Boolean,
    placeholder: String,
    variant: {
      type: String,
      default: "primary"
    },
    right: Boolean
  },
  computed: {
    computedOptions: function () {
      let options = [...this.options];
      if (this.addDisableOptions || this.placeholder) {
        options.unshift({
          value: "",
          disabled: true,
          text: this.placeholder ? this.placeholder : this.$t("select.disabled-option")
        });
      }

      return options;
    }
  },
  methods: {
    selectOption: function (option) {
      this.$emit("input", option.value);
    }
  }
};
</script>

<style lang="scss">
$dropdownWidthMobile: 125px;
$dropdownWidth: 240px;

.ui-dropdown {
  .dropdown-toggle {
    width: $dropdownWidthMobile;
    white-space: nowrap;
    text-overflow: ellipsis;
    overflow: hidden;
    position: relative;
    padding-right: 32px;
    text-align: left;

    &:after {
      position: absolute;
      transform: translate(0, -50%);
      right: 12px;
      top: 50%;
    }
  }

  @include media-breakpoint-up(sm) {
    .dropdown-toggle {
      width: $dropdownWidth;
    }
  }
}

// Override bootstrap style for toggle btn when open
.btn-outline-secondary:not(:disabled):not(.disabled):active,
.show > .btn-outline-secondary.dropdown-toggle {
  background-color: $green-darker;
  color: $gray-100;
  border-color: $green-darker;
}
</style>