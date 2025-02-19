import {
  ALLERGEN_NUTS,
  ALLERGEN_PEANUTS,
  ALLERGEN_SESAME,
  ALLERGEN_WHEAT,
  ALLERGEN_GLUTEN,
  ALLERGEN_SULPHITES,
  ALLERGEN_EGGS,
  ALLERGEN_MILK,
  ALLERGEN_SOY,
  ALLERGEN_FISH,
  ALLERGEN_MUSTARD
} from "@/consts/allergens";

export const Allergen = {
  data() {
    return {
      allergenOptions: [
        { value: ALLERGEN_NUTS, text: this.$t("select.allergen-nuts") },
        { value: ALLERGEN_PEANUTS, text: this.$t("select.allergen-peanuts") },
        { value: ALLERGEN_SESAME, text: this.$t("select.allergen-sesame") },
        { value: ALLERGEN_WHEAT, text: this.$t("select.allergen-wheat") },
        { value: ALLERGEN_GLUTEN, text: this.$t("select.allergen-gluten") },
        { value: ALLERGEN_SULPHITES, text: this.$t("select.allergen-sulphites") },
        { value: ALLERGEN_EGGS, text: this.$t("select.allergen-eggs") },
        { value: ALLERGEN_MILK, text: this.$t("select.allergen-milk") },
        { value: ALLERGEN_SOY, text: this.$t("select.allergen-soy") },
        { value: ALLERGEN_FISH, text: this.$t("select.allergen-fish") },
        { value: ALLERGEN_MUSTARD, text: this.$t("select.allergen-mustard") }
      ]
    };
  },

  methods: {
    getAllergenLabel: function (allergen) {
      return this.allergenOptions.filter((x) => x.value === allergen).first().text;
    }
  }
};
