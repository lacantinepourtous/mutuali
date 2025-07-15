<template>
  <div class="rm-child-margin">
    <s-form-select
      v-model="form.humanResourceField"
      @input="$emit('input', value)"
      id="humanResourceField"
      :label="$t('label.ad-humanResourceField')"
      name="humanResourceField"
      rules="required"
      :placeholder="$t('placeholder.ad-humanResourceField')"
      :options="humanResourceFieldOptions"
      required
    />
    <s-form-input
      v-if="form.humanResourceField === HUMAN_RESOURCE_FIELD_OTHER"
      @input="$emit('input', value)"
      v-model="form.humanResourceFieldOther"
      id="humanResourceFieldOther"
      :label="$t('label.ad-humanResourceFieldOther')"
      name="humanResourceFieldOther"
      rules="required"
      :placeholder="$t('placeholder.ad-humanResourceFieldOther')"
      required
    />
    <s-form-rich-text-editor
      v-model="form.description"
      @input="$emit('input', value)"
      name="description"
      rules="emptyHtml"
      :label="$t('label.ad-description')"
      :description="$t('placeholder.ad-description-human-resources')"
      required
    />
    <s-form-rich-text-editor
      v-model="form.qualifications"
      @input="$emit('input', value)"
      name="qualifications"
      :label="$t('label.ad-qualifications')"
      :description="$t('placeholder.ad-qualifications')"
      :headingOn="false"
    />
    <s-form-rich-text-editor
      v-model="form.tasks"
      @input="$emit('input', value)"
      name="tasks"
      :label="$t('label.ad-tasks')"
      :description="$t('placeholder.ad-tasks')"
      :headingOn="false"
    />
    <s-form-rich-text-editor
      v-model="form.geographicCoverage"
      @input="$emit('input', value)"
      name="geographicCoverage"
      :label="$t('label.ad-geographicCoverage')"
      :description="$t('placeholder.ad-geographicCoverage')"
      :headingOn="false"
    />
  </div>
</template>

<script>
import SFormRichTextEditor from "@/components/form/s-form-rich-text-editor";
import SFormInput from "@/components/form/s-form-input";
import SFormSelect from "@/components/form/s-form-select";
import { AdHumanResourceField } from "@/mixins/ad-human-resource-field";
import { HUMAN_RESOURCE_FIELD_OTHER } from "@/consts/human-resource-field";

export default {
  mixins: [AdHumanResourceField],
  props: {
    value: {
      type: Object,
      default() {
        return {
          humanResourceField: "",
          humanResourceFieldOther: "",
          description: "",
          qualifications: "",
          tasks: "",
          geographicCoverage: ""
        };
      }
    }
  },
  data: function () {
    return {
      HUMAN_RESOURCE_FIELD_OTHER
    };
  },
  computed: {
    form() {
      return this.value;
    }
  },
  components: {
    SFormRichTextEditor,
    SFormInput,
    SFormSelect
  }
};
</script>