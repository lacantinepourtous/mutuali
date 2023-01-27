import Apollo from "@/graphql/vue-apollo";
import { CreateConversation, RemoveConversationNotification } from "./conversation.graphql";

export default {
  createConversation: async function(adId) {
    let result = await Apollo.instance.defaultClient.mutate({
      mutation: CreateConversation,
      variables: {
        input: {
          adId
        }
      }
    });

    return result;
  },
  removeConversationNotification: async function(sid) {
    let result = await Apollo.instance.defaultClient.mutate({
      mutation: RemoveConversationNotification,
      variables: {
        input: {
          sid
        }
      }
    });

    return result;
  }
};
