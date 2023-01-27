import {
  CATEGORY_PROFESSIONAL_KITCHEN,
  CATEGORY_DELIVERY_TRUCK,
  CATEGORY_STORAGE_SPACE,
  CATEGORY_OTHER
} from "@/consts/categories";

export const AdCategory = {
  data() {
    return {
      categoryOptions: [
        { value: CATEGORY_PROFESSIONAL_KITCHEN, text: this.$t("select.category-professional-kitchen") },
        { value: CATEGORY_DELIVERY_TRUCK, text: this.$t("select.category-delivery-truck") },
        { value: CATEGORY_STORAGE_SPACE, text: this.$t("select.category-storage-space") },
        { value: CATEGORY_OTHER, text: this.$t("select.category-other") }
      ]
    };
  },
  methods: {
    getCategoryLabel: function(category) {
      return this.categoryOptions.filter((x) => x.value === category).first().text;
    }
  }
};
