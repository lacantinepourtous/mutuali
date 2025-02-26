<template>
  <div class="availability">
    <div class="availability__calendar">
      <b-calendar
        v-model="activeCalendarDate"
        v-bind="calendarLabels || {}"
        class="border rounded pt-2"
        today-variant="today"
        selected-variant="active-date"
        :locale="locale"
        :start-weekday="1"
        block
        hide-header
        :date-info-fn="addAvailableDateClasses"
        :date-disabled-fn="setDisabledDates"
        @context="updateCalendarContext"
      ></b-calendar>
    </div>

    <div v-if="activeCalendarDate" class="availability__period">
      <p class="availability__period-title font-weight-bold mb-2">
        <span v-if="locale === 'fr-CA'" class="text-capitalize">{{ formattedCalendarDay }}</span>
        {{ formattedCalendarDate }}
      </p>
      <fieldset class="availability__period-fieldset">
        <legend class="sr-only">Choississez une période pour la date sélectionnée</legend>
        <b-checkbox
          v-if="checkPeriodAvailability(calendarContext.selectedDate, DAY)"
          class="availability__period-checkbox"
          value="day"
        >
          <svg xmlns="http://www.w3.org/2000/svg" width="17" height="16" viewBox="0 0 17 16" fill="none" aria-hidden="true">
            <path
              d="M4 8C4 6.80653 4.47411 5.66193 5.31802 4.81802C6.16193 3.97411 7.30653 3.5 8.5 3.5C9.69347 3.5 10.8381 3.97411 11.682 4.81802C12.5259 5.66193 13 6.80653 13 8C13 9.19347 12.5259 10.3381 11.682 11.182C10.8381 12.0259 9.69347 12.5 8.5 12.5C7.30653 12.5 6.16193 12.0259 5.31802 11.182C4.47411 10.3381 4 9.19347 4 8Z"
              fill="#F4B42B"
            />
            <path
              fill-rule="evenodd"
              clip-rule="evenodd"
              d="M8.70205 0.280439C8.67885 0.24862 8.64846 0.222732 8.61336 0.204885C8.57825 0.187037 8.53943 0.177734 8.50005 0.177734C8.46067 0.177734 8.42185 0.187037 8.38675 0.204885C8.35164 0.222732 8.32125 0.24862 8.29805 0.280439L7.38805 1.53544C7.35116 1.58676 7.2962 1.6222 7.23423 1.63463C7.17226 1.64706 7.10788 1.63556 7.05405 1.60244L5.73205 0.790439C5.69839 0.769767 5.66031 0.757366 5.62093 0.754251C5.58155 0.751137 5.542 0.757398 5.50551 0.772522C5.46902 0.787645 5.43663 0.811201 5.411 0.841257C5.38537 0.871314 5.36722 0.907017 5.35805 0.945439L4.99805 2.45344C4.98367 2.51484 4.94662 2.56853 4.89433 2.60377C4.84203 2.639 4.77835 2.65317 4.71605 2.64344L3.18405 2.39844C3.14509 2.39227 3.10522 2.39542 3.06771 2.40761C3.03019 2.4198 2.9961 2.4407 2.9682 2.46859C2.94031 2.49648 2.91941 2.53058 2.90722 2.56809C2.89503 2.60561 2.89189 2.64548 2.89805 2.68444L3.14205 4.21644C3.15182 4.2786 3.13779 4.34216 3.10276 4.39444C3.06772 4.44671 3.01426 4.48385 2.95305 4.49844L1.44405 4.85844C1.40572 4.86773 1.37012 4.88597 1.34018 4.91165C1.31024 4.93733 1.28681 4.96974 1.27179 5.00621C1.25677 5.04269 1.25059 5.0822 1.25377 5.12152C1.25694 5.16084 1.26938 5.19885 1.29005 5.23244L2.10205 6.55444C2.13488 6.60818 2.14621 6.67233 2.13379 6.73406C2.12137 6.7958 2.08611 6.85057 2.03505 6.88744L0.779052 7.79744C0.747038 7.82062 0.720975 7.85105 0.703003 7.88625C0.68503 7.92146 0.675659 7.96042 0.675659 7.99994C0.675659 8.03946 0.68503 8.07842 0.703003 8.11362C0.720975 8.14882 0.747038 8.17926 0.779052 8.20244L2.03505 9.11244C2.08637 9.14933 2.12181 9.20429 2.13424 9.26626C2.14667 9.32823 2.13517 9.39261 2.10205 9.44644L1.29005 10.7684C1.26938 10.802 1.25694 10.84 1.25377 10.8794C1.25059 10.9187 1.25677 10.9582 1.27179 10.9947C1.28681 11.0311 1.31024 11.0635 1.34018 11.0892C1.37012 11.1149 1.40572 11.1331 1.44405 11.1424L2.95405 11.5024C3.01508 11.5172 3.0683 11.5545 3.10313 11.6067C3.13797 11.659 3.15186 11.7224 3.14205 11.7844L2.89805 13.3164C2.89189 13.3554 2.89503 13.3953 2.90722 13.4328C2.91941 13.4703 2.94031 13.5044 2.9682 13.5323C2.9961 13.5602 3.03019 13.5811 3.06771 13.5933C3.10522 13.6055 3.14509 13.6086 3.18405 13.6024L4.71605 13.3584C4.77822 13.3487 4.84178 13.3627 4.89405 13.3977C4.94632 13.4328 4.98346 13.4862 4.99805 13.5474L5.35805 15.0554C5.36722 15.0939 5.38537 15.1296 5.411 15.1596C5.43663 15.1897 5.46902 15.2132 5.50551 15.2284C5.542 15.2435 5.58155 15.2497 5.62093 15.2466C5.66031 15.2435 5.69839 15.2311 5.73205 15.2104L7.05405 14.3984C7.10779 14.3656 7.17194 14.3543 7.23368 14.3667C7.29541 14.3791 7.35019 14.4144 7.38705 14.4654L8.29705 15.7214C8.32023 15.7535 8.35067 15.7795 8.38587 15.7975C8.42107 15.8155 8.46003 15.8248 8.49955 15.8248C8.53907 15.8248 8.57803 15.8155 8.61324 15.7975C8.64844 15.7795 8.67887 15.7535 8.70205 15.7214L9.61205 14.4654C9.64894 14.4141 9.70391 14.3787 9.76587 14.3662C9.82784 14.3538 9.89222 14.3653 9.94605 14.3984L11.2681 15.2104C11.3017 15.2311 11.3398 15.2435 11.3792 15.2466C11.4185 15.2497 11.4581 15.2435 11.4946 15.2284C11.5311 15.2132 11.5635 15.1897 11.5891 15.1596C11.6147 15.1296 11.6329 15.0939 11.6421 15.0554L12.0021 13.5474C12.0164 13.486 12.0535 13.4323 12.1058 13.3971C12.1581 13.3619 12.2217 13.3477 12.2841 13.3574L13.8161 13.6024C13.855 13.6086 13.8949 13.6055 13.9324 13.5933C13.9699 13.5811 14.004 13.5602 14.0319 13.5323C14.0598 13.5044 14.0807 13.4703 14.0929 13.4328C14.1051 13.3953 14.1082 13.3554 14.1021 13.3164L13.8581 11.7844C13.8483 11.7223 13.8623 11.6587 13.8973 11.6064C13.9324 11.5542 13.9858 11.517 14.0471 11.5024L15.5551 11.1424C15.5935 11.1333 15.6292 11.1151 15.6592 11.0895C15.6893 11.0639 15.7128 11.0315 15.728 10.995C15.7431 10.9585 15.7494 10.9189 15.7462 10.8796C15.7431 10.8402 15.7307 10.8021 15.7101 10.7684L14.8981 9.44644C14.8652 9.3927 14.8539 9.32855 14.8663 9.26681C14.8787 9.20508 14.914 9.1503 14.9651 9.11344L16.2211 8.20344C16.2531 8.18026 16.2791 8.14982 16.2971 8.11462C16.3151 8.07942 16.3244 8.04046 16.3244 8.00094C16.3244 7.96142 16.3151 7.92246 16.2971 7.88725C16.2791 7.85205 16.2531 7.82162 16.2211 7.79844L14.9651 6.88844C14.9137 6.85155 14.8783 6.79659 14.8659 6.73462C14.8534 6.67265 14.8649 6.60827 14.8981 6.55444L15.7101 5.23244C15.7307 5.19878 15.7431 5.1607 15.7462 5.12132C15.7494 5.08194 15.7431 5.04239 15.728 5.00589C15.7128 4.9694 15.6893 4.93701 15.6592 4.91138C15.6292 4.88575 15.5935 4.86761 15.5551 4.85844L14.0471 4.49844C13.9857 4.48406 13.932 4.44701 13.8967 4.39472C13.8615 4.34242 13.8473 4.27874 13.8571 4.21644L14.1021 2.68444C14.1082 2.64548 14.1051 2.60561 14.0929 2.56809C14.0807 2.53058 14.0598 2.49648 14.0319 2.46859C14.004 2.4407 13.9699 2.4198 13.9324 2.40761C13.8949 2.39542 13.855 2.39227 13.8161 2.39844L12.2841 2.64244C12.2219 2.65221 12.1583 2.63818 12.1061 2.60314C12.0538 2.56811 12.0166 2.51465 12.0021 2.45344L11.6421 0.945439C11.6329 0.907017 11.6147 0.871314 11.5891 0.841257C11.5635 0.811201 11.5311 0.787645 11.4946 0.772522C11.4581 0.757398 11.4185 0.751137 11.3792 0.754251C11.3398 0.757366 11.3017 0.769767 11.2681 0.790439L9.94605 1.60244C9.89231 1.63526 9.82816 1.6466 9.76643 1.63418C9.70469 1.62176 9.64992 1.58649 9.61305 1.53544L8.70305 0.280439H8.70205ZM8.50005 2.50044C7.04136 2.50044 5.64241 3.0799 4.61096 4.11135C3.57951 5.1428 3.00005 6.54175 3.00005 8.00044C3.00005 9.45913 3.57951 10.8581 4.61096 11.8895C5.64241 12.921 7.04136 13.5004 8.50005 13.5004C9.95874 13.5004 11.3577 12.921 12.3891 11.8895C13.4206 10.8581 14.0001 9.45913 14.0001 8.00044C14.0001 6.54175 13.4206 5.1428 12.3891 4.11135C11.3577 3.0799 9.95874 2.50044 8.50005 2.50044Z"
              fill="#F4B42B"
            />
          </svg>
          {{ $t("label.ad-dayAvailability") }}
        </b-checkbox>
        <b-checkbox
          v-if="checkPeriodAvailability(calendarContext.selectedDate, EVENING)"
          class="availability__period-checkbox"
          value="evening"
        >
          <svg xmlns="http://www.w3.org/2000/svg" width="17" height="16" viewBox="0 0 17 16" fill="none">
            <path
              fill-rule="evenodd"
              clip-rule="evenodd"
              d="M15.03 10.5297C13.7644 11.0207 12.3833 11.1331 11.0549 10.8533C9.72653 10.5734 8.5082 9.91331 7.54828 8.95339C6.58835 7.99347 5.92826 6.77513 5.6484 5.44676C5.36853 4.11839 5.48095 2.7373 5.97199 1.47168C4.45176 2.06413 3.18611 3.1688 2.39356 4.59496C1.60101 6.02113 1.33131 7.67927 1.63102 9.28309C1.93073 10.8869 2.78104 12.3358 4.03513 13.3795C5.28923 14.4232 6.8684 14.9962 8.49999 14.9997C9.91225 15 11.2916 14.5734 12.457 13.7756C13.6224 12.9779 14.5193 11.8464 15.03 10.5297Z"
              fill="#34A49A"
            />
          </svg>
          {{ $t("label.ad-eveningAvailability") }}
        </b-checkbox>
      </fieldset>
    </div>
  </div>
