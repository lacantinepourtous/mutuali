<template>
  <div class="conversation-date">
    <span class="conversation-date__label small">{{ getDate() }}</span>
  </div>
</template>

<script>
import { SHORT_DAY_MONTH_YEAR, SHORT_MONTH_DAY_YEAR } from "@/consts/formats";
import i18nHelpers from "@/helpers/i18n";

export default {
  props: {
    date: {
      type: Date,
      required: true
    }
  },
  computed: {
    getDate() {
      return (date) => {
        if (this.date.isToday()) {
          return i18nHelpers.instance().t("date.today");
        }

        if (i18nHelpers.locale() === "fr") {
          return i18nHelpers.getLocalizedDate(this.date, SHORT_DAY_MONTH_YEAR);
        } else {
          return i18nHelpers.getLocalizedDate(this.date, SHORT_MONTH_DAY_YEAR);
        }
      };
    }
  }
};
</script>

<style lang="scss">
.conversation-date {
  display: flex;
  align-items: center;
  margin-bottom: $spacer / 2;

  &::after,
  &::before {
    content: "";
    display: block;
    flex-grow: 1;
    width: 100%;
    height: 1px;
    background-color: $gray-200;
  }

  &__label {
    margin: 0 $spacer;
    text-transform: uppercase;
    color: $green;
    white-space: nowrap;
  }
}
</style>