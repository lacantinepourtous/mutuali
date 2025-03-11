<template>
  <div class="restrictions">
    <h3 class="font-weight-bold h6 mt-4">{{ $t("label.availability-restrictions") }}</h3>
    <ul v-if="availabilityRestrictions.length > 0" class="list-unstyled my-4">
      <li
        v-for="(restriction, index) in sortedAvailabilityRestrictions"
        :key="restriction.startDate"
        class="restrictions__list-item"
      >
        <div class="restrictions__list-item-content">
          <b-icon icon="calendar" class="mr-2" aria-hidden="true"></b-icon>
          <span class="font-weight-bold">
            <span v-if="$format.dateDay(restriction.startDate)" class="text-capitalize">
              {{ $format.dateDay(restriction.startDate) }},
            </span>
            <span>{{ $format.dateLong(restriction.startDate) }}</span>
          </span>
          <span>•</span>
          <span>
            <span v-if="restriction.day">{{ $t("label.ad-dayAvailability") }} </span>
            <span v-if="restriction.day && restriction.evening">{{ $t("text.and") }} </span>
            <span v-if="restriction.evening" :class="{ 'text-lowercase': restriction.day }">
              {{ $t("label.ad-eveningAvailability") }}
            </span>
          </span>
        </div>

        <div class="restrictions__list-item-actions">
          <b-button variant="outline-secondary" @click="editRestriction(index)">
            <b-icon icon="pencil" :aria-label="$t('btn.edit-availability-restriction')"></b-icon>
          </b-button>
          <b-button variant="outline-danger" @click="deleteRestriction(index)">
            <b-icon icon="trash" :aria-label="$t('btn.delete-availability-restriction')"></b-icon>
          </b-button>
        </div>
      </li>
    </ul>
    <p v-else class="text-muted mb-2">{{ $t("text.no-availability-restrictions") }}</p>

    <b-button variant="outline-secondary" @click="createRestriction()">
      <b-icon icon="plus" aria-hidden="true"></b-icon>
      {{ $t("btn.create-availability-restriction") }}
    </b-button>

    <b-modal
      id="availabilityRestrictionModal"
      ref="availabilityRestrictionModal"
      :title="editionMode ? $t('modal.availability-restriction.title-edit') : $t('modal.availability-restriction.title-add')"
      centered
      hide-footer
    >
      <s-form class="rm-child-margin" @submit="editionMode ? submitEditRestriction() : submitCreateRestriction()">
        <s-form-checkbox-group
          v-model="form.periods"
          class="restrictions__period-checkbox-group"
          :label="$t('modal.availability-restriction.label-period')"
          :options="periodOptions"
          rules="required:true"
        />
        <s-form-datepicker
          v-model="form.date"
          id="availability-restriction-date"
          name="availability-restriction-date"
          :label="$t('modal.availability-restriction.label-date')"
          rules="required"
        />
        <div class="restrictions__modal-footer">
          <b-button type="button" variant="outline-primary" @click="clearForm()">
            {{ $t("modal.availability-restriction.label-cancel") }}
          </b-button>
          <b-button type="submit" variant="primary">
            {{ $t("modal.availability-restriction.label-confirm") }}
          </b-button>
        </div>
      </s-form>
    </b-modal>

    <b-modal
      id="deleteAvailabilityRestrictionModal"
      ref="deleteAvailabilityRestrictionModal"
      :title="$t('modal.availability-restriction.title-delete')"
      centered
      hide-footer
    >
      <template #default="{ cancel }">
        <p v-if="deletingIndex !== null">
          {{
            $t("modal.availability-restriction.text-delete", {
              date: $format.dateLong(availabilityRestrictions[deletingIndex].startDate)
            })
          }}
        </p>
        <div class="restrictions__modal-footer">
          <b-button type="button" variant="outline-primary" @click="cancel()">{{
            $t("modal.availability-restriction.label-cancel")
          }}</b-button>
          <b-button type="button" variant="danger" @click="submitDeleteRestriction()">{{
            $t("modal.availability-restriction.label-delete")
          }}</b-button>
        </div>
      </template>
    </b-modal>
  </div>
</template>

<script>
import { DAY, EVENING } from "@/consts/periods";

import SForm from "@/components/form/s-form";
import SFormDatepicker from "@/components/form/s-form-datepicker";
import SFormCheckboxGroup from "@/components/form/s-form-checkbox-group";

