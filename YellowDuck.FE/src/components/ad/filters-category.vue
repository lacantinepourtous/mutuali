<template>
  <dropdown
    :id="id"
    :label="categoryLabel(category)"
    :options="dropdownCategoryOptions"
    right
    variant="outline-secondary"
    @input="$emit('update:category', $event)"
  />
</template>

<script>
import Dropdown from "@/components/generic/dropdown";

import { AdCategory } from "@/mixins/ad-category";

export default {
  mixins: [AdCategory],
  components: {
    Dropdown,
  },
  props: {
    id: {
      type: String,
      required: true
    },
    category: {
      type: String,
      default: null
    }
  },
  computed: {
    dropdownCategoryOptions() {
      const options = [
        { value: null, text: this.$t("select.all-equipment") }
      ];
      for (const group of this.categoryGroupOptions) {
        // add divider between group
        options.push({
          type: "divider"
        });

        for (const category of this.getCategoryOptionsByCategoryGroup(group.value)) {
          options.push({
            value: category.value,
            text: category.text,
            color: group.color
          });
        }
      }
      return options;
    }

  },
  methods: {
    categoryLabel(category) {
      const options = this.dropdownCategoryOptions.slice(1);
      const selectedOption = options.find((cat) => cat.value === category);
      if (!selectedOption) return this.$t("select.filter");
      return selectedOption.text;
    },
  },
};
</script>