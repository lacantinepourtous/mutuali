<template>
  <b-dropdown :id="id" :right="right" :text="label" :variant="variant" class="m-0 ui-dropdown">
    <template v-for="(option, index) in computedOptions">
      <b-dropdown-divider v-if="option.type === 'divider'" :key="`${index}-divider`"></b-dropdown-divider>
      <b-dropdown-item v-else :key="index" :class="option.color" @click="selectOption(option)">
        <span v-if="option.color" class="dropdown-item-icon"></span> 
        <span>{{ option.text }}</span>
      </b-dropdown-item>
    </template>
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

  .dropdown-menu {
    min-width: 100%;
  }

  .dropdown-item {
    display: flex;
    align-items: center;
    white-space: normal;
  }

  .dropdown-item-icon {
    display: inline-block;
    width: 12px;
    height: 12px;
    border-radius: 50%;
    margin-right: 8px;
    background-color: var(--accent-color);
    flex-shrink: 0;
  }

  @include media-breakpoint-up(sm) {
    .dropdown-toggle {
      width: $dropdownWidth;
    }

    .dropdown-item {
      white-space: nowrap;
    }
  }
}

// Override bootstrap style for toggle btn when open
.btn-outline-secondary:not(:disabled):not(.disabled):active,
.show > .btn-outline-secondary.dropdown-toggle {
  background-color: $gray-900;
  color: $gray-100;
  border-color: $gray-900;
}
</style>