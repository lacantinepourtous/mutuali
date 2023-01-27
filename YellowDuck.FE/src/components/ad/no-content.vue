<template>
  <div class="no-content my-5 py-sm-4">
    <img class="no-content__img mb-4" alt="" :src="require('@/assets/ambiance/bowl-pile.svg')" />
    <p>{{ $t("profile-detail.no-ads") }}</p>
    <b-button variant="primary" :to="{ name: $consts.urls.URL_CREATE_AD }">{{ $t("nav.create-ad") }} </b-button>
  </div>
</template>

<script>
export default {};
</script>
