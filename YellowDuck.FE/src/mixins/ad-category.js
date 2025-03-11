import {
  CATEGORY_PROFESSIONAL_KITCHEN,
  CATEGORY_DELIVERY_TRUCK,
  CATEGORY_STORAGE_SPACE,
  CATEGORY_PROFESSIONAL_COOKING_EQUIPMENT,
  CATEGORY_PREP_EQUIPMENT,
  CATEGORY_REFRIGERATION_EQUIPMENT,
  CATEGORY_HEAVY_EQUIPMENT,
  CATEGORY_SURPLUS,
  CATEGORY_OTHER
} from "@/consts/categories";

export const AdCategory = {
  data() {
    return {
      categoryOptions: [
        {
          value: CATEGORY_PROFESSIONAL_KITCHEN,
          text: this.$t("select.category-professional-kitchen"),
          shortText: this.$t("select.category-professional-kitchen")
        },
        {
          value: CATEGORY_DELIVERY_TRUCK,
          text: this.$t("select.category-delivery-truck"),
          shortText: this.$t("select.category-delivery-truck")
        },
        {
          value: CATEGORY_STORAGE_SPACE,
          text: this.$t("select.category-storage-space"),
          shortText: this.$t("select.category-storage-space")
        },
        {
          value: CATEGORY_PROFESSIONAL_COOKING_EQUIPMENT,
          text: this.$t("select.category-professional-cooking-equipment"),
          shortText: this.$t("select.category-professional-cooking-equipment-short")
        },
        {
          value: CATEGORY_PREP_EQUIPMENT,
          text: this.$t("select.category-prep-equipment"),
          shortText: this.$t("select.category-prep-equipment")
        },
        {
          value: CATEGORY_REFRIGERATION_EQUIPMENT,
          text: this.$t("select.category-refrigeration-equipment"),
          shortText: this.$t("select.category-refrigeration-equipment-short")
        },
        {
          value: CATEGORY_HEAVY_EQUIPMENT,
          text: this.$t("select.category-heavy-equipment"),
          shortText: this.$t("select.category-heavy-equipment")
        },
        {
          value: CATEGORY_SURPLUS,
          text: this.$t("select.category-surplus"),
          shortText: this.$t("select.category-surplus")
        },
        { value: CATEGORY_OTHER, text: this.$t("select.category-other"), shortText: this.$t("select.category-other") }
      ]
    };
  },
  methods: {
    getCategoryLabel: function (category) {
      return this.categoryOptions.filter((x) => x.value === category).first().text;
    },
    getCategoryShortLabel: function (category) {
      return this.categoryOptions.filter((x) => x.value === category).first().shortText;
    }
  }
};
