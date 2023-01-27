import Apollo from "@/graphql/vue-apollo";
import { UpdateShowMenu } from "./app.graphql";

export default {
  updateShowMenu: async function(showMenu) {
    await Apollo.instance.defaultClient.mutate({
      mutation: UpdateShowMenu,
      variables: {
        showMenu
      }
    });
  }
};
