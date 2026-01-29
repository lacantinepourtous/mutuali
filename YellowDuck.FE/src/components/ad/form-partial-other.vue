<template>
  <div class="rm-child-margin">
    <s-form-rich-text-editor
      v-model="form.description"
      @input="$emit('input', value)"
      name="description"
      rules="emptyHtml"
      :label="$t('label.ad-description')"
      required
    />
    <s-form-rich-text-editor
      v-model="form.conditions"
      @input="$emit('input', value)"
      name="conditions"
      :label="$t('label.ad-conditions')"
      :headingOn="false"
    />
  </div>
</template>

<script>
import SFormRichTextEditor from "@/components/form/s-form-rich-text-editor";

export default {
  props: {
    value: {
      type: Object,
      default() {
        return {
          description: "",
          conditions: ""
        };
      }
    }
  },
  computed: {
    form() {
      return this.value;
    }
  },
  components: {
    SFormRichTextEditor
  }
};
</script>