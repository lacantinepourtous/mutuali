import Apollo from "@/graphql/vue-apollo";
import { CreateAd, UpdateAd, UpdateAdTranslation, PublishAd, UnpublishAd, TransferAd, LockAd, UnlockAd } from "./ad.graphql";

import { CONTENT_LANG_FR } from "@/consts/langs";

import { addMaybeValue } from "@/helpers/graphql";

import { getAddressFromGooglePlace } from "@/services/google-map";
import AuthentificationService from "./authentification";
import { uploadImage } from "./file-upload";

const IMAGES_FOLDER = "/images/";

export async function createAd(input) {
  let galleryItems = undefined;
  if (input.galleryItems.length > 0) {
    galleryItems = { value: await GetGalleryItems(input.galleryItems) };
  }

  let address = await getAddressFromGooglePlace(input.address);

  let mutationInput = {
    title: input.title,
    category: input.category,
    isAvailableForRent: typeof input.isAvailableForRent === "boolean" ? input.isAvailableForRent : false,
    isAvailableForSale: typeof input.isAvailableForSale === "boolean" ? input.isAvailableForSale : false,
    isAvailableForDonation: typeof input.isAvailableForDonation === "boolean" ? input.isAvailableForDonation : false,
    isAvailableForTrade: typeof input.isAvailableForTrade === "boolean" ? input.isAvailableForTrade : false,
    language: CONTENT_LANG_FR,
    galleryItems,
    address,
    showAddress: typeof input.showAddress === "boolean" ? input.showAddress : false,
    rentPriceToBeDetermined: typeof input.rentPriceToBeDetermined === "boolean" ? input.rentPriceToBeDetermined : false,
    rentPrice: input.rentPrice,
    salePriceToBeDetermined: typeof input.salePriceToBeDetermined === "boolean" ? input.salePriceToBeDetermined : false,
    salePrice: input.salePrice,
    rentPriceDescription: input.rentPriceDescription,
    salePriceDescription: input.salePriceDescription,
    donationDescription: input.donationDescription,
    tradeDescription: input.tradeDescription,
    organization: input.organization
  };

  addMaybeValue(input, mutationInput, "rentPriceRange");
  addMaybeValue(input, mutationInput, "salePriceRange");
  addMaybeValue(input, mutationInput, "conditions");
  addMaybeValue(input, mutationInput, "description");
  addMaybeValue(input, mutationInput, "surfaceSize");
  addMaybeValue(input, mutationInput, "equipment");
  addMaybeValue(input, mutationInput, "surfaceDescription");
  addMaybeValue(input, mutationInput, "professionalKitchenEquipment");
  addMaybeValue(input, mutationInput, "professionalKitchenEquipmentOther");
  addMaybeValue(input, mutationInput, "deliveryTruckType");
  addMaybeValue(input, mutationInput, "deliveryTruckTypeOther");
  addMaybeValue(input, mutationInput, "dayAvailability");
  addMaybeValue(input, mutationInput, "eveningAvailability");
  addMaybeValue(input, mutationInput, "availabilityRestriction");
  addMaybeValue(input, mutationInput, "refrigerated");
  addMaybeValue(input, mutationInput, "canSharedRoad");
  addMaybeValue(input, mutationInput, "canHaveDriver");
  addMaybeValue(input, mutationInput, "certification");
  addMaybeValue(input, mutationInput, "allergen");
  addMaybeValue(input, mutationInput, "humanResourceField");
  addMaybeValue(input, mutationInput, "humanResourceFieldOther");
  addMaybeValue(input, mutationInput, "qualifications");
  addMaybeValue(input, mutationInput, "tasks");
  addMaybeValue(input, mutationInput, "geographicCoverage");

  let result = await Apollo.instance.defaultClient.mutate({
    mutation: CreateAd,
    variables: {
      input: mutationInput
    }
  });

  // Renew the token since we want the new ad in our
  await AuthentificationService.renewToken();
  return result;
}

