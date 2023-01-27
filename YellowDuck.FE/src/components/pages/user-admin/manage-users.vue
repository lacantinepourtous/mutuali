<template>
  <div class="w-100">
    <div class="section section--xl mt-4 mt-md-5 mb-4">
      <h1>{{ $t("page-title.manage-users") }}</h1>
    </div>

    <div v-if="users && users.items" class="section section--xl my-4">
      <div class="mb-4">
        <b-button :to="{ name: $consts.urls.URL_CREATE_ADMIN }" variant="primary" size="md" class="mr-3 mb-2">{{ $t("add-admin-btn") }}</b-button>
        <b-button :to="{ name: $consts.urls.URL_CREATE_USER }" variant="primary" size="md" class="mr-3 mb-2">{{ $t("add-user-btn") }}</b-button>
      </div>

      <UiTable :items="users.items" :cols="cols">
        <template #default="slotProps">
          <td class="px-4 py-3">
            <a :href="getUserMailTo(slotProps.item)">{{ getUserName(slotProps.item) }}</a>
          </td>
          <td class="px-4 py-3">
            {{ getUserStatus(slotProps.item) }}
          </td>
          <td class="px-4 py-3">
            {{ isUserAdmin(slotProps.item) }}
          </td>
          <td class="px-4">
            <b-button-group>
              <b-button
              v-if="!slotProps.item.isConfirmed"
              class="text-nowrap"
              variant="outline-primary"
              size="sm"
              @click="resendVerificationEmail(slotProps.item)">
                {{ $t("btn.resend-verification-email") }}
              </b-button>
              <b-button
                variant="outline-primary"
                size="sm"
                :to="{ name: $consts.urls.URL_EDIT_PROFILE, params: { id: slotProps.item.id } }">
                  {{ $t("btn.modify-user") }}
              </b-button>
              <b-button v-if="!slotProps.item.isConfirmed" variant="outline-primary" size="sm" @click="confirmEmail(slotProps.item)">
                {{ $t("btn.allow-user") }}
              </b-button>
            </b-button-group>
          </td>
        </template>
      </UiTable>

      <UiPagination v-if="users.totalPages > 1" class="mt-6" v-model="page" :total-pages="users.totalPages"> </UiPagination>
    </div>
  </div>
</template>

<script>
import UiTable from "@/components/ui/table";
import UiPagination from "@/components/ui/pagination";

import { USER_TYPE_ADMIN } from "@/consts/enums";

import AuthentificationService from "@/services/authentification";
import NotificationService from "@/services/notification";

export default {
  data() {
    return {
      page: 1,
      cols: [
        { label: this.$t("username") },
        { label: this.$t("status") },
        { label: this.$t("role") },
        {
          label: this.$t("options"),
          hasHiddenLabel: true
        }
      ]
    };
  },
  components: {
    UiTable,
    UiPagination
  },
  methods: {
    getUserMailTo(item) {
      return `mailto:${item.email}`;
    },
    getUserName(item) {
      return `${item.profile.firstName} ${item.profile.lastName}`;
    },
    getUserStatus(item) {
      return item.isConfirmed ? this.$t("user-confirmed") : this.$t("user-not-confirmed");
    },
    isUserAdmin(item) {
      return item.type === USER_TYPE_ADMIN ? this.$t("user-type-admin") : this.$t("user-type-user");
    },
    async resendVerificationEmail(item) {
      await AuthentificationService.resendConfirmationEmail(item.email);
      NotificationService.showSuccess(this.$t("notification.resend-complete", { email: item.email }));
    },
    async confirmEmail(item) {
      await AuthentificationService.verifyEmail(item.email);
      NotificationService.showInfo(this.$t("notification.verify-email", { email: item.email }));
      this.$apollo.queries.users.refresh();
    }
  },
  apollo: {
    users: {
      query() {
        return this.$options.query.GetUsers;
      },
      variables() {
        return {
          page: this.page
        };
      }
    }
  }
};
</script>

<graphql>
query GetUsers($page: Int!) {
  users(page: $page, limit: 30) {
    items {
      id
      email
      profile {
        id
        firstName
        lastName
      }
      isConfirmed
      type
    }
    totalPages
  }
}
</graphql>