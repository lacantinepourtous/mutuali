import Apollo from "@/graphql/vue-apollo";

import { CreateContract, UpdateContract, GetContractIdByConversationId } from "./contract.graphql";
import { uploadFile } from "./file-upload";

import { addMaybeValue } from "@/helpers/graphql";

const FILES_FOLDER = "/files/";

export async function createContract(input) {
  let fileItems = undefined;
  if (input.fileItems.length > 0) {
    fileItems = { value: await GetFileItems(input.fileItems) };
  }

  let result = await Apollo.instance.defaultClient.mutate({
    mutation: CreateContract,
    variables: {
      input: {
        conversationId: input.conversationId,
        datePrecision: input.datePrecision,
        fileItems,
        price: input.price,
        startDate: input.startDate,
        endDate: input.endDate,
        terms: input.terms
      }
    }
  });

  return result;
}

export async function updateContract(input) {
  let mutationInput = {
    contractId: input.contractId
  };

  addMaybeValue(input, mutationInput, "datePrecision");
  addMaybeValue(input, mutationInput, "startDate");
  addMaybeValue(input, mutationInput, "endDate");
  addMaybeValue(input, mutationInput, "price");
  addMaybeValue(input, mutationInput, "terms");

  await addMaybeValue(input, mutationInput, "fileItems", GetFileItems);

  let result = await Apollo.instance.defaultClient.mutate({
    mutation: UpdateContract,
    variables: {
      input: mutationInput
    }
  });

  return result;
}

async function GetFileItems(files) {
  let fileItems = [];

  for (let i = 0; i < files.length; i++) {
    let fileId = "";
    let file = files[i];
    if (file.name.indexOf(FILES_FOLDER) === -1) {
      fileId = await uploadFile(file);
    } else {
      fileId = file.name.substring(file.name.indexOf(FILES_FOLDER) + FILES_FOLDER.length, file.name.indexOf("?"));
    }
    fileItems.push(fileId);
  }

  return fileItems;
}

export async function getContractIdByConversationId(id) {
  let result = await Apollo.instance.defaultClient.query({
    fetchPolicy: "network-only",
    query: GetContractIdByConversationId,
    variables: {
      id
    }
  });

  return result.data.conversation.contract.id;
}
