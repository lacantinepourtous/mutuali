<template>
  <div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.tasks)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-tasks-subcontracting") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.tasks"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.description)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-description") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.description"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.conditions)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-conditions") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.conditions"></div>
    </div>
    <div v-if="ad.certification.length" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("section-title.certifications") }}</h2>
      <div class="rm-child-margin">
        <ul class="equipment-detail__certifications-list">
          <li v-for="certification in ad.certification" :key="certification" class="equipment-detail__certifications-item">
            <b-img :src="require('@/assets/icons/checkmark-badge.svg')" alt="" height="20" block></b-img>
            {{ getCertificationLabel(certification) }}
          </li>
        </ul>
      </div>
    </div>
    <div v-if="ad.allergen.length" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-allergen") }}</h2>
      <div class="rm-child-margin">
        <ul>
          <li v-for="allergen in ad.allergen" :key="allergen" class="mb-3">
            {{ getAllergenLabel(allergen) }}
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script>
import { Allergen } from "@/mixins/allergen";
import { Certification } from "@/mixins/certification";
import haveHtmlContent from "@/helpers/have-html-content";

export default {
  mixins: [Certification, Allergen],
  props: {
    ad: {
      type: Object,
      default() {
        return {
          translationOrDefault: {
            tasks: "",
            description: "",
            conditions: "",
          },
          allergen: null,
          certification: null
        };
      }
    }
  },
  methods: {
    haveHtmlContent
  }
};
</script>

<style lang="scss">
.equipment-detail {
  &__certifications {
    &-list {
      list-style-type: none;
      padding-left: 0;
      display: flex;
      flex-wrap: wrap;
      gap: $spacer / 2;
    }

    &-item {
      display: flex;
      align-items: center;
      column-gap: $spacer / 2;
      padding: $spacer / 4 $spacer / 2;
      border: 1px solid $gray-300;
      border-radius: 8px;
    }
  }
}
</style>