export async function updateAd(input) {
  if (
    "title" in input ||
    "description" in input ||
    "rentPriceDescription" in input ||
    "salePriceDescription" in input ||
    "donationDescription" in input ||
    "tradeDescription" in input ||
    "conditions" in input ||
    "professionalKitchenEquipmentOther" in input ||
    "deliveryTruckTypeOther" in input ||
    "equipment" in input ||
    "surfaceSize" in input ||
    "surfaceDescription" in input ||
    "humanResourceFieldOther" in input ||
    "qualifications" in input ||
    "tasks" in input ||
    "geographicCoverage" in input
  ) {
    await updateAdTranslation(input);
  }

  let mutationInput = {
    adId: input.adId
  };

  addMaybeValue(input, mutationInput, "category");
  addMaybeValue(input, mutationInput, "isAvailableForRent");
  addMaybeValue(input, mutationInput, "isAvailableForSale");
  addMaybeValue(input, mutationInput, "isAvailableForDonation");
  addMaybeValue(input, mutationInput, "isAvailableForTrade");
  await addMaybeValue(input, mutationInput, "address", getAddressFromGooglePlace);
  await addMaybeValue(input, mutationInput, "galleryItems", GetGalleryItems);
  addMaybeValue(input, mutationInput, "rentPrice");
  addMaybeValue(input, mutationInput, "salePrice");
  addMaybeValue(input, mutationInput, "showAddress");
  addMaybeValue(input, mutationInput, "rentPriceToBeDetermined");
  addMaybeValue(input, mutationInput, "salePriceToBeDetermined");
  addMaybeValue(input, mutationInput, "rentPriceRange");
  addMaybeValue(input, mutationInput, "salePriceRange");
  addMaybeValue(input, mutationInput, "organization");
  addMaybeValue(input, mutationInput, "professionalKitchenEquipment");
  addMaybeValue(input, mutationInput, "deliveryTruckType");
  addMaybeValue(input, mutationInput, "dayAvailability");
  addMaybeValue(input, mutationInput, "eveningAvailability");
  addMaybeValue(input, mutationInput, "availabilityRestriction");
  addMaybeValue(input, mutationInput, "refrigerated");
  addMaybeValue(input, mutationInput, "canSharedRoad");
  addMaybeValue(input, mutationInput, "canHaveDriver");
  addMaybeValue(input, mutationInput, "certification");
  addMaybeValue(input, mutationInput, "allergen");
  addMaybeValue(input, mutationInput, "humanResourceField");

  if (Object.keys(mutationInput).length > 1) {
    let result = await Apollo.instance.defaultClient.mutate({
      mutation: UpdateAd,
      variables: {
        input: mutationInput
      }
    });

    return result;
  }
}

export async function publishAd(adId) {
  let mutationInput = {
    adId
  };

  let result = await Apollo.instance.defaultClient.mutate({
    mutation: PublishAd,
    variables: {
      input: mutationInput
    }
  });

  return result;
}

export async function unpublishAd(adId) {
  let mutationInput = {
    adId
  };

  let result = await Apollo.instance.defaultClient.mutate({
    mutation: UnpublishAd,
    variables: {
      input: mutationInput
    }
  });

  return result;
}

export async function transferAd(input) {
  const result = await Apollo.instance.defaultClient.mutate({
    mutation: TransferAd,
    variables: {
      input
    }
  });

  return result;
}

async function updateAdTranslation(input) {
  let mutationInput = {
    adId: input.adId,
    language: CONTENT_LANG_FR
  };

  addMaybeValue(input, mutationInput, "description");
  addMaybeValue(input, mutationInput, "title");
  addMaybeValue(input, mutationInput, "rentPriceDescription");
  addMaybeValue(input, mutationInput, "salePriceDescription");
  addMaybeValue(input, mutationInput, "donationDescription");
  addMaybeValue(input, mutationInput, "tradeDescription");
  addMaybeValue(input, mutationInput, "conditions");
  addMaybeValue(input, mutationInput, "rentPriceToBeDetermined");
  addMaybeValue(input, mutationInput, "salePriceToBeDetermined");
  addMaybeValue(input, mutationInput, "equipment");
  addMaybeValue(input, mutationInput, "surfaceSize");
  addMaybeValue(input, mutationInput, "surfaceDescription");
  addMaybeValue(input, mutationInput, "professionalKitchenEquipmentOther");
  addMaybeValue(input, mutationInput, "deliveryTruckTypeOther");
  addMaybeValue(input, mutationInput, "isAvailableForRent");
  addMaybeValue(input, mutationInput, "isAvailableForSale");
  addMaybeValue(input, mutationInput, "isAvailableForDonation");
  addMaybeValue(input, mutationInput, "isAvailableForTrade");
  addMaybeValue(input, mutationInput, "humanResourceFieldOther");
  addMaybeValue(input, mutationInput, "qualifications");
  addMaybeValue(input, mutationInput, "tasks");
  addMaybeValue(input, mutationInput, "geographicCoverage");

  let result = await Apollo.instance.defaultClient.mutate({
    mutation: UpdateAdTranslation,
    variables: {
      input: mutationInput
    }
  });

  return result;
}

async function GetGalleryItems(images) {
  let galleryItems = [];

  for (let i = 0; i < images.length; i++) {
    let fileId = "";
    let file = null;
    file = images[i].file;
    if (file.name.indexOf(IMAGES_FOLDER) === -1) {
      fileId = await uploadImage(file);
    } else {
      // Trouver le nom du fichier dans l'url de la source déjà existante
      fileId = file.name.substring(file.name.indexOf(IMAGES_FOLDER) + IMAGES_FOLDER.length);
    }
    galleryItems.push({ src: fileId, alt: images[i].alt });
  }

  return galleryItems;
}

export async function lockAd(adId) {
  let mutationInput = {
    adId
  };

  let result = await Apollo.instance.defaultClient.mutate({
    mutation: LockAd,
    variables: {
      input: mutationInput
    }
  });

  return result;
}

export async function unlockAd(adId) {
  let mutationInput = {
    adId
  };

  let result = await Apollo.instance.defaultClient.mutate({
    mutation: UnlockAd,
    variables: {
      input: mutationInput
    }
  });

  return result;
}
