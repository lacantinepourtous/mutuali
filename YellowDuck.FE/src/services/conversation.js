import Apollo from "@/graphql/vue-apollo";
import { CreateConversation, RemoveConversationNotification, RateAdAndUser } from "./conversation.graphql";
import { RATING } from "@/consts/rating";

export default {
  createConversation: async function (adId) {
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
  removeConversationNotification: async function (sid) {
    let result = await Apollo.instance.defaultClient.mutate({
      mutation: RemoveConversationNotification,
      variables: {
        input: {
          sid
        }
      }
    });

    return result;
  },
  rateAdAndUser: async function (input) {
    let mutationInput = {
      userId: input.userId,
      adId: input.adId,
      conversationId: input.conversationId,
      userRating: convertRatingToEnumsValue(input.userRating)
    };

    if (input.adRating) {
      mutationInput.adRating = convertRatingToEnumsValue(input.adRating);
    }

    let result = await Apollo.instance.defaultClient.mutate({
      mutation: RateAdAndUser,
      variables: {
        input: mutationInput
      }
    });

    return result;
  }
};

function convertRatingToEnumsValue(rating) {
  let enumValuesRating = {};
  for (const [key, value] of Object.entries(rating)) {
    enumValuesRating[key] = RATING[value];
  }
  return enumValuesRating;
}
