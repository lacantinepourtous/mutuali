<template>
  <div class="rm-child-margin">
    <s-form-rich-text-editor
      v-model="form.tasks"
      @input="$emit('input', value)"
      name="tasks"
      rules="emptyHtml"
      :label="$t('label.ad-tasks-subcontracting')"
      :description="$t('placeholder.ad-tasks-subcontracting')"
      required
    />
    <s-form-rich-text-editor
      v-model="form.description"
      @input="$emit('input', value)"
      name="description"
      :label="$t('label.ad-descriptionOther')"
      :description="$t('placeholder.ad-descriptionOther')"
    />
    <s-form-rich-text-editor
      v-model="form.conditions"
      @input="$emit('input', value)"
      name="conditions"
      :label="$t('label.ad-conditions')"
      :description="$t('placeholder.ad-conditions-subcontracting')"
      :headingOn="false"
    />
    <s-form-checkbox-group
      v-model="form.certification"
      id="certification"
      name="certification"
      :label="$t('section-title.certifications')"
      label-class="h2 mt-4 mb-2"
      :options="certificationOptions"
      @input="$emit('input', value)"
    />
    <s-form-checkbox-group
      v-model="form.allergen"
      id="allergen"
      :label="$t('label.ad-allergen')"
      label-class="h2 mt-4 mb-2"
      name="allergen"
      :options="allergenOptions"
    />
  </div>
</template>

<script>
import SFormRichTextEditor from "@/components/form/s-form-rich-text-editor";
import SFormCheckboxGroup from "@/components/form/s-form-checkbox-group";
import { Certification } from "@/mixins/certification";
import { Allergen } from "@/mixins/allergen";

export default {
  mixins: [Certification, Allergen],
  props: {
    value: {
      type: Object,
      default() {
        return {
          tasks: "",
          description: "",
          conditions: "",
          certification: [],
          allergen: []
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
    SFormCheckboxGroup
  }
};
</script>
