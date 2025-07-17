<template>
  <div>
    <div class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-description") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.description"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.conditions)" class="border-top border-grey py-6">
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
  methods: {
    haveHtmlContent(content) {
      if (!content) return false;
      let divEl = document.createElement("div");
      divEl.innerHTML = content;
      return divEl.textContent.trim() !== "";
    }
  }
};
</script>
