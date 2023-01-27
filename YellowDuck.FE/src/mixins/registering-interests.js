import {
  REGISTERING_INTERESTS_EQUIPMENT_OFFERS,
  REGISTERING_INTERESTS_SHOW_EQUIPMENTS,
  REGISTERING_INTERESTS_GENERAL
} from "@/consts/registering-interests";

export const RegisteringInterests = {
  data() {
    return {
      registeringInterestOptions: [
        { value: REGISTERING_INTERESTS_EQUIPMENT_OFFERS, text: this.$t("select.registering-interests-equipment-offers") },
        { value: REGISTERING_INTERESTS_SHOW_EQUIPMENTS, text: this.$t("select.registering-interests-show-equipments") },
        { value: REGISTERING_INTERESTS_GENERAL, text: this.$t("select.registering-interests-general") }
      ]
    };
  },
  methods: {
    registeringInterestLabel: function(registeringInterest) {
      return this.registeringInterestOptions.filter((x) => x.value === registeringInterest).first().text;
    }
  }
};
