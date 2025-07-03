<template>
  <div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.surfaceDescription)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-surfaceDescription") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.surfaceDescription"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.description)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-description") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.description"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.conditions)" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-conditions") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.conditions"></div>
    </div>
    <div v-if="ad.professionalKitchenEquipment.length" class="border-top border-grey py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-equipments-inclusions") }}</h2>
      <div class="rm-child-margin">
        <ul>
          <li
            v-for="professionalKitchenEquipment in ad.professionalKitchenEquipment"
            :key="professionalKitchenEquipment"
            class="mb-3"
          >
            {{
              professionalKitchenEquipment == PROFESSIONAL_KITCHEN_EQUIPMENT_OTHER
                ? ad.translationOrDefault.professionalKitchenEquipmentOther
                : getProfessionalKitchenEquipmentLabel(professionalKitchenEquipment)
            }}
          </li>
        </ul>
      </div>
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
import { ProfessionalKitchenEquipment } from "@/mixins/professional-kitchen-equipment";
import { PROFESSIONAL_KITCHEN_EQUIPMENT_OTHER } from "@/consts/professional-kitchen-equipment";
import { Allergen } from "@/mixins/allergen";
import { Certification } from "@/mixins/certification";

export default {
  mixins: [ProfessionalKitchenEquipment, Certification, Allergen],
  props: {
    ad: {
      type: Object,
      default() {
        return {
          translationOrDefault: {
            surfaceDescription: "",
            description: "",
            conditions: "",
            professionalKitchenEquipmentOther: ""
          },
          professionalKitchenEquipment: null,
          certification: null
        };
      }
    }
  },
  data: function () {
    return {
      PROFESSIONAL_KITCHEN_EQUIPMENT_OTHER
    };
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
