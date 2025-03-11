import {
  PRICE_RANGE_RENTAL_100,
  PRICE_RANGE_RENTAL_500,
  PRICE_RANGE_RENTAL_1000,
  PRICE_RANGE_RENTAL_2000,
  PRICE_RANGE_RENTAL_MORE
} from "@/consts/price-range-rental";

export const AdRentalPriceRange = {
  data() {
    return {
      rentalPriceRangeOptions: [
        { value: PRICE_RANGE_RENTAL_100, text: this.$t("select.price-range-0-100") },
        { value: PRICE_RANGE_RENTAL_500, text: this.$t("select.price-range-100-500") },
        { value: PRICE_RANGE_RENTAL_1000, text: this.$t("select.price-range-500-1000") },
        { value: PRICE_RANGE_RENTAL_2000, text: this.$t("select.price-range-1000-2000") },
        { value: PRICE_RANGE_RENTAL_MORE, text: this.$t("select.price-range-2000-or-more") }
      ]
    };
  },
  methods: {
    getRentalPriceRangeLabel: function (priceRange) {
      var label = this.rentalPriceRangeOptions.filter((x) => x.value === priceRange).first();
      return label ? label.text : "";
    }
  }
};
