<template>
  <div class="rm-child-margin">
    <s-form-rich-text-editor
      v-model="form.surfaceDescription"
      @input="$emit('input', value)"
      name="surfaceDescription"
      rules="emptyHtml"
      :label="$t('label.ad-surfaceDescription')"
      :description="$t('placeholder.ad-surfaceDescription')"
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
      :description="$t('placeholder.ad-conditions')"
      :headingOn="false"
    />
    <s-form-checkbox-group
      v-model="form.professionalKitchenEquipment"
      @input="$emit('input', value)"
      id="professionalKitchenEquipment"
      :label="$t('label.ad-professionalKitchenEquipment')"
      label-class="h2 mt-4 mb-2"
      name="professionalKitchenEquipment"
      :options="professionalKitchenEquipmentOptions"
      rules="required:true"
    />
    <s-form-input
      v-if="form.professionalKitchenEquipment.includes(PROFESSIONAL_KITCHEN_EQUIPMENT_OTHER)"
      v-model="form.professionalKitchenEquipmentOther"
      @input="$emit('input', value)"
      id="professionalKitchenEquipmentOther"
      :label="$t('label.ad-professionalKitchenEquipmentOther')"
      name="professionalKitchenEquipmentOther"
      rules="required"
      :placeholder="$t('placeholder.ad-professionalKitchenEquipmentOther')"
      required
    />
  </div>
</template>

<script>
import SFormRichTextEditor from "@/components/form/s-form-rich-text-editor";
import SFormInput from "@/components/form/s-form-input";
import SFormCheckboxGroup from "@/components/form/s-form-checkbox-group";
import { ProfessionalKitchenEquipment } from "@/mixins/professional-kitchen-equipment";
import { PROFESSIONAL_KITCHEN_EQUIPMENT_OTHER } from "@/consts/professional-kitchen-equipment";

export default {
  mixins: [ProfessionalKitchenEquipment],
  props: {
    value: {
      type: Object,
      default() {
        return {
          surfaceDescription: "",
          description: "",
          conditions: "",
          professionalKitchenEquipment: [],
          professionalKitchenEquipmentOther: ""
        };
      }
    }
  },
  data: function () {
    return {
      PROFESSIONAL_KITCHEN_EQUIPMENT_OTHER
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
    SFormCheckboxGroup
  }
};
</script>