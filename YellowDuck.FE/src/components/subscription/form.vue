<template>
  <s-form class="my-4" @submit="subscribeUser">
    <s-form-input
      id="email"
      :label="$t('label.email')"
      name="email"
      rules="required|email"
      v-model="email"
      type="email"
      :placeholder="$t('placeholder.email')"
      required
    />
    <s-form-checkbox v-model="showEmail" id="showEmail" :label="$t('label.userProfile-showEmail')" name="showEmail" />
    <s-form-input
      id="password"
      :label="$t('label.password')"
      :description="$t('description.password')"
      name="password"
      rules="required|password"
      v-model="password"
      type="password"
      required
    />
    <s-form-input
      id="passwordConfirmation"
      :label="$t('label.password-confirmation')"
      name="passwordConfirmation"
      rules="required|samePassword:@password"
      v-model="passwordConfirmation"
      type="password"
      required
    />
    <s-form-input
      id="firstname"
      :label="$t('label.firstname')"
      name="firstname"
      rules="required"
      v-model="firstname"
      type="text"
      :placeholder="$t('placeholder.firstname')"
      required
    />
    <s-form-input
      id="lastname"
      :label="$t('label.lastname')"
      name="lastname"
      rules="required"
      v-model="lastname"
      type="text"
      :placeholder="$t('placeholder.lastname')"
      required
    />
    <s-form-input
      id="organizationName"
      :label="$t('label.organizationName')"
      name="organizationName"
      rules="required"
      v-model="organizationName"
      type="text"
      :placeholder="$t('placeholder.organizationName')"
      required
    />
    <s-form-input
      id="organizationNEQ"
      :label="$t('label.organizationNEQ')"
      name="organizationNEQ"
      rules="required|isNEQ"
      v-model="organizationNEQ"
      type="text"
      required
    />
    <s-form-select
      v-model="organizationType"
      id="organizationType"
      :label="$t('label.organizationType')"
      name="organizationType"
      rules="required"
      :placeholder="$t('placeholder.organizationType')"
      :options="organizationTypeOptions"
      required
    />
    <s-form-input
      v-if="displayOrganizationTypeOtherSpecification"
      id="organizationTypeOtherSpecification"
      :label="$t('label.organizationTypeOtherSpecification')"
      name="organizationTypeOtherSpecification"
      :rules="organizationTypeOtherSpecificationRules"
      v-model="organizationTypeOtherSpecification"
      type="text"
      required
    />
    <s-form-select
      v-model="industry"
      id="industry"
      :label="$t('label.industry')"
      name="industry"
      rules="required"
      :placeholder="$t('placeholder.industry')"
      :options="industryOptions"
      required
    />
    <s-form-input
      v-if="displayIndustryOtherSpecification"
      id="industryOtherSpecification"
      :label="$t('label.industryOtherSpecification')"
      name="industryOtherSpecification"
      :rules="industryOtherSpecificationRules"
      v-model="industryOtherSpecification"
      type="text"
      required
    />
    <s-form-input
      v-model="phoneNumber"
      id="phoneNumber"
      :label="$t('label.phoneNumber')"
      name="phoneNumber"
      rules="required"
      :placeholder="$t('placeholder.phoneNumber')"
      required
      @change="phoneNumberIsConfirmed = false"
    />
    <div class="d-flex flex-wrap align-items-center mt-n3">
      <b-button
        class="mr-3 my-1"
        :disabled="!isValidPhoneNumber || phoneNumberIsConfirmed"
        variant="primary"
        type="button"
        @click="validatePhoneModal = true"
        >{{ $t("confirm-phone.open-modal") }}</b-button
      >
      <div class="d-flex align-items-center">
        <b-icon v-if="phoneNumberIsConfirmed" icon="check-circle" variant="success"></b-icon>
        <p v-if="phoneNumberIsConfirmed" class="mb-0 ml-2 text-success">{{ $t("confirm-phone.phone-number-confirmed") }}</p>
      </div>
    </div>

    <s-form-hidden
      v-if="phoneNumber"
      class="mt-n1"
      :value="phoneNumberIsConfirmed ? 1 : null"
      id="phoneNumberIsConfirmed"
      name="phoneNumberIsConfirmed"
      rules="isValidPhoneNumber"
    />

    <phone-verification-modal
      v-model="validatePhoneModal"
      :phone-number="phoneNumber"
      :title="$t('confirm-phone.title')"
      @validation-success="onPhoneValidated"
    />

    <s-form-checkbox
      v-model="showPhoneNumber"
      id="showPhoneNumber"
      :label="$t('label.userProfile-showPhoneNumber')"
      name="showPhoneNumber"
    />
    <s-form-checkbox-group
      v-model="registeringInterests"
      id="registeringInterests"
      :label="$t('label.userProfile-registeringInterests')"
      name="registeringInterests"
      :options="registeringInterestOptions"
    />
    <s-form-checkbox
      v-model="contactAuthorizationSurveys"
      id="contactAuthorizationSurveys"
      :label="$t('label.userProfile-contactAuthorizationSurveys')"
      name="contactAuthorizationSurveys"
    />
    <s-form-checkbox
      v-model="contactAuthorizationNews"
      id="contactAuthorizationNews"
      :label="$t('label.userProfile-contactAuthorizationNews')"
      name="contactAuthorizationNews"
    />
    <s-form-checkbox
      id="tosAccepted"
      :label="
        $t('label.tos-accepted', {
          url1: '/static/terms/MutuAli_ConditionsGenerales_2024-07-05.pdf',
          url2: '/static/terms/MutuAli_PolitiqueConfidentialite_2024-07-05.pdf'
        })
      "
      name="tosAccepted"
      :rules="{ required: { allowFalse: false } }"
      v-model="tosAccepted"
      required
    />
    <s-form-checkbox
      id="notRentalCompany"
      :label="$t('label.notRentalCompany')"
      name="notRentalCompany"
      :rules="{ required: { allowFalse: false } }"
      v-model="notRentalCompany"
      required
    />
    <b-button type="submit" variant="primary" size="lg" block>{{ $t("btn.submit") }}</b-button>
  </s-form>
