<template>
  <div>
    <s-form-rich-text-editor
      v-model="form.description"
      @input="$emit('input', value)"
      name="description"
      rules="emptyHtml"
      :label="$t('label.ad-description')"
      :description="$t('placeholder.ad-description')"
      required
    />
    <s-form-input
      v-model="form.surfaceSize"
      @input="$emit('input', value)"
      id="surfaceSize"
      :label="$t('label.ad-surfaceSize')"
      name="surfaceSize"
      rules="required"
      :placeholder="$t('placeholder.ad-surfaceSize')"
      required
    />
    <s-form-rich-text-editor
      v-model="form.equipment"
      @input="$emit('input', value)"
      name="equipment"
      rules="emptyHtml"
      :label="$t('label.ad-equipment')"
      :description="$t('placeholder.ad-equipment')"
      required
    />
    <s-form-rich-text-editor
      v-model="form.conditions"
      @input="$emit('input', value)"
      name="conditions"
      :label="$t('label.ad-conditions')"
      :description="$t('placeholder.ad-conditions')"
      :headingOn="false"
    />
  </div>
</template>

<script>
import SFormRichTextEditor from "@/components/form/s-form-rich-text-editor";
import SFormInput from "@/components/form/s-form-input";

export default {
  props: {
    value: {
      type: Object,
      default() {
        return {
          description: "",
          surfaceSize: "",
          equipment: "",
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
    SFormRichTextEditor,
    SFormInput
  }
};
</script>