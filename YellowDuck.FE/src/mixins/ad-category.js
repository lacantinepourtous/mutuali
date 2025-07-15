import {
  CATEGORY_GROUP_EQUIPMENT,
  CATEGORY_GROUP_SPACE,
  CATEGORY_GROUP_WORKFORCE,
  CATEGORY_GROUP_OTHER
} from "@/consts/category-groups";

import {
  CATEGORY_PROFESSIONAL_KITCHEN,
  CATEGORY_DELIVERY_TRUCK,
  CATEGORY_STORAGE_SPACE,
  CATEGORY_PROFESSIONAL_COOKING_EQUIPMENT,
  CATEGORY_PREP_EQUIPMENT,
  CATEGORY_REFRIGERATION_EQUIPMENT,
  CATEGORY_HEAVY_EQUIPMENT,
  CATEGORY_SURPLUS,
  CATEGORY_HUMAN_RESOURCES,
  CATEGORY_SUBCONTRACTING,
  CATEGORY_OTHER
} from "@/consts/categories";

export const AdCategory = {
  data() {
    return {
      categoryGroupOptions: [
        {
          value: CATEGORY_GROUP_EQUIPMENT,
          text: this.$t("select.category-group-equipment"),
          shortText: this.$t("select.category-group-equipment")
        },
        {
          value: CATEGORY_GROUP_SPACE,
          text: this.$t("select.category-group-space"),
          shortText: this.$t("select.category-group-space")
        },
        {
          value: CATEGORY_GROUP_WORKFORCE,
          text: this.$t("select.category-group-workforce"),
          shortText: this.$t("select.category-group-workforce")
        },
        {
          value: CATEGORY_GROUP_OTHER,
          text: this.$t("select.category-group-other"),
          shortText: this.$t("select.category-group-other")
        }
      ],
      categoryOptions: [
        {
          value: CATEGORY_DELIVERY_TRUCK,
          text: this.$t("select.category-delivery-truck"),
          shortText: this.$t("select.category-delivery-truck"),
          titlePlaceholder: this.$t("placeholder.ad-title-delivery-truck")
        },
        {
          value: CATEGORY_PROFESSIONAL_COOKING_EQUIPMENT,
          text: this.$t("select.category-professional-cooking-equipment"),
          shortText: this.$t("select.category-professional-cooking-equipment-short"),
          titlePlaceholder: this.$t("placeholder.ad-title-professional-cooking-equipment")
        },
        {
          value: CATEGORY_PREP_EQUIPMENT,
          text: this.$t("select.category-prep-equipment"),
          shortText: this.$t("select.category-prep-equipment"),
          titlePlaceholder: this.$t("placeholder.ad-title-prep-equipment")
        },
        {
          value: CATEGORY_REFRIGERATION_EQUIPMENT,
          text: this.$t("select.category-refrigeration-equipment"),
          shortText: this.$t("select.category-refrigeration-equipment-short"),
          titlePlaceholder: this.$t("placeholder.ad-title-refrigeration-equipment")
        },
        {
          value: CATEGORY_HEAVY_EQUIPMENT,
          text: this.$t("select.category-heavy-equipment"),
          shortText: this.$t("select.category-heavy-equipment"),
          titlePlaceholder: this.$t("placeholder.ad-title-heavy-equipment")
        },
        {
          value: CATEGORY_PROFESSIONAL_KITCHEN,
          text: this.$t("select.category-professional-kitchen"),
          shortText: this.$t("select.category-professional-kitchen"),
          titlePlaceholder: this.$t("placeholder.ad-title-professional-kitchen")
        },
        {
          value: CATEGORY_STORAGE_SPACE,
          text: this.$t("select.category-storage-space"),
          shortText: this.$t("select.category-storage-space"),
          titlePlaceholder: this.$t("placeholder.ad-title-storage-space")
        },
        {
          value: CATEGORY_HUMAN_RESOURCES,
          text: this.$t("select.category-human-resources"),
          shortText: this.$t("select.category-human-resources"),
          titlePlaceholder: this.$t("placeholder.ad-title-human-resources")
        },
        {
          value: CATEGORY_SUBCONTRACTING,
          text: this.$t("select.category-subcontracting"),
          shortText: this.$t("select.category-subcontracting"),
          titlePlaceholder: this.$t("placeholder.ad-title-subcontracting")
        },
        {
          value: CATEGORY_SURPLUS,
          text: this.$t("select.category-surplus"),
          shortText: this.$t("select.category-surplus"),
          titlePlaceholder: this.$t("placeholder.ad-title-surplus")
        },
        {
          value: CATEGORY_OTHER,
          text: this.$t("select.category-other"),
          shortText: this.$t("select.category-other"),
          titlePlaceholder: this.$t("placeholder.ad-title-other")
        }
      ]
    };
  },
  methods: {
    getCategoryLabel: function (category) {
      return this.categoryOptions.filter((x) => x.value === category).first().text;
    },
    getCategoryShortLabel: function (category) {
      return this.categoryOptions.filter((x) => x.value === category).first().shortText;
    },
    getCategoryTitlePlaceholder: function (category) {
      return this.categoryOptions.filter((x) => x.value === category).first().titlePlaceholder;
    },
    getCategoryGroupByCategory: function (category) {
      switch (category) {
        case CATEGORY_DELIVERY_TRUCK:
        case CATEGORY_PROFESSIONAL_COOKING_EQUIPMENT:
        case CATEGORY_PREP_EQUIPMENT:
        case CATEGORY_REFRIGERATION_EQUIPMENT:
        case CATEGORY_HEAVY_EQUIPMENT:
          return CATEGORY_GROUP_EQUIPMENT;
        case CATEGORY_PROFESSIONAL_KITCHEN:
        case CATEGORY_STORAGE_SPACE:
          return CATEGORY_GROUP_SPACE;
        case CATEGORY_HUMAN_RESOURCES:
        case CATEGORY_SUBCONTRACTING:
          return CATEGORY_GROUP_WORKFORCE;
        case CATEGORY_SURPLUS:
        case CATEGORY_OTHER:
          return CATEGORY_GROUP_OTHER;
        default:
          "";
      }
    },
    getCategoryOptionsByCategoryGroup: function (categoryGroup) {
      return this.categoryOptions.filter((x) => this.getCategoryGroupByCategory(x.value) === categoryGroup);
    }
  }
};
