<template>
  <s-field
    :id="id"
    :label="label"
    :name="name"
    :input-id="inputId"
    :rules="rules"
    :description="description"
    v-slot="{ sState }"
    mode="aggressive"
  >
    <rich-text-editor
      v-model="computedValue"
      :inputId="inputId"
      :state="sState"
      :listOn="listOn"
      :linkOn="linkOn"
      :headingOn="headingOn"
    />
  </s-field>
</template>

<script>
import SField from "@/components/form/s-field";
import RichTextEditor from "@/components/tiptap-richtext/rich-text-editor";

export default {
  components: { SField, RichTextEditor },
  props: {
    id: String,
    label: String,
    name: String,
    rules: {
      type: [String, Object]
    },
    value: String,
    description: String,
    listOn: {
      type: Boolean,
      default: true
    },
    linkOn: {
      type: Boolean,
      default: true
    },
    headingOn: {
      type: Boolean,
      default: true
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
    },
    inputId() {
      return `input-${this.name}`;
    }
  }
};
</script>
