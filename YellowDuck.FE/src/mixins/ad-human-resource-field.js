import {
  HUMAN_RESOURCE_FIELD_OTHER,
  HUMAN_RESOURCE_FIELD_KITCHEN,
  HUMAN_RESOURCE_FIELD_MAINTENANCE,
  HUMAN_RESOURCE_FIELD_DELIVERY,
  HUMAN_RESOURCE_FIELD_WAREHOUSE,
  HUMAN_RESOURCE_FIELD_OFFICE,
  HUMAN_RESOURCE_FIELD_INVENTORY,
  HUMAN_RESOURCE_FIELD_COMMUNICATION,
  HUMAN_RESOURCE_FIELD_SEASONAL
} from "@/consts/human-resource-field";

export const AdHumanResourceField = {
  data() {
    return {
      humanResourceFieldOptions: [
        { value: HUMAN_RESOURCE_FIELD_KITCHEN, text: this.$t("select.human-resource-field-kitchen") },
        { value: HUMAN_RESOURCE_FIELD_MAINTENANCE, text: this.$t("select.human-resource-field-maintenance") },
        { value: HUMAN_RESOURCE_FIELD_DELIVERY, text: this.$t("select.human-resource-field-delivery") },
        { value: HUMAN_RESOURCE_FIELD_WAREHOUSE, text: this.$t("select.human-resource-field-warehouse") },
        { value: HUMAN_RESOURCE_FIELD_OFFICE, text: this.$t("select.human-resource-field-office") },
        { value: HUMAN_RESOURCE_FIELD_INVENTORY, text: this.$t("select.human-resource-field-inventory") },
        { value: HUMAN_RESOURCE_FIELD_COMMUNICATION, text: this.$t("select.human-resource-field-communication") },
        { value: HUMAN_RESOURCE_FIELD_SEASONAL, text: this.$t("select.human-resource-field-seasonal") },
        { value: HUMAN_RESOURCE_FIELD_OTHER, text: this.$t("select.human-resource-field-other") },
      ]
    };
  },
  methods: {
    getHumanResourceFieldLabel: function(field) {
      return this.humanResourceFieldOptions.filter((x) => x.value === field).first().text;
    }
  }
};
