<template>
  <s-field
    :id="id"
    :label="label"
    :name="name"
    :rules="rules"
    :description="description"
    v-slot="{ sState, sValidate }"
    mode="aggressive"
  >
    <b-form-file
      :id="`input-${name}`"
      v-model="computedValue"
      :placeholder="placeholder"
      :drop-placeholder="dropPlaceholder"
      :required="required"
      :state="sState"
      :accept="accept"
      :browse-text="$t('btn.browse-file')"
      @focusout.native="blur(sValidate)"
    ></b-form-file>
  </s-field>
</template>

<script>
import SField from "@/components/form/s-field";

export default {
  props: {
    id: String,
    label: String,
    name: String,
    rules: {
      type: [String, Object]
    },
    value: {
      type: File
    },
    placeholder: String,
    dropPlaceholder: String,
    required: Boolean,
    description: String,
    accept: String
  },
  components: {
    SField
  },
  methods: {
    blur(validate) {
      if (this.computedValue === null) {
        validate();
      }
    }
  },
  computed: {
    computedValue: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit("input", val);
      }
    }
  }
};
</script>

<style lang="scss">
.custom-file-label[data-browse] {
  cursor: pointer;

  &::after {
    transition: background-color 0.2s ease-in-out, color 0.2s ease-in-out;
  }

  &:hover {
    &::after {
      background-color: $green-darker;
      color: $gray-100;
    }
  }
}
</style>