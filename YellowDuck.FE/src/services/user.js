import Apollo from "@/graphql/vue-apollo";

import { addMaybeValue } from "@/helpers/graphql";
import {
  CreateUserAccount,
  SendPasswordReset,
  ResetPassword,
  VerifyToken,
  UpdateUserProfile,
  ChangePassword,
  CreateAdminAccount,
  CompleteAdminRegistration
} from "./user.graphql";

export default {
  createUserAccount: async function(input, returnPath) {
    await Apollo.instance.defaultClient.mutate({
      mutation: CreateUserAccount,
      variables: {
        input: {
          postalCode: input.postalCode,
          phoneNumber: input.phoneNumber,
          showPhoneNumber: input.showPhoneNumber || false,
          showEmail: input.showEmail || false,
          email: input.email,
          firstName: input.firstName,
          lastName: input.lastName,
          organizationName: input.organizationName,
          organizationType: input.organizationType,
          organizationTypeOtherSpecification: input.organizationTypeOtherSpecification,
          industry: input.industry,
          industryOtherSpecification: input.industryOtherSpecification,
          password: input.password,
          contactAuthorizationNews: input.contactAuthorizationNews || false,
          contactAuthorizationSurveys: input.contactAuthorizationSurveys || false,
          registeringInterests: input.registeringInterests,
          returnPath
        }
      }
    });
  },
  createAdminAccount: async function(input) {
    await Apollo.instance.defaultClient.mutate({
      mutation: CreateAdminAccount,
      variables: {
        input: {
          email: input.email,
          firstName: input.firstName,
          lastName: input.lastName
        }
      }
    });
  },
  completeAdminRegistration: async function(input) {
    await Apollo.instance.defaultClient.mutate({
      mutation: CompleteAdminRegistration,
      variables: {
        input
      }
    });
  },
  verifyToken: async function(email, token, tokenType) {
    let result = await Apollo.instance.defaultClient.query({
      query: VerifyToken,
      variables: {
        email,
        token,
        type: tokenType
      }
    });
    return result.data.verifyToken;
  },
  sendPasswordReset: async function(email) {
    let input = { email };

    await Apollo.instance.defaultClient.mutate({
      mutation: SendPasswordReset,
      variables: {
        input
      }
    });
  },
  resetPassword: async function(emailAddress, newPassword, token) {
    let input = { emailAddress, newPassword, token };

    await Apollo.instance.defaultClient.mutate({
      mutation: ResetPassword,
      variables: {
        input
      }
    });
  },
  updateUserProfile: async function(input) {
    let mutationInput = {
      userId: input.userId
    };
    addMaybeValue(input, mutationInput, "firstName");
    addMaybeValue(input, mutationInput, "lastName");
    addMaybeValue(input, mutationInput, "organizationName");
    addMaybeValue(input, mutationInput, "organizationType");
    addMaybeValue(input, mutationInput, "organizationTypeOtherSpecification");
    addMaybeValue(input, mutationInput, "industry");
    addMaybeValue(input, mutationInput, "industryOtherSpecification");
    addMaybeValue(input, mutationInput, "postalCode");
    addMaybeValue(input, mutationInput, "phoneNumber");
    addMaybeValue(input, mutationInput, "showPhoneNumber");
    addMaybeValue(input, mutationInput, "showEmail");
    await Apollo.instance.defaultClient.mutate({
      mutation: UpdateUserProfile,
      variables: {
        input: mutationInput
      }
    });
  },
  saveAccountSettings: async function(input) {
    await Apollo.instance.defaultClient.mutate({
      mutation: ChangePassword,
      variables: {
        input
      }
    });
  }
};
