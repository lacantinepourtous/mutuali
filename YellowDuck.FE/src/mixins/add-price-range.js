import {
    PRICE_RANGE_0,
    PRICE_RANGE_1,
    PRICE_RANGE_2,
    PRICE_RANGE_3,
    PRICE_RANGE_4       
  } from "@/consts/price-range";

  export const AdPriceRange = {
    data() {
      return {
        priceRangeOptions: [
          { value: PRICE_RANGE_0, text: this.$t("select.price-range-0") },
          { value: PRICE_RANGE_1, text: this.$t("select.price-range-100-500") },
          { value: PRICE_RANGE_2, text: this.$t("select.price-range-500-1000") },
          { value: PRICE_RANGE_3, text: this.$t("select.price-range-1000-2000") },
          { value: PRICE_RANGE_4, text: this.$t("select.price-range-2000-or-more") }
        ]
      };
    },
    methods: {
      getPriceRangeLabel: function(priceRange) {
        return this.priceRangeOptions.filter((x) => x.value === priceRange).first().text;
      }
    }
  };
  