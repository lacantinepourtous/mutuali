<template>
  <div>
    <div class="section section--md py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-description") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.description"></div>
    </div>
    <div v-if="haveAdConditions" class="section section--md section--border-top py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-conditions") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.conditions"></div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    ad: {
      type: Object,
      default() {
        return {
          translationOrDefault: {
            conditions: "",
            description: ""
          }
        };
      }
    }
  },
  computed: {
    haveAdConditions() {
      let adConditionsEl = document.createElement("div");
      adConditionsEl.innerHTML = this.adConditions;
      return adConditionsEl.textContent.trim() !== "";
    }
  }
};
</script>
