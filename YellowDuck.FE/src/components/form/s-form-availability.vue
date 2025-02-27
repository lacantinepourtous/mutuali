<template>
  <fieldset class="form-availability" :id="id" :aria-labelledby="`${id}__legend`">
    <legend :id="`${id}__legend`" :class="legendClass || 'font-weight-bold h6 mt-4 mb-3'">{{ legend }}</legend>
    <div class="form-availability__all">
      <s-field labelClass="label" :id="`${id}-all`" :name="`${id}-all`" margin="none" class="mt-2">
        <b-form-checkbox
          :id="`input-${id}-all`"
          @input="(v) => updateAllAvailability(v)"
          :checked="allSelectedState"
          :indeterminate="indeterminateState"
        >
          {{ $t("label.ad-allAvailabilities") }}
        </b-form-checkbox>
      </s-field>
    </div>
    <ul class="form-availability__list">
      <li v-for="day in availabilityWeekdayOptions" :key="day.value" class="form-availability__item">
        <b-form-checkbox
          labelClass="label"
          class="form-availability__day"
          :id="`input-${id}-${day.text}`"
          :checked="tempDayAvailability.includes(day.value) || tempEveningAvailability.includes(day.value)"
          stacked
          @input="(v) => toggleDay(v, day.value)"
        >
          {{ day.text }}
        </b-form-checkbox>
        <b-form-checkbox
          :id="`input-${id}-${day.text}-day`"
          :checked="tempDayAvailability.includes(day.value) && activeDays.includes(day.value)"
          :disabled="!activeDays.includes(day.value)"
          @input="(v) => updateSingleAvailability(DAY, day.value, v)"
        >
          {{ $t("label.ad-dayAvailability") }}
        </b-form-checkbox>
        <b-form-checkbox
          :id="`input-${id}-${day.text}-evening`"
          :checked="tempEveningAvailability.includes(day.value) && activeDays.includes(day.value)"
          :disabled="!activeDays.includes(day.value)"
          @input="(v) => updateSingleAvailability(EVENING, day.value, v)"
        >
          {{ $t("label.ad-eveningAvailability") }}
        </b-form-checkbox>
      </li>
    </ul>
  </fieldset>
</template>

<script>
import SField from "@/components/form/s-field";
import { AvailabilityWeekday } from "@/mixins/availability-weekday";
import { DAY, EVENING } from "@/consts/periods";

export default {
  mixins: [AvailabilityWeekday],
  props: {
    id: String,
    legend: String,
    legendClass: String,
    label: String,
    name: String,
    rules: {
      type: [String, Object]
    },
    dayAvailability: Array,
    eveningAvailability: Array,
    required: Boolean,
    allSelected: Boolean
  },
  components: {
    SField
  },
  computed: {
    indeterminateState() {
      return (
        (this.tempDayAvailability.length !== this.availabilityWeekdayOptions.length && this.tempDayAvailability.length > 0) ||
        (this.tempEveningAvailability.length !== this.availabilityWeekdayOptions.length &&
          this.tempEveningAvailability.length > 0)
      );
    },
    allSelectedState() {
      return (
        this.tempDayAvailability.length === this.availabilityWeekdayOptions.length &&
        this.tempEveningAvailability.length === this.availabilityWeekdayOptions.length
      );
    }
  },
  data() {
    return {
      tempDayAvailability: this.dayAvailability ? [...this.dayAvailability] : [],
      tempEveningAvailability: this.eveningAvailability ? [...this.eveningAvailability] : [],
      activeDays: [...new Set([...(this.dayAvailability || []), ...(this.eveningAvailability || [])])],
      DAY,
      EVENING
    };
  },
  methods: {
    updateSingleAvailability(period, day, val) {
      if (period === DAY) {
        if (val && this.tempDayAvailability.includes(day)) return;
        if (val) {
          this.tempDayAvailability.push(day);
        } else {
          this.tempDayAvailability = this.tempDayAvailability.filter((x) => x !== day);
        }
        this.$emit("update:dayAvailability", this.tempDayAvailability);
      } else if (period === EVENING) {
        if (val && this.tempEveningAvailability.includes(day)) return;
        if (val) {
          this.tempEveningAvailability.push(day);
        } else {
          this.tempEveningAvailability = this.tempEveningAvailability.filter((x) => x !== day);
        }
        this.$emit("update:eveningAvailability", this.tempEveningAvailability);
      }
    },
    updateAllAvailability(allSelected) {
      if (allSelected) {
        this.activeDays = this.availabilityWeekdayOptions.map((x) => x.value);
        this.tempDayAvailability = this.availabilityWeekdayOptions.map((x) => x.value);
        this.tempEveningAvailability = this.availabilityWeekdayOptions.map((x) => x.value);
      } else {
        this.tempDayAvailability = [];
        this.tempEveningAvailability = [];
        this.activeDays = [];
      }
      this.$emit("update:dayAvailability", this.tempDayAvailability);
      this.$emit("update:eveningAvailability", this.tempEveningAvailability);
    },
    toggleDay(val, day) {
      if (val) {
        if (!this.tempDayAvailability.includes(day)) {
          this.tempDayAvailability.push(day);
        }
        if (!this.tempEveningAvailability.includes(day)) {
          this.tempEveningAvailability.push(day);
        }
        if (!this.activeDays.includes(day)) {
          this.activeDays.push(day);
        }
      } else {
        this.tempDayAvailability = this.tempDayAvailability.filter((x) => x !== day);
        this.tempEveningAvailability = this.tempEveningAvailability.filter((x) => x !== day);
        this.activeDays = this.activeDays.filter((x) => x !== day);
      }
      this.$emit("update:dayAvailability", this.tempDayAvailability);
      this.$emit("update:eveningAvailability", this.tempEveningAvailability);
    }
  },
  mounted() {
    if (this.allSelected) {
      this.updateAllAvailability(true);
    }
  }
};
</script>

<style lang="scss">
.form-availability {
  &__all {
    border-bottom: 1px solid $black;
    padding-bottom: $spacer / 2;
    margin-bottom: $spacer / 2;
  }

  &__list {
    list-style: none;
    padding-left: 0;
  }

  &__item {
    display: flex;
    gap: $spacer;
    border-bottom: 1px solid $border-color;
    padding-bottom: $spacer / 2;
    margin-bottom: $spacer / 2;
  }

  &__day {
    margin-right: auto;
  }
}
</style>
