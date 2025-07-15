<template>
  <div class="rm-child-margin">
    <div class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-description") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.description"></div>
    </div>
    <div v-if="haveQualifications" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-qualifications") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.qualifications"></div>
    </div>
    <div v-if="haveTasks" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-tasks") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.tasks"></div>
    </div>
    <div v-if="haveGeographicCoverage" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-geographicCoverage") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.geographicCoverage"></div>
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
  computed: {
    haveQualifications() {
      let qualificationsEl = document.createElement("div");
      qualificationsEl.innerHTML = this.ad.translationOrDefault.qualifications;
      return qualificationsEl.textContent.trim() !== "";
    },
    haveTasks() {
      let tasksEl = document.createElement("div");
      tasksEl.innerHTML = this.ad.translationOrDefault.tasks;
      return tasksEl.textContent.trim() !== "";
    },
    haveGeographicCoverage() {
      let geographicCoverageEl = document.createElement("div");
      geographicCoverageEl.innerHTML = this.ad.translationOrDefault.geographicCoverage;
      return geographicCoverageEl.textContent.trim() !== "";
    }
  },
};
</script>
