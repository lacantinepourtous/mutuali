import {
  TRUCK_TYPE_VAN,
  TRUCK_TYPE_CUBE_12_FOOT,
  TRUCK_TYPE_CUBE_14_FOOT,
  TRUCK_TYPE_CUBE_16_FOOT_AND_MORE,
  TRUCK_TYPE_OTHER
} from "@/consts/delivery-truck-types";

export const AdDeliveryTruckType = {
  data() {
    return {
      deliveryTruckTypesOptions: [
        { value: TRUCK_TYPE_VAN, text: this.$t("select.delivery-truck-type-van") },
        { value: TRUCK_TYPE_CUBE_12_FOOT, text: this.$t("select.delivery-truck-type-cube-12") },
        { value: TRUCK_TYPE_CUBE_14_FOOT, text: this.$t("select.delivery-truck-type-cube-14") },
        { value: TRUCK_TYPE_CUBE_16_FOOT_AND_MORE, text: this.$t("select.delivery-truck-type-cube-16") },
        { value: TRUCK_TYPE_OTHER, text: this.$t("select.delivery-truck-type-other") }
      ]
    };
  },
  methods: {
    getDeliveryTruckTypeLabel: function(type) {
      return this.deliveryTruckTypesOptions.filter((x) => x.value === type).first().text;
    }
  }
};
