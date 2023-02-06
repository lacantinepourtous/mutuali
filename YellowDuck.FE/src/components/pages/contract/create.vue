<template>
  <div class="w-100">
    <portal v-if="!contractCreated" :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_CONVERSATION_DETAIL, params: { id: this.conversationId } }"></nav-close>
    </portal>
    <div v-if="!contractCreated" class="section section--md my-4">
      <h1 class="my-4">{{ $t("page-title.create-contract") }}</h1>
      <contract-form
        :disabledBtn="isSubmitted"
        @submitForm="createContract"
        displayDisclaimer
        :btnLabel="$t('btn.create-contract')"
      />
    </div>
    <form-complete
      v-else
      :title="$t('form-complete.create-contract.title')"
      :description="$t('form-complete.create-contract.description', { user: otherParticipantName })"
      :image="require('@/assets/icons/checklist.png')"
      :ctas="formCompleteCtas"
    />
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import FormComplete from "@/components/generic/form-complete";
import ContractForm from "@/components/contract/form";

import { URL_CONVERSATION_DETAIL, URL_CONTRACT_DETAIL } from "@/consts/urls";

import { createContract } from "@/services/contract";

export default {
  components: {
    ContractForm,
    FormComplete,
    NavClose
  },
  data() {
    return {
      conversationId: this.$route.params.id.split("-").last(),
      contractCreated: false,
      contractCreatedId: "",
      formCompleteCtas: [
        {
          action: () => this.$router.push({ name: URL_CONTRACT_DETAIL, params: { id: this.contractCreatedId } }),
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
    createContract: async function (input) {
      this.isSubmitted = true;
      input.conversationId = this.conversationId;
      let result = await createContract(input);

      if (result) {
        this.contractCreatedId = result.data.createContract.contract.id;
        this.contractCreated = true;
        window.scrollTo(0, 0);
      }
      this.isSubmitted = false;
    }
  },
  computed: {
    otherParticipantName: function () {
      let otherParticipant = this.conversation.participants.find((x) => x.user !== null && x.user.id !== this.me.id);
      return otherParticipant !== null ? otherParticipant.user.profile.publicName : "";
    }
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      }
    },
    conversation: {
      query() {
        return this.$options.query.ConversationById;
      },
      variables() {
        return {
          id: this.conversationId
        };
      }
    }
  }
};
</script>

<graphql>
query ConversationById($id: ID!) {
  conversation(id: $id) {
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
}

query Me {
  me {
    id
  }
}
</graphql>