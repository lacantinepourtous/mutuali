import Apollo from "@/graphql/vue-apollo";
import { DeleteRating } from "./rating.graphql";

export async function deleteRating(input) {
  let mutationInput = {};

  if (input.adRatingId) {
    mutationInput.adRatingId = input.adRatingId;
  }

  if (input.userRatingId) {
    mutationInput.userRatingId = input.userRatingId;
  }

  let result = await Apollo.instance.defaultClient.mutate({
    mutation: DeleteRating,
    variables: {
      input: mutationInput
    }
  });

  return result;
}
