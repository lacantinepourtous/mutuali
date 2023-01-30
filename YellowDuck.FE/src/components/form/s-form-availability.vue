<template>
  <div class="form-availability">
    <s-field labelClass="label" :id="id" :inputId="`input-show-${name}`" :name="name" :rules="rules" margin="none" class="mt-2">
      <b-form-checkbox :id="`input-show-${name}`" v-model="computedShown" :required="required">
        <span v-html="label" />
      </b-form-checkbox>
    </s-field>
    <s-field
      margin="none"
      class="mb-4"
      :id="`${id}-items`"
      :name="`${name}-items`"
      :label="specifyLabel"
      v-slot="{ sState }"
      v-if="computedShown"
      :rules="{ required: required }"
    >
      <b-form-checkbox-group
        class="items"
        :id="`input-${name}-items`"
        v-model="computedValue"
        :required="required"
        :options="options"
        :state="sState"
        stacked
      >
      </b-form-checkbox-group>
    </s-field>
  </div>
</template>

<script>
import SField from "@/components/form/s-field";

export default {
  props: {
    id: String,
    label: String,
    specifyLabel: String,
    specifyLabelSrOnly: Boolean,
    name: String,
    rules: {
      type: [String, Object]
    },
    options: {
      type: Array,
      default() {
        return null;
      }
    },
    value: Array,
    required: Boolean,
    preSelected: Boolean
  },
  components: {
    SField
  },
  computed: {
    computedValue: {
      get() {
        return this.value || [];
      },
      set(val) {
        this.$emit("input", val);
      }
    },
    computedShown: {
      get() {
        return this.shown || this.computedValue.length > 0;
      },
      set(val) {
        this.shown = val;
      }
    }
  },
  data: function () {
    return {
      shown: Array.isArray(this.value) ? this.value.length > 0 : false
    };
  },
  watch: {
    shown() {
      if (this.shown && this.preSelected) {
        this.computedValue = this.options.map((x) => x.value);
      } else {
        this.computedValue = [];
      }
    }
  }
};
</script>
<style lang="scss">
.form-availability {
  .items {
    margin-top: $spacer / 2;
    border-left: 3px solid $yellow;
    padding-left: $spacer;
    .custom-checkbox:not(:last-child) {
      margin-bottom: $spacer * 0.75;
    }
  }
}
</style>
