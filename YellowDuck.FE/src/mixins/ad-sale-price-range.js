import {
  PRICE_RANGE_SALE_100,
  PRICE_RANGE_SALE_500,
  PRICE_RANGE_SALE_1000,
  PRICE_RANGE_SALE_2000,
  PRICE_RANGE_SALE_MORE
} from "@/consts/price-range-sale";

export const AdSalePriceRange = {
  data() {
    return {
      salePriceRangeOptions: [
        { value: PRICE_RANGE_SALE_100, text: this.$t("select.price-range-0-100") },
        { value: PRICE_RANGE_SALE_500, text: this.$t("select.price-range-100-500") },
        { value: PRICE_RANGE_SALE_1000, text: this.$t("select.price-range-500-1000") },
        { value: PRICE_RANGE_SALE_2000, text: this.$t("select.price-range-1000-2000") },
        { value: PRICE_RANGE_SALE_MORE, text: this.$t("select.price-range-2000-or-more") }
      ]
    };
  },
  methods: {
    getSalePriceRangeLabel: function (priceRange) {
      return this.salePriceRangeOptions.filter((x) => x.value === priceRange).first().text;
    }
  }
};