export default {
  components: {
    SForm,
    SFormDatepicker,
    SFormCheckboxGroup
  },
  props: {
    initialAvailabilityRestrictions: {
      type: Array,
      default: null
    }
  },
  data() {
    return {
      form: {
        date: null,
        periods: []
      },
      periodOptions: [
        { text: this.$t("label.ad-dayAvailability"), value: DAY },
        { text: this.$t("label.ad-eveningAvailability"), value: EVENING }
      ],
      availabilityRestrictions: this.initialAvailabilityRestrictions ? [...this.initialAvailabilityRestrictions] : [],
      editionMode: false,
      editingIndex: null,
      deletingIndex: null
    };
  },
  computed: {
    sortedAvailabilityRestrictions() {
      return [...this.availabilityRestrictions].sort((a, b) => {
        return new Date(a.startDate) - new Date(b.startDate);
      });
    },
    availabilityRestriction() {
      return {
        startDate: this.form.date,
        day: this.form.periods.includes(DAY),
        evening: this.form.periods.includes(EVENING)
      };
    }
  },
  methods: {
    createRestriction() {
      this.editionMode = false;
      this.$refs["availabilityRestrictionModal"].show();
    },
    editRestriction(index) {
      this.editionMode = true;
      // Set the form values
      this.form.date = this.sortedAvailabilityRestrictions[index].startDate;
      if (this.sortedAvailabilityRestrictions[index].day) this.form.periods.push(DAY);
      if (this.sortedAvailabilityRestrictions[index].evening) this.form.periods.push(EVENING);
      // Find the index of the restriction in the availabilityRestrictions array
      let restrictionIndex = this.availabilityRestrictions.findIndex(
        (restriction) => restriction.startDate === this.sortedAvailabilityRestrictions[index].startDate
      );
      this.editingIndex = restrictionIndex;
      this.$refs["availabilityRestrictionModal"].show();
    },
    deleteRestriction(index) {
      let restrictionIndex = this.availabilityRestrictions.findIndex(
        (restriction) => restriction.startDate === this.sortedAvailabilityRestrictions[index].startDate
      );
      this.deletingIndex = restrictionIndex;
      this.$refs["deleteAvailabilityRestrictionModal"].show();
    },
    submitCreateRestriction() {
      // Check if the restriction already exists
      if (this.availabilityRestrictions.some((restriction) => restriction.startDate === this.availabilityRestriction.startDate)) {
        let restrictionsList = [...this.availabilityRestrictions];
        let existingRestrictionIndex = restrictionsList.findIndex(
          (restriction) => restriction.startDate === this.availabilityRestriction.startDate
        );
        restrictionsList[existingRestrictionIndex] = {
          ...this.availabilityRestriction
        };

        this.availabilityRestrictions = restrictionsList;
      } else {
        this.availabilityRestrictions.push(this.availabilityRestriction);
      }

      this.clearForm();
    },
    submitEditRestriction() {
      let restrictionsList = [...this.availabilityRestrictions];
      restrictionsList[this.editingIndex] = {
        ...this.availabilityRestriction
      };
      this.availabilityRestrictions = restrictionsList;
      this.editingIndex = null;
      this.clearForm();
    },
    submitDeleteRestriction() {
      this.availabilityRestrictions.splice(this.deletingIndex, 1);
      this.deletingIndex = null;
      this.$refs["deleteAvailabilityRestrictionModal"].hide();
      this.emitUpdate();
    },
    clearForm() {
      this.$refs["availabilityRestrictionModal"].hide();
      this.emitUpdate();
      // Wait for the modal to be hidden before clearing the form
      setTimeout(() => {
        this.form.date = null;
        this.form.periods = [];
      }, 200);
    },
    emitUpdate() {
      this.$emit(
        "update",
        this.availabilityRestrictions.map((restriction) => ({
          startDate: restriction.startDate,
          day: restriction.day,
          evening: restriction.evening
        }))
      );
    }
  }
};
</script>

<style lang="scss">
.restrictions {
  &__period-checkbox-group {
    .col > div {
      display: flex;
      gap: $spacer;
    }
  }

  &__list-item {
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    gap: $spacer / 2;
    justify-content: space-between;
    margin-bottom: $spacer / 2;
    padding-bottom: $spacer / 2;
    border-bottom: 1px solid $border-color;

    &:last-child {
      margin-bottom: 0;
    }
  }

  &__list-item-content {
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    column-gap: $spacer / 2;
  }

  &__list-item-actions {
    display: flex;
    justify-content: flex-end;
    gap: $spacer / 2;
    flex-grow: 1;
  }

  &__modal-footer {
    display: flex;
    justify-content: flex-end;
    gap: $spacer / 2;
    margin: 0 -1rem;
    padding: $spacer 1rem 0;
    border-top: 1px solid $border-color;
  }
}
</style>
