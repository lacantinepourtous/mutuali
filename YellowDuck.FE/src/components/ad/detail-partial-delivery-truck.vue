<template>
  <div class="rm-child-margin">
    <div v-if="haveAdConditions" class="section section--md section--border-top py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-conditions") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.conditions"></div>
    </div>
    <div v-if="haveEquipmentsInclusions" class="section section--md section--border-top py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-equipments-inclusions") }}</h2>
      <div class="rm-child-margin">
        <ul>
          <li v-if="ad.deliveryTruckType" class="mb-3">
            {{
              ad.deliveryTruckType == TRUCK_TYPE_OTHER
                ? ad.translationOrDefault.deliveryTruckTypeOther
                : getDeliveryTruckTypeLabel(ad.deliveryTruckType)
            }}
          </li>
          <li v-if="ad.refrigerated" class="mb-3">{{ $t("label.ad-refrigerated") }}</li>
          <li v-if="ad.canHaveDriver" class="mb-3">{{ $t("label.ad-canHaveDriver") }}</li>
          <li v-if="ad.canSharedRoad">{{ $t("label.ad-canSharedRoad") }}</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script>
import { AdDeliveryTruckType } from "@/mixins/ad-delivery-truck-type";
import { TRUCK_TYPE_OTHER } from "@/consts/delivery-truck-types";

export default {
  mixins: [AdDeliveryTruckType],
  props: {
    ad: {
      type: Object,
      default() {
        return {
          translationOrDefault: {
            conditions: "",
            deliveryTruckTypeOther: ""
          },
          deliveryTruckType: null,
          refrigerated: false,
          canSharedRoad: false,
          canHaveDriver: false
        };
      }
    }
  },
  computed: {
    haveAdConditions() {
      let adConditionsEl = document.createElement("div");
      adConditionsEl.innerHTML = this.adConditions;
      return adConditionsEl.textContent.trim() !== "";
    },
    haveEquipmentsInclusions() {
      return this.ad.canHaveDriver || this.ad.canSharedRoad || this.ad.refrigerated || this.ad.deliveryTruckType;
    }
  },
  data: function () {
    return {
      TRUCK_TYPE_OTHER
    };
  }
};
</script>
