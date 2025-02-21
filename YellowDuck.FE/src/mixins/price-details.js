import { AdRentalPriceRange } from "@/mixins/ad-rental-price-range";
import { AdSalePriceRange } from "@/mixins/ad-sale-price-range";

export const PriceDetails = {
  mixins: [AdRentalPriceRange, AdSalePriceRange],
  methods: {
    getPriceDetailsFromAd(ad) {
      const translation = ad.translationOrDefault || {};

      var priceDetails = {
        isAvailableForRent: ad.isAvailableForRent,
        isAvailableForSale: ad.isAvailableForSale,
        isAvailableForDonation: ad.isAvailableForDonation,
        isAvailableForTrade: ad.isAvailableForTrade,
        rentPriceDescription: ad.rentPriceToBeDetermined ? "" : translation.rentPriceDescription || "",
        salePriceDescription: ad.salePriceToBeDetermined ? "" : translation.salePriceDescription || "",
        donationDescription: translation.donationDescription || "",
        tradeDescription: translation.tradeDescription || "",
        rentPrice: ad.rentPriceToBeDetermined ? this.$t("price.toBeDetermined") : this.$format.formatMoney(ad.rentPrice),
        salePrice: ad.salePriceToBeDetermined ? this.$t("price.toBeDetermined") : this.$format.formatMoney(ad.salePrice),
        rentPriceRange:
          ad.rentPriceToBeDetermined && ad.rentPriceRange ? `(${this.getRentalPriceRangeLabel(ad.rentPriceRange)})` : "",
        salePriceRange:
          ad.salePriceToBeDetermined && ad.salePriceRange ? `(${this.getSalePriceRangeLabel(ad.salePriceRange)})` : "",
        rentPriceToBeDetermined: ad.rentPriceToBeDetermined,
        salePriceToBeDetermined: ad.salePriceToBeDetermined
      };

      return priceDetails;
    }
  }
};
