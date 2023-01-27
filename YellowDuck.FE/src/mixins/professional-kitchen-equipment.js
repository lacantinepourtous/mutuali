import {
  PROFESSIONAL_KITCHEN_EQUIPMENT_STOVE,
  PROFESSIONAL_KITCHEN_EQUIPMENT_OVEN,
  PROFESSIONAL_KITCHEN_EQUIPMENT_WORKPLAN,
  PROFESSIONAL_KITCHEN_EQUIPMENT_MICROWAVES,
  PROFESSIONAL_KITCHEN_EQUIPMENT_SINK_DISHWASHER,
  PROFESSIONAL_KITCHEN_EQUIPMENT_COOKING_TOOLS,
  PROFESSIONAL_KITCHEN_EQUIPMENT_DRY_STORAGE_SPACE,
  PROFESSIONAL_KITCHEN_EQUIPMENT_FRIDGE,
  PROFESSIONAL_KITCHEN_EQUIPMENT_COLD_ROOM,
  PROFESSIONAL_KITCHEN_EQUIPMENT_FREEZER,
  PROFESSIONAL_KITCHEN_EQUIPMENT_OTHER
} from "@/consts/professional-kitchen-equipment";

export const ProfessionalKitchenEquipment = {
  data() {
    return {
      professionalKitchenEquipmentOptions: [
        { value: PROFESSIONAL_KITCHEN_EQUIPMENT_STOVE, text: this.$t("select.professional-kitchen-equipment-STOVE") },
        { value: PROFESSIONAL_KITCHEN_EQUIPMENT_OVEN, text: this.$t("select.professional-kitchen-equipment-OVEN") },
        { value: PROFESSIONAL_KITCHEN_EQUIPMENT_WORKPLAN, text: this.$t("select.professional-kitchen-equipment-WORKPLAN") },
        { value: PROFESSIONAL_KITCHEN_EQUIPMENT_MICROWAVES, text: this.$t("select.professional-kitchen-equipment-MICROWAVES") },
        {
          value: PROFESSIONAL_KITCHEN_EQUIPMENT_SINK_DISHWASHER,
          text: this.$t("select.professional-kitchen-equipment-SINK_DISHWASHER")
        },
        {
          value: PROFESSIONAL_KITCHEN_EQUIPMENT_COOKING_TOOLS,
          text: this.$t("select.professional-kitchen-equipment-COOKING_TOOLS")
        },
        {
          value: PROFESSIONAL_KITCHEN_EQUIPMENT_DRY_STORAGE_SPACE,
          text: this.$t("select.professional-kitchen-equipment-DRY_STORAGE_SPACE")
        },
        { value: PROFESSIONAL_KITCHEN_EQUIPMENT_FRIDGE, text: this.$t("select.professional-kitchen-equipment-FRIDGE") },
        { value: PROFESSIONAL_KITCHEN_EQUIPMENT_COLD_ROOM, text: this.$t("select.professional-kitchen-equipment-COLD_ROOM") },
        { value: PROFESSIONAL_KITCHEN_EQUIPMENT_FREEZER, text: this.$t("select.professional-kitchen-equipment-FREEZER") },
        { value: PROFESSIONAL_KITCHEN_EQUIPMENT_OTHER, text: this.$t("select.professional-kitchen-equipment-OTHER") }
      ]
    };
  },
  methods: {
    getProfessionalKitchenEquipmentLabel: function(equipment) {
      return this.professionalKitchenEquipmentOptions.filter((x) => x.value === equipment).first().text;
    }
  }
};