</template>

<script>
import { DAY, EVENING } from "@/consts/periods";
import i18nHelpers from "@/helpers/i18n";

import { AvailabilityWeekday } from "@/mixins/availability-weekday";

export default {
  mixins: [AvailabilityWeekday],
  props: {
    availability: {
      type: Array,
      required: true
    }
  },
  data() {
    return {
      activeCalendarDate: "",
      calendarContext: null,
      calendarLabels: {
        labelPrevDecade: this.$t("calendar.labelPrevDecade"),
        labelPrevYear: this.$t("calendar.labelPrevYear"),
        labelPrevMonth: this.$t("calendar.labelPrevMonth"),
        labelCurrentMonth: this.$t("calendar.labelCurrentMonth"),
        labelNextMonth: this.$t("calendar.labelNextMonth"),
        labelNextYear: this.$t("calendar.labelNextYear"),
        labelNextDecade: this.$t("calendar.labelNextDecade"),
        labelToday: this.$t("calendar.labelToday"),
        labelSelected: this.$t("calendar.labelSelected"),
        labelNoDateSelected: this.$t("calendar.labelNoDateSelected"),
        labelCalendar: this.$t("calendar.labelCalendar"),
        labelNav: this.$t("calendar.labelNav"),
        labelHelp: this.$t("calendar.labelHelp")
      },
      DAY,
      EVENING
    };
  },
  computed: {
    locale: function () {
      return i18nHelpers.locale() === "fr" ? "fr-CA" : "en-CA";
    },
    formattedCalendarDay() {
      return this.$format.date(this.activeCalendarDate, "dddd,");
    },
    formattedCalendarDate() {
      if (this.locale === "fr-CA") {
        return this.$format.date(this.activeCalendarDate, "D MMMM YYYY");
      } else {
        return this.$format.date(this.activeCalendarDate, "dddd, MMMM D, YYYY");
      }
    }
  },
  methods: {
    updateCalendarContext(context) {
      this.calendarContext = context;
    },
    checkPeriodAvailability(date, period) {
      if (!date) return false;
      const dayIndex = date.getDay() === 0 ? 6 : date.getDay() - 1;
      const dayValue = this.availabilityWeekdayOptions[dayIndex].value;
      return this.availability.some((availability) => availability.key === dayValue && availability.availability[period]);
    },
    addAvailableDateClasses(ymd, date) {
      let classes = "";
      if (this.checkPeriodAvailability(date, DAY)) {
        classes += "available-day";
      }
      if (this.checkPeriodAvailability(date, EVENING)) {
        classes += " available-evening";
      }
      return classes;
    },
    setDisabledDates(ymd, date) {
      const dayIndex = date.getDay() === 0 ? 6 : date.getDay() - 1;
      const dayValue = this.availabilityWeekdayOptions[dayIndex].value;
      return (
        !this.availability.some((availability) => availability.key === dayValue && availability.availability.day) &&
        !this.availability.some((availability) => availability.key === dayValue && availability.availability.evening)
      );
    }
  }
};
</script>

