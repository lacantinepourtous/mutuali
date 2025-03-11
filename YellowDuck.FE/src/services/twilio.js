import Apollo from "@/graphql/vue-apollo";

import { TwilioToken } from "@/graphql/local/twilio.graphql";
import { MyTwilioToken, UpdateTwilioToken } from "./twilio.graphql";

import { LOCAL_STORAGE_TWILIOTOKEN } from "@/consts/local-storage";
import {
  TWILIO_EVENT_PARTICIPANT_JOINED,
  TWILIO_EVENT_MESSAGE_ADDED,
  TWILIO_EVENT_MESSAGE_READ,
  TWILIO_EVENT_MESSAGE_SENT
} from "@/consts/twilio";

import { Client as ConversationsClient } from "@twilio/conversations";

import AuthentificationService from "@/services/authentification";
import ConversationService from "@/services/conversation";

import EventBus from "@/helpers/event-bus";

const ONE_HOUR = 60 * 60 * 1000;
let twilioClient = null;

async function loadLocalState() {
  let token = localStorage.getItem(LOCAL_STORAGE_TWILIOTOKEN);

  if (token !== null) {
    await setToken(token);
  }
}

async function getConversationBySid(sid) {
  let conversation = await twilioClient.getConversationBySid(sid);
  return conversation;
}

async function getUnreadMessagesCount() {
  let conversations = await twilioClient.getSubscribedConversations();
  let unreadMessagesCount = 0;

  await Promise.all(
    conversations.items.map(async (x) => {
      let count = await x.getUnreadMessagesCount();
      unreadMessagesCount += count === null ? 1 : count;
    })
  );

  return unreadMessagesCount;
}

async function getConversationUnreadMessagesCount(sid) {
  let conversation = await twilioClient.getConversationBySid(sid);
  let unreadMessagesCount = await conversation.getUnreadMessagesCount();

  return unreadMessagesCount;
}

async function addMessageToConversation({ sid, body, medias }) {
  let conversation = await getConversationBySid(sid);
  let message = conversation.prepareMessage().setBody(body);

  if (medias) {
    if (!Array.isArray(medias)) {
      medias = [medias];
    }

    for (let media of medias) {
      if (!media) continue;
      message.addMedia(media);
    }
  }

  await message.build().send();
}

async function getConversationMessages(sid) {
  let conversation = await getConversationBySid(sid);

  return loadMessage(conversation);
}

async function loadMessage(conversation) {
  let messages = [];
  let messageResult = null;

  do {
    if (messageResult) {
      messageResult = await messageResult.prevPage();
    } else {
      messageResult = await conversation.getMessages();
    }
    messages = messageResult.items.concat(messages);
  } while (messageResult.hasPrevPage);

  return messages;
}

async function setAllMessagesReadOnConversation(sid, dontEmit) {
  let conversation = await getConversationBySid(sid);
  await conversation.setAllMessagesRead();

  await ConversationService.removeConversationNotification(sid);

  if (!dontEmit) {
    EventBus.$emit(TWILIO_EVENT_MESSAGE_READ);
  }
}

async function getTwilioToken() {
  let result = await Apollo.instance.defaultClient.query({
    query: TwilioToken
  });

  let token = result.data.twilio.token;
  let needNewToken = false;

  if (token === "") {
    needNewToken = true;
  } else {
    let tokenToJSON = JSON.parse(atob(token.split(".")[1]));
    if (isTokenExpire(tokenToJSON.exp, ONE_HOUR)) {
      needNewToken = true;
    }
  }

  if (needNewToken) {
    let result = await Apollo.instance.defaultClient.query({
      query: MyTwilioToken,
      fetchPolicy: "network-only"
    });

    await setToken(result.data.myTwilioToken);

    return result.data.myTwilioToken;
  }

  return token;
}

function isTokenExpire(exp, beforeMs) {
  return exp * 1000 - new Date() < beforeMs;
}

async function setToken(token) {
  localStorage.setItem(LOCAL_STORAGE_TWILIOTOKEN, token);

  await Apollo.instance.defaultClient.mutate({
    mutation: UpdateTwilioToken,
    variables: {
      token
    }
  });
}

async function initTwilioClient() {
  if (!twilioClient) {
    let token = await getTwilioToken();
    twilioClient = await Promise.race([
      new ConversationsClient(token),
      new Promise((resolve) => {
        setTimeout(resolve, 2500, null);
      })
    ]);
    if (twilioClient) {
      twilioClient.on("messageAdded", onMessageAdded);
      twilioClient.on("participantJoined", onParticipantJoined);
    }
  }
}

async function onMessageAdded(event) {
  if (AuthentificationService.getUserId() === event.state.author) {
    EventBus.$emit(TWILIO_EVENT_MESSAGE_SENT, event);
    await setAllMessagesReadOnConversation(event.conversation.sid, true);
  } else {
    EventBus.$emit(TWILIO_EVENT_MESSAGE_ADDED, event);
  }
}

async function onParticipantJoined(event) {
  EventBus.$emit(TWILIO_EVENT_PARTICIPANT_JOINED);
}

export default {
  initTwilioClient,
  loadLocalState,
  getConversationBySid,
  getUnreadMessagesCount,
  getConversationUnreadMessagesCount,
  setAllMessagesReadOnConversation,
  addMessageToConversation,
  getConversationMessages
};
