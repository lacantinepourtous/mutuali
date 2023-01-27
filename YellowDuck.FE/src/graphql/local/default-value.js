export default {
  app: {
    __typename: "LocalApp",
    showMenu: false
  },
  user: {
    __typename: "LocalUser",
    isConnected: false,
    accessToken: "",
    refreshToken: ""
  },
  twilio: {
    __typename: "Twilio",
    token: ""
  },
  notifications: []
};
