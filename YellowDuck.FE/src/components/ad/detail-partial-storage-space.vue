<template>
  <div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.description)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-description") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.description"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.surfaceSize)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-surfaceSize") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.surfaceSize"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.equipment)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-equipment") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.equipment"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.conditions)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-conditions") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.conditions"></div>
    </div>
  </div>
</template>

<script>
import haveHtmlContent from "@/helpers/have-html-content";

export default {
  props: {
    ad: {
      type: Object,
      default() {
        return {
          translationOrDefault: {
            description: "",
            surfaceSize: "",
            equipment: "",
            conditions: ""
          },
          professionalKitchenEquipment: null
        };
      }
    }
  },
  methods: {
    haveHtmlContent
  }
};
</script>
