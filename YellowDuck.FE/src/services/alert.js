import Apollo from "@/graphql/vue-apollo";
import { CreateAlert } from "./alert.graphql";
import { addMaybeValue } from "@/helpers/graphql";

import { getAddressFromGooglePlace } from "@/services/google-map";
import AuthentificationService from "./authentification";

export async function createAlert(input) {
  let mutationInput = {
    category: input.category,
    address: await getAddressFromGooglePlace(input.address),
    radius: input.radius,
  };

  addMaybeValue(input, mutationInput, "professionalKitchenEquipment");
  addMaybeValue(input, mutationInput, "deliveryTruckType");
  addMaybeValue(input, mutationInput, "dayAvailability");
  addMaybeValue(input, mutationInput, "eveningAvailability");
  addMaybeValue(input, mutationInput, "refrigerated");
  addMaybeValue(input, mutationInput, "canSharedRoad");
  addMaybeValue(input, mutationInput, "canHaveDriver");
  addMaybeValue(input, mutationInput, "email");

  let result = await Apollo.instance.defaultClient.mutate({
    mutation: CreateAlert,
    variables: {
      input: mutationInput
    }
  });

  // Renew the token since we want the new ad in our
  await AuthentificationService.renewToken();
  return result;
}