import {
  CERTIFICATION_RESTAURANT_KITCHEN,
  CERTIFICATION_C1_KITCHEN,
  CERTIFICATION_HACCP,
  CERTIFICATION_GFSI
} from "@/consts/certifications";

export const Certification = {
  data() {
    return {
      certificationOptions: [
        { value: CERTIFICATION_RESTAURANT_KITCHEN, text: this.$t("select.certification-restaurant-kitchen") },
        { value: CERTIFICATION_C1_KITCHEN, text: this.$t("select.certification-c1-kitchen") },
        { value: CERTIFICATION_HACCP, text: this.$t("select.certification-haccp") },
        { value: CERTIFICATION_GFSI, text: this.$t("select.certification-gfsi") }
      ]
    };
  },

  methods: {
    getCertificationLabel: function (certification) {
      return this.certificationOptions.filter((x) => x.value === certification).first().text;
    }
  }
};
