export const PriceDetails = {
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
        salePrice: ad.salePriceToBeDetermined ? this.$t("price.toBeDetermined") : this.$format.formatMoney(ad.salePrice)
      };

      return priceDetails;
    }
  }
};
