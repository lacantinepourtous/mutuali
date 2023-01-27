import Authentification from "./authentification";
import { VUE_APP_ROOT_API } from "@/helpers/env";
import i18n from "@/helpers/i18n";

export async function uploadImage(file) {
  // Make sure auth token will be valid
  await Authentification.renewToken();

  const token = Authentification.getUserToken();
  const data = new FormData();
  data.append("file", file);

  const response = await fetch(`${VUE_APP_ROOT_API}/upload/image`, {
    method: "post",
    headers: {
      Authorization: `Bearer ${token}`
    },
    body: data
  });

  if (response.status === 200) {
    const result = await response.json();
    return result.fileId;
  } else {
    throw new Error(i18n.t("error.image-upload"));
  }
}

export async function uploadFile(file) {
  // Make sure auth token will be valid
  await Authentification.renewToken();

  const token = Authentification.getUserToken();
  const data = new FormData();
  data.append("file", file);

  const response = await fetch(`${VUE_APP_ROOT_API}/upload/file`, {
    method: "post",
    headers: {
      Authorization: `Bearer ${token}`
    },
    body: data
  });

  if (response.status === 200) {
    const result = await response.json();
    return result.fileId;
  } else {
    throw new Error(i18n.t("error.file-upload"));
  }
}