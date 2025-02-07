import {
  CERTIFICATION_RESTAURANT_COOKING,
  CERTIFICATION_C1,
  CERTIFICATION_HACCP,
  CERTIFICATION_GFSI
} from "@/consts/certifications";

export const Certification = {
  data() {
    return {
      certificationOptions: [
        { value: CERTIFICATION_RESTAURANT_COOKING, text: this.$t("select.certification-restaurant-cooking") },
        { value: CERTIFICATION_C1, text: this.$t("select.certification-c1") },
        { value: CERTIFICATION_HACCP, text: this.$t("select.certification-haccp") },
        { value: CERTIFICATION_GFSI, text: this.$t("select.certification-gfsi") }
      ]
    };
  },

  methods: {
    getCertificationLabel: function(certification) {
      return this.certificationOptions.filter((x) => x.value === certification).first().text;
    }
  }

};
