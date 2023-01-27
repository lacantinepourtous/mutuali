import Vue from "vue";
import VueApollo from "vue-apollo";
import { ApolloClient } from "apollo-client";
import { ApolloLink } from "apollo-link";
import { createHttpLink } from "apollo-link-http";
import { InMemoryCache, IntrospectionFragmentMatcher } from "apollo-cache-inmemory";

import defaults from "@/graphql/local/default-value";
import resolvers from "@/graphql/local/resolvers";
import * as typeDefs from "@/graphql/local/type-defs.graphql";

import GraphqlService from "@/services/graphql";

import { VUE_APP_GRAPHQL_HTTP } from "@/helpers/env";
import GlobalErrorHandler from "@/helpers/global-error-handler";

import introspectionQueryResultData from "@/fragmentTypes.json";

Vue.use(VueApollo);

const AUTH_TOKEN = "apollo-token";
const httpEndpoint = VUE_APP_GRAPHQL_HTTP || "http://localhost:4000/graphql";

let instance = createProvider();

export default {
  instance
};

function createProvider() {
  const fragmentMatcher = new IntrospectionFragmentMatcher({
    introspectionQueryResultData
  });
  const cache = new InMemoryCache({ fragmentMatcher });

  const httpLink = createHttpLink({ uri: httpEndpoint, fetch: GraphqlService.graphQLFetcher });

  const apolloClient = new ApolloClient({
    link: ApolloLink.from([httpLink]),
    cache,
    resolvers,
    typeDefs,
    defaultOptions: {
      httpEndpoint,
      tokenName: AUTH_TOKEN,
      persisting: false,
      websocketsOnly: false,
      ssr: false
    }
  });

  cache.writeData({ data: defaults });

  let vueApollo = new VueApollo({
    defaultClient: apolloClient,
    defaultOptions: {
      $query: {
        fetchPolicy: "cache-and-network"
      }
    },
    errorHandler: (error, component, _, operationType) => {
      error.gqlOperationType = operationType;
      GlobalErrorHandler(error, component);
      throw error;
    }
  });

  return vueApollo;
}
