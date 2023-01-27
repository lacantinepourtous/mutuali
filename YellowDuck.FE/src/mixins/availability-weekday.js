import { MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY } from "@/consts/day-of-week";

export const AvailabilityWeekday = {
  data() {
    return {
      availabilityWeekdayOptions: [
        { value: MONDAY, text: this.$t("select.availability-weekday-MONDAY") },
        { value: TUESDAY, text: this.$t("select.availability-weekday-TUESDAY") },
        { value: WEDNESDAY, text: this.$t("select.availability-weekday-WEDNESDAY") },
        { value: THURSDAY, text: this.$t("select.availability-weekday-THURSDAY") },
        { value: FRIDAY, text: this.$t("select.availability-weekday-FRIDAY") },
        { value: SATURDAY, text: this.$t("select.availability-weekday-SATURDAY") },
        { value: SUNDAY, text: this.$t("select.availability-weekday-SUNDAY") }
      ]
    };
  },
  methods: {
    getAvailabilityWeekdayLabel: function(weekday) {
      return this.availabilityWeekdayOptions.filter((x) => x.value === weekday).first().text;
    }
  }
};
