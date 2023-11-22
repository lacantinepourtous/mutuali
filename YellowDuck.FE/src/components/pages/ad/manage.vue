<template>
  <div v-if="userProfile" class="w-100 mt-4 mt-md-5">
    <div class="section section--sm mb-4">
      <h1 v-if="isAdmin">{{ $t("title.manage-admin-ads") }}</h1>
      <h1 v-else>{{ $t("title.manage-my-ads") }}</h1>
    </div>
    <template v-if="publishedAds.length > 0">
      <div class="section section--sm">
        <h2 class="font-family-base font-weight-bold">{{ $t("title.ad-publish") }}</h2>
      </div>

      <div class="mb-5 pt-3">
        <ad-snippet
          v-for="ad in publishedAds"
          :key="ad.id"
          :id="ad.id"
          :title="ad.translationOrDefault.title"
          titleTag="h3"
          :image="ad.gallery[0]"
          :price="adPrice(ad)"
          :priceDescription="ad.translationOrDefault.priceDescription"
          :organization="ad.organization"
          sectionWidth="sm"
          smallTitle
          show-ad-detail-btn
          show-manage-btn
          :isPublished="ad.isPublish"
          :canTransfer="isAdmin"
          :isAdminOnly="ad.isAdminOnly"
        />
      </div>
    </template>
    <template v-if="unpublishedAds.length > 0">
      <div class="section section--sm">
        <h2 class="font-family-base font-weight-bold">{{ $t("title-ad-unpublish") }}</h2>
      </div>
      <div class="mb-5 pt-3">
        <ad-snippet
          v-for="ad in unpublishedAds"
          :key="ad.id"
          :id="ad.id"
          :title="ad.translationOrDefault.title"
          :image="ad.gallery[0]"
          :price="adPrice(ad)"
          :priceDescription="adPriceDescription(ad)"
          :organization="ad.organization"
          sectionWidth="sm"
          smallTitle
          show-ad-detail-btn
          show-manage-btn
          :isPublished="ad.isPublish"
          :canTransfer="isAdmin"
          :isAdminOnly="ad.isAdminOnly"
        />
      </div>
    </template>
    <ad-no-content v-if="ads.length === 0" class="my-5 py-sm-5" :isAdmin="isAdmin" />
  </div>
</template>

<script>
import AdNoContent from "@/components/ad/no-content.vue";
import AdSnippet from "@/components/ad/snippet.vue";
import { CONTENT_LANG_FR } from "@/consts/langs";
import { RatingsCriterias } from "@/mixins/ratings-criterias";

export default {
  mixins: [RatingsCriterias],
  components: {
    AdNoContent,
    AdSnippet
  },
  computed: {
    isAdmin() {
      return !this.me || this.me.type === this.$consts.enums.USER_TYPE_ADMIN;
    },
    profileId: function () {
      return this.me.profile.id;
    },
    publishedAds: function () {
      return this.userProfile.user ? this.userProfile.user.ads.filter((x) => x.isPublish) : [];
    },
    unpublishedAds: function () {
      return this.userProfile.user ? this.userProfile.user.ads.filter((x) => !x.isPublish) : [];
    },
    ads: function () {
      return this.userProfile.user ? this.userProfile.user.ads : [];
    }
  },
  methods: {
    adPrice: function (ad) {
      return ad.priceToBeDetermined ? this.$t("price.toBeDetermined") : this.$format.formatMoney(ad.price);
    },
    adPriceDescription: function (ad) {
      return ad.priceToBeDetermined ? "" : ad.translationOrDefault.priceDescription;
    }
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      }
    },
    userProfile: {
      query() {
        return this.$options.query.UserProfileById;
      },
      variables() {
        return {
          id: this.profileId,
          language: CONTENT_LANG_FR
        };
      },
      skip() {
        return !this.me;
      }
    }
  }
};
</script>
<graphql>
query Me {
  me {
    id
    type
    profile {
      id
    }
  }
}
query UserProfileById($id: ID!, $language: ContentLanguage!) {
  userProfile(id: $id) {
    id
    user {
      ads {
        id
        isPublish
        isAdminOnly
        gallery {
          id
          src
          alt
        }
        price
        priceToBeDetermined
        translationOrDefault(language: $language) {
          id
          title
          priceDescription
        }
        organization
      }
    }
  }
}
</graphql>
