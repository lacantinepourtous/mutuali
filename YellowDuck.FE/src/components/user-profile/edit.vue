<template>
  <s-form class="my-4" @submit="saveProfile">
    <s-form-input
      id="firstname"
      :label="$t('label.firstname')"
      name="firstname"
      rules="required"
      v-model="form.firstName"
      type="text"
      :placeholder="$t('placeholder.firstname')"
      required
    />
    <s-form-input
      id="lastname"
      :label="$t('label.lastname')"
      name="lastname"
      rules="required"
      v-model="form.lastName"
      type="text"
      :placeholder="$t('placeholder.lastname')"
      required
    />
    <template v-if="userType === $consts.enums.USER_TYPE_USER">
      <s-form-input
        id="organizationName"
        :label="$t('label.organizationName')"
        name="organizationName"
        rules="required"
        v-model="form.organizationName"
        type="text"
        :placeholder="$t('placeholder.organizationName')"
        required
      />
      <s-form-select
        v-model="form.organizationType"
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
        v-model="form.organizationTypeOtherSpecification"
        type="text"
        required
      />
      <s-form-select
        v-model="form.industry"
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
        v-model="form.industryOtherSpecification"
        type="text"
        required
      />
      <s-form-input
        v-model="form.phoneNumber"
        id="phoneNumber"
        :label="$t('label.phoneNumber')"
        name="phoneNumber"
        rules="required"
        :placeholder="$t('placeholder.phoneNumber')"
        required
      />
      <s-form-checkbox
        v-model="form.showPhoneNumber"
        id="showPhoneNumber"
        :label="$t('label.userProfile-showPhoneNumber')"
        name="showPhoneNumber"
      />
      <s-form-checkbox v-model="form.showEmail" id="showEmail" :label="$t('label.userProfile-showEmail')" name="showEmail" />
    </template>
    <b-button type="submit" variant="primary" size="lg" block>{{ $t("btn.edit-profile") }}</b-button>
  </s-form>
</template>

<script>
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";
import SFormSelect from "@/components/form/s-form-select";
import SFormCheckbox from "@/components/form/s-form-checkbox";

import {
  ORGANIZATION_TYPE_NON_PROFIT_ORGANIZATIONS,
  ORGANIZATION_TYPE_PRIVATE_COMPANY,
  ORGANIZATION_TYPE_PUBLIC_SECTOR,
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

export default {
  components: {
    SForm,
    SFormInput,
    SFormSelect,
    SFormCheckbox
  },
  data() {
    return {
      form: {
        firstName: "",
        lastName: "",
        organizationName: "",
        organizationType: null,
        organizationTypeOtherSpecification: "",
        industry: null,
        industryOtherSpecification: "",
        phoneNumber: null,
        showPhoneNumber: null,
        showEmail: null
      },
      organizationTypeOptions: [
        { value: ORGANIZATION_TYPE_NON_PROFIT_ORGANIZATIONS, text: this.$t("select.non-profit-organizations") },
        { value: ORGANIZATION_TYPE_PRIVATE_COMPANY, text: this.$t("select.private-company") },
        { value: ORGANIZATION_TYPE_PUBLIC_SECTOR, text: this.$t("select.public-sector") },
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
      ]
    };
  },
  props: {
    profileId: {
      type: String,
      required: true
    },
    userId: {
      type: String,
      required: true
    },
    userType: {
      type: String,
      required: true
    }
  },
  apollo: {
    userProfile: {
      query() {
        return this.$options.query.UserProfileById;
      },
      variables() {
        return {
          id: this.profileId
        };
      },
      result({ data }) {
        if (data) {
          this.form = {
            ...this.form,
            ...data.userProfile
          };
        }
      }
    }
  },
  computed: {
    organizationTypeOtherSpecificationRules: function() {
      return this.form.organizationType ? "required" : "";
    },
    displayOrganizationTypeOtherSpecification: function() {
      return this.form.organizationType === ORGANIZATION_TYPE_OTHER;
    },
    industryOtherSpecificationRules: function() {
      return this.form.industry ? "required" : "";
    },
    displayIndustryOtherSpecification: function() {
      return this.form.industry === INDUSTRY_OTHER;
    }
  },
  methods: {
    saveProfile: async function() {
      let input = {
        userId: this.userId
      };

      let maybeEditedFields = ["firstName", "lastName"];

      if (this.userType === this.$consts.enums.USER_TYPE_USER) {
        maybeEditedFields.push(
          "organizationName",
          "organizationType",
          "organizationTypeOtherSpecification",
          "industry",
          "industryOtherSpecification",
          "phoneNumber",
          "showPhoneNumber",
          "showEmail"
        );
      }

      for (let maybeEditedField of maybeEditedFields) {
        if (this.userProfile[maybeEditedField] !== this.form[maybeEditedField]) {
          input[maybeEditedField] = this.form[maybeEditedField];
        }
      }

      this.$emit("submitForm", input);
    }
  }
};
</script>
<graphql>
query UserProfileById($id: ID!) {
  userProfile(id: $id) {
    id
    firstName
    lastName
    organizationName
    organizationType
    organizationTypeOtherSpecification
    industry
    industryOtherSpecification
    phoneNumber
    showPhoneNumber
    showEmail
  }
}
</graphql>
