<template>
  <s-form class="my-n4" @submit="submitForm">
    <s-form-input
      v-model="message"
      id="message"
      name="message"
      :label="inputPlaceholder"
      label-sr-only
      :placeholder="inputPlaceholder"
    >
      <template #input-group-append>
        <b-input-group-append>
          <b-button type="submit" variant="outline-secondary" :aria-label="$t('sr.send-message')">
            <b-icon icon="reply-fill" aria-hidden="true"></b-icon>
          </b-button>
        </b-input-group-append>
      </template>
    </s-form-input>
  </s-form>
</template>

<script>
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";

import TwilioService from "@/services/twilio";

export default {
  data() {
    return {
      message: ""
    };
  },
  components: {
    SForm,
    SFormInput
  },
  computed: {
    inputPlaceholder: function () {
      return this.$t("placeholder.new-message", { user: this.otherParticipantName });
    }
  },
  props: {
    conversationSid: {
      type: String
    },
    otherParticipantName: {
      type: String,
      required: true
    }
  },
  methods: {
    submitForm: async function () {
      if (this.message !== "") {
        if (this.conversationSid) {
          await TwilioService.addMessageToConversation(this.conversationSid, this.message);
        } else {
          this.$emit("sendMessage", this.message);
        }
        this.message = "";
      }
    }
  }
};
</script>
