<template>
  <div class="rm-child-margin">
    <div class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-description") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.description"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.qualifications)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-qualifications") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.qualifications"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.tasks)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-tasks") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.tasks"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.geographicCoverage)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-geographic-coverage") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.geographicCoverage"></div>
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
            qualifications: "",
            tasks: "",
            geographicCoverage: "",
            humanResourceFieldOther: ""
          },
        };
      }
    }
  },
  methods: {
    haveHtmlContent
  }
};
</script>
