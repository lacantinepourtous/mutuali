<template>
  <div class="fab-container">
    <template v-if="!alertEdited">
      <portal :to="$consts.enums.PORTAL_HEADER">
        <nav-close :to="{ name: $consts.urls.URL_AD_ALERT_LIST }"></nav-close>
      </portal>
      <div class="section section--md section--padding-x section--border-bottom my-4">
        <h1 class="my-4">{{ $t("page-title.edit-alert") }}</h1>
      </div>
      <alert-form
        v-if="alert"
        :value="alert"
        @submitForm="editAlert"
        :btnLabel="$t('btn.edit-alert')"
        :disabledBtn="isSubmitted"
      />
    </template>
    <form-complete
      v-else
      :title="$t('form-complete.edit-alert.title')"
      :description="$t('form-complete.edit-alert.description')"
      :image="require('@/assets/icons/checklist-yellow.svg')"
      :ctas="formCompleteCtas"
    />
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import FormComplete from "@/components/generic/form-complete";
import AlertForm from "@/components/alert/form";

import { URL_ROOT, URL_AD_ALERT_LIST } from "@/consts/urls";

import NotificationService from "@/services/notification";
import { updateAlert } from "@/services/alert";

export default {
  components: {
    NavClose,
    AlertForm,
    FormComplete
  },
  data() {
    return {
      alertEdited: false,
      isSubmitted: false,
      formCompleteCtas: [
        { action: () => this.$router.push({ name: URL_AD_ALERT_LIST }), text: this.$t("btn.manage-my-alerts") },
        { action: () => this.$router.push({ name: URL_ROOT }), text: this.$t("btn.return-dashboard") }
      ]
    };
  },
  computed: {
    alertId: function () {
      return this.$route.params.id;
    }
  },
  apollo: {
    alert: {
      fetchPolicy: "no-cache",
      query() {
        return this.$options.query.AlertById;
      },
      variables() {
        return {
          id: this.alertId
        };
      }
    }
  },
  methods: {
    async editAlert(input) {
      if (input && Object.keys(input).length === 0 && input.constructor === Object) {
        NotificationService.showInfo(this.$t("notification.edit-alert-empty"));
      } else {
        this.isSubmitted = true;
        input.alertId = this.alertId;
        await updateAlert(input);
        this.alertEdited = true;
        window.scrollTo(0, 0);
        this.isSubmitted = false;
      }
    }
  }
};
</script>

<graphql>
query AlertById($id: ID!) {
  alert(id: $id) {
    id
    address {
      id
      latitude
      longitude
      locality
      postalCode
      route
      streetNumber
      neighborhood
      sublocality
    }
    radius
    category
    refrigerated
    canSharedRoad
    canHaveDriver
    professionalKitchenEquipment
    deliveryTruckType
    dayAvailability
    eveningAvailability
  }
}
</graphql>