</template>

<script>
import SForm from "@/components/form/s-form";
import SFormHidden from "@/components/form/s-form-hidden";
import SFormInput from "@/components/form/s-form-input";
import SFormCheckbox from "@/components/form/s-form-checkbox";
import SFormSelect from "@/components/form/s-form-select";
import SFormCheckboxGroup from "@/components/form/s-form-checkbox-group";
import { RegisteringInterests } from "@/mixins/registering-interests";

import {
  ORGANIZATION_TYPE_NON_PROFIT,
  ORGANIZATION_TYPE_FOOD_PROCESSING,
  ORGANIZATION_TYPE_AGRICULTURE,
  ORGANIZATION_TYPE_SOCIAL_ECONOMY,
  ORGANIZATION_TYPE_OTHER
} from "@/consts/organization-type";

import {
  INDUSTRY_FOOD_PROCESSING_AND_DISTRIBUTION,
  INDUSTRY_CATERING,
  INDUSTRY_RETAIL,
  INDUSTRY_EDUCATION_AND_TEACHING,
  INDUSTRY_HEALTH_AND_SOCIAL_SERVICES,
  INDUSTRY_TRANSPORT,
  INDUSTRY_OTHER
} from "@/consts/industry";

import PhoneVerificationModal from "@/components/phone-verification/phone-verification-modal";