<style lang="scss">
.availability {
  & {
    @include media-breakpoint-up(sm) {
      display: grid;
      grid-template-columns: 2fr 1fr;
      column-gap: $spacer;
    }
  }

  &__calendar {
    .available-day,
    .available-evening {
      &:before,
      &:after {
        display: block;
        width: 6px;
        height: 6px;
        border-radius: 100%;
        position: absolute;
        bottom: 10px;
        left: 50%;
      }
    }

    .available-day {
      &:before {
        content: "";
        background-color: $yellow;
        transform: translate(-50%, 0);
      }
    }

    .available-evening {
      &:after {
        content: "";
        background-color: $green;
        transform: translate(-50%, 0);
      }
    }

    .b-calendar {
      .b-calendar-grid {
        border: 0;
        border-top-left-radius: 0;
        border-top-right-radius: 0;
        border-top: 1px solid $gray-300;
      }

      .b-calendar-grid-body {
        padding-top: 6px;
        padding-bottom: 6px;

        .col[data-date] {
          &.available-day.available-evening {
            &:before {
              transform: translate(calc(-50% - 5px), 0);
            }
            &:after {
              transform: translate(calc(-50% + 5px), 0);
            }
          }

          &[aria-disabled="true"] {
            background-color: $white !important;

            .btn {
              background-color: $gray-100;
            }
          }

          .btn {
            min-width: 65%;
            padding-top: 8px;
            padding-bottom: 20px;
            height: 42px;
            border-radius: 4px !important;
          }

          .btn-outline-today {
            border: 1px solid $primary !important;
          }

          .btn-active-date {
            background-color: $primary;
            color: $white;
          }
        }
      }
    }
  }

  &__period {
    margin-top: $spacer;
    @include media-breakpoint-up(sm) {
      margin-top: 0;
    }
  }

  &__period-title {
    text-align: center;
    @include media-breakpoint-up(sm) {
      text-align: left;
    }
  }

  &__period-fieldset {
    display: grid;
    grid-template-columns: 1fr 1fr;
    column-gap: $spacer / 2;
    @include media-breakpoint-up(sm) {
      display: block;
    }
  }

  &__period-checkbox {
    position: relative;
    width: 100%;
    padding: $spacer * 1.5 $spacer;
    border-radius: 4px;
    border: 1px solid $gray-300;
    margin-bottom: $spacer / 2;

    .custom-control-input {
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      width: 100%;
      height: 100%;
    }

    .custom-control-label {
      position: absolute;
      top: 50%;
      left: 50%;
      transform: translate(calc(-50% + 8px), -50%);
      display: flex;
      align-items: center;
      column-gap: $spacer / 4;
    }
  }
}
</style>
