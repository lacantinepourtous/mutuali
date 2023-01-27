<template>
  <b-badge variant="info" pill class="text-white">{{ getCategoryLabel(category) }}</b-badge>
</template>

<script>
import { AdCategory } from "@/mixins/ad-category";

export default {
  mixins: [AdCategory],
  props: {
    category: String
  }
};
</script>