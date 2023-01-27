<template>
  <div>
    <s-field :id="id" :name="name" :rules="rules" margin="none" class="mt-4">
      <b-form-checkbox :id="`input-show-${name}`" v-model="computedShown" :required="required">
        <span v-html="label" />
      </b-form-checkbox>
    </s-field>
    <s-field
      margin="none"
      class="mb-4"
      :id="`${id}-items`"
      :name="`${name}-items`"
      v-slot="{ sState }"
      v-if="computedShown"
      rules="required"
    >
      <div v-html="specifyLabel" />
      <b-form-checkbox-group
        :id="`input-${name}-items`"
        v-model="computedValue"
        :required="true"
        :options="options"
        :state="sState"
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
    required: Boolean
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
        if (!val) this.$emit("input", []);
      }
    }
  },
  data: function () {
    return {
      shown: Array.isArray(this.value) ? this.value.length > 0 : false
    };
  }
};
</script>
