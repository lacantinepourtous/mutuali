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
                variant="outline-primary"
                size="sm"
                @click="resendVerificationEmail(slotProps.item)">
                <b-icon icon="envelope" />
                <span class="sr-only">{{ $t("btn.resend-verification-email") }}</span>
              </b-button>
              <b-button
                variant="outline-primary"
                size="sm"
                :to="{ name: $consts.urls.URL_EDIT_PROFILE, params: { id: slotProps.item.id } }">
                <b-icon icon="pencil" />
                <span class="sr-only">{{ $t("btn.modify-user") }}</span>
              </b-button>
              <b-button 
                v-if="!slotProps.item.isConfirmed" 
                variant="outline-primary" 
                size="sm" 
                @click="confirmEmail(slotProps.item)">
                <b-icon icon="check-circle" />
                <span class="sr-only">{{ $t("btn.allow-user") }}</span>
              </b-button>
              <b-button variant="outline-danger" size="sm" @click="openDeleteUserProfileModal(slotProps.item)">
                <b-icon icon="trash" />
                <span class="sr-only">{{ $t("btn.delete-user") }}</span>
              </b-button>
            </b-button-group>
          </td>
        </template>
      </UiTable>

      <UiPagination v-if="users.totalPages > 1" class="mt-6" v-model="page" :total-pages="users.totalPages"> </UiPagination>

      <b-modal
        id="deleteUserProfileModal"
        ref="deleteUserProfileModal"
        :title="$t('modal.delete-user-profile.title-delete')"
        centered
        hide-footer
      >
      <template #default="{ cancel }">
        <p v-if="currentUser">
          {{
            $t("modal.delete-user-profile.text-delete", {
              name: getUserName(currentUser)
            })
          }}
        </p>
        <div class="delete-user-profile__modal-footer">
          <b-button type="button" variant="outline-primary" @click="cancel()">{{
            $t("modal.delete-user-profile.label-cancel")
          }}</b-button>
          <b-button class="ml-2" type="button" variant="danger" @click="handleDeleteUserProfile(currentUser.id)">{{
            $t("modal.delete-user-profile.label-delete")
          }}</b-button>
        </div>
      </template>
    </b-modal>
    </div>
  </div>
</template>

<script>
import UserService from "@/services/user";
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
      ],
      currentUser: null
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
    openDeleteUserProfileModal(user) {
      this.currentUser = user;
      this.$refs.deleteUserProfileModal.show();
    },
    async resendVerificationEmail(item) {
      await AuthentificationService.resendConfirmationEmail(item.email);
      NotificationService.showSuccess(this.$t("notification.resend-complete", { email: item.email }));
    },
    async confirmEmail(item) {
      await AuthentificationService.verifyEmail(item.email);
      NotificationService.showInfo(this.$t("notification.verify-email", { email: item.email }));
      this.$apollo.queries.users.refresh();
    },
    async handleDeleteUserProfile(userId) {
      this.$refs.deleteUserProfileModal.hide();
      await UserService.deleteUserProfile(userId);
      NotificationService.showSuccess(this.$t("notification.user-deleted"));
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