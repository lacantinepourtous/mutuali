import AuthentificationService from "@/services/authentification";

export default {
  graphQLFetcher
};

function graphQLFetcher(uri, params) {
  return doFetchAndRefreshToken().catch((error) => {
    return doFetchAndRefreshToken();
  });

  function doFetchAndRefreshToken() {
    return doFetch().then(async function(response) {
      if (response.status !== 200) {
        await AuthentificationService.renewToken();

        let r2 = await doFetch();
        if (r2.status !== 200) {
          await AuthentificationService.logout();
          throw new Error("Unauthorized");
        }

        return r2;
      }

      return response;
    });
  }

  async function doFetch() {
    let token = AuthentificationService.getUserToken();
    if (token !== "") {
      params.headers.authorization = `Bearer ${token}`;
    }
    return fetch(uri, params);
  }
}
