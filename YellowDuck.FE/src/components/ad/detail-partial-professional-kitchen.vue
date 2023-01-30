<template>
  <div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.surfaceDescription)" class="section section--md section--border-top py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-surfaceDescription") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.surfaceDescription"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.description)" class="section section--md section--border-top py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-description") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.description"></div>
    </div>
    <div v-if="haveHtmlContent(ad.translationOrDefault.conditions)" class="section section--md section--border-top py-6">
      <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-conditions") }}</h2>
      <div class="rm-child-margin" v-html="ad.translationOrDefault.conditions"></div>
    </div>
    <div v-if="ad.professionalKitchenEquipment.length" class="section section--md section--border-top py-6">
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
  </div>
</template>

<script>
import { ProfessionalKitchenEquipment } from "@/mixins/professional-kitchen-equipment";
import { PROFESSIONAL_KITCHEN_EQUIPMENT_OTHER } from "@/consts/professional-kitchen-equipment";

export default {
  mixins: [ProfessionalKitchenEquipment],
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
          professionalKitchenEquipment: null
        };
      }
    }
  },
  data: function() {
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
