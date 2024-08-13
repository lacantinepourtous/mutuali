<template>
  <div class="w-100" v-if="contract">
    <portal v-if="!contractEdited" :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_CONVERSATION_DETAIL, params: { id: this.conversationId } }"></nav-close>
    </portal>
    <div v-if="!contractEdited" class="section section--md my-4">
      <h1 class="my-4">{{ $t("page-title.edit-contract") }}</h1>
      <contract-form
        :datePrecision="contractDatePrecision"
        :files="contractFiles"
        :price="contractPrice"
        :startDate="contractStartDate"
        :endDate="contractEndDate"
        :terms="contractTerms"
        @submitForm="editContract"
        :btnLabel="$t('btn.update-contract')"
        :disabledBtn="isSubmitted"
      />
    </div>
    <form-complete
      v-else
      :title="$t('form-complete.edit-contract.title')"
      :description="$t('form-complete.edit-contract.description', { user: otherParticipantName })"
      :image="require('@/assets/icons/checklist-yellow.svg')"
      :ctas="formCompleteCtas"
    />
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import FormComplete from "@/components/generic/form-complete";
import ContractForm from "@/components/contract/form";

import { URL_CONVERSATION_DETAIL, URL_CONTRACT_DETAIL } from "@/consts/urls";

import NotificationService from "@/services/notification";
import { updateContract } from "@/services/contract";

export default {
  components: {
    ContractForm,
    FormComplete,
    NavClose
  },
  data() {
    return {
      contractEdited: false,
      formCompleteCtas: [
        {
          action: () => this.$router.push({ name: URL_CONTRACT_DETAIL, params: { id: this.contractId } }),
          text: this.$t("btn.display-detail-contract")
        },
        {
          action: () => this.$router.push({ name: URL_CONVERSATION_DETAIL, params: { id: this.conversationId } }),
          text: this.$t("btn.return-conversation")
        }
      ],
      isSubmitted: false
    };
  },
  gqlErrors() {
    return {
      FILE_NOT_FOUND(error) {
        return this.$t("error.file-upload");
      }
    };
  },
  methods: {
    editContract: async function (input) {
      if (input && Object.keys(input).length === 0 && input.constructor === Object) {
        NotificationService.showInfo(this.$t("notification.edit-contract-empty"));
      } else {
        this.isSubmitted = true;
        input.contractId = this.contractId;
        await updateContract(input);
        this.contractEdited = true;
        window.scrollTo(0, 0);
        this.isSubmitted = false;
      }
    }
  },
  computed: {
    contractId: function () {
      return this.$route.params.id.split("-").last();
    },
    conversationId: function () {
      return this.contract.conversation.id;
    },
    contractDatePrecision: function () {
      return this.contract.datePrecision;
    },
    contractFiles: function () {
      return this.contract.files;
    },
    contractPrice: function () {
      return this.contract.price;
    },
    contractStartDate: function () {
      return this.contract.startDate;
    },
    contractEndDate: function () {
      return this.contract.endDate;
    },
    contractTerms: function () {
      return this.contract.terms;
    },
    otherParticipantName: function () {
      let otherParticipant = this.contract.conversation.participants.find((x) => x.user !== null && x.user.id !== this.me.id);
      return otherParticipant !== null ? otherParticipant.user.profile.publicName : "";
    }
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      }
    },
    contract: {
      query() {
        return this.$options.query.ContractById;
      },
      variables() {
        return {
          id: this.contractId
        };
      }
    }
  }
};
</script>

<graphql>
query ContractById($id: ID!) {
  contract(id: $id) {
    id
    conversation {
      id
      participants {
        id
        user {
          id
          profile {
            id
            ... on UserProfileGraphType {
              publicName
            }
          }
        }
      }
    }
    datePrecision
    files
    price
    startDate
    endDate
    terms
  }
}

query Me {
  me {
    id
  }
}
</graphql>