export default {
  mixins: [RegisteringInterests],
  components: {
    SForm,
    SFormHidden,
    SFormInput,
    SFormCheckbox,
    SFormSelect,
    SFormCheckboxGroup,
    PhoneVerificationModal
  },
  data() {
    return {
      email: "",
      password: "",
      passwordConfirmation: "",
      firstname: "",
      lastname: "",
      organizationName: "",
      organizationNEQ: "",
      organizationType: null,
      industry: null,
      phoneNumber: null,
      pin: null,
      phoneNumberIsConfirmed: false,
      validatePhoneModal: false,
      showPhoneNumber: null,
      showEmail: null,
      registeringInterests: [],
      contactAuthorizationSurveys: null,
      contactAuthorizationNews: null,
      tosAccepted: false,
      notRentalCompany: false,
      organizationTypeOptions: [
        { value: ORGANIZATION_TYPE_SOCIAL_ECONOMY, text: this.$t("select.social-economy-organizations") },
        { value: ORGANIZATION_TYPE_FOOD_PROCESSING, text: this.$t("select.food-processing-organizations") },
        { value: ORGANIZATION_TYPE_AGRICULTURE, text: this.$t("select.agriculture-organizations") },
        { value: ORGANIZATION_TYPE_NON_PROFIT, text: this.$t("select.non-profit-organizations") },

        { value: ORGANIZATION_TYPE_OTHER, text: this.$t("select.other") }
      ],
      industryOptions: [
        { value: INDUSTRY_FOOD_PROCESSING_AND_DISTRIBUTION, text: this.$t("select.industry-food-processing-and-distribution") },
        { value: INDUSTRY_CATERING, text: this.$t("select.industry-catering") },
        { value: INDUSTRY_RETAIL, text: this.$t("select.industry-retail") },
        { value: INDUSTRY_EDUCATION_AND_TEACHING, text: this.$t("select.industry-education-and-teaching") },
        { value: INDUSTRY_HEALTH_AND_SOCIAL_SERVICES, text: this.$t("select.industry-health-and-social-services") },
        { value: INDUSTRY_TRANSPORT, text: this.$t("select.industry-transport") },
        { value: INDUSTRY_OTHER, text: this.$t("select.other") }
      ],
      organizationTypeOtherSpecification: "",
      industryOtherSpecification: ""
    };
  },
  computed: {
    organizationTypeOtherSpecificationRules: function () {
      return this.organizationType ? "required" : "";
    },
    displayOrganizationTypeOtherSpecification: function () {
      return this.organizationType === ORGANIZATION_TYPE_OTHER;
    },
    industryOtherSpecificationRules: function () {
      return this.industry ? "required" : "";
    },
    displayIndustryOtherSpecification: function () {
      return this.industry === INDUSTRY_OTHER;
    },
    isValidPhoneNumber() {
      return this.phoneNumber && this.phoneNumber.replace(/\D/g, "").length === 10;
    }
  },
  watch: {
    phoneNumber: function (newVal) {
      if (newVal && this.phoneNumberIsConfirmed) {
        this.phoneNumberIsConfirmed = false;
      }
    }
  },
  methods: {
    subscribeUser: async function () {
      let input = {
        email: this.email,
        password: this.password,
        firstName: this.firstname,
        lastName: this.lastname,
        organizationName: this.organizationName,
        organizationNEQ: this.organizationNEQ,
        organizationType: this.organizationType,
        industry: this.industry,
        phoneNumber: this.phoneNumber,
        showPhoneNumber: this.showPhoneNumber,
        showEmail: this.showEmail,
        contactAuthorizationNews: this.contactAuthorizationNews,
        contactAuthorizationSurveys: this.contactAuthorizationSurveys
      };

      if (this.displayOrganizationTypeOtherSpecification) {
        input.organizationTypeOtherSpecification = { value: this.organizationTypeOtherSpecification };
      }

      if (this.displayIndustryOtherSpecification) {
        input.industryOtherSpecification = { value: this.industryOtherSpecification };
      }

      if (this.registeringInterests.length > 0) {
        input.registeringInterests = { value: this.registeringInterests };
      }

      this.$emit("submitForm", input);
    },
    onPhoneValidated() {
      this.phoneNumberIsConfirmed = true;
    }
  }
};
</script>

<style scoped>
.text-muted {
  cursor: not-allowed;
}
</style>
