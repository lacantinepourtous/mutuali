<template>
  <div class="no-content my-5 py-sm-4">
    <img class="no-content__img mb-4" alt="" :src="require('@/assets/ambiance/bowl-pile.svg')" />
    <p>{{ $t("profile-detail.no-ads") }}</p>
    <b-button v-if="isAdmin" variant="admin" :to="{ name: $consts.urls.URL_PREPARE_AD }">{{ $t("nav.prepare-ad") }} </b-button>
    <b-button v-else variant="primary" :to="{ name: $consts.urls.URL_CREATE_AD }">{{ $t("nav.create-ad") }} </b-button>
  </div>
</template>

<script>
export default {
  props: {
    isAdmin: {
      type: Boolean,
      required: false,
      default: false
    }
  }
};
</script>
