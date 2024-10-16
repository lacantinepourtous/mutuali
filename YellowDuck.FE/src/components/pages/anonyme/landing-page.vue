<template>
  <div class="landing w-100">
    <Heading />

    <div class="landing__block landing__block--green-lighter landing__block--include-margin position-relative">
      <div class="position-relative">
        <div id="introVideo" class="landing__video-anchor"></div>
        <Video :video-id="$t('text.landingpage.video.id')" video-type="Youtube" />
        <div class="section section--xl">
          <div class="landing__tiles">
            <LinkTile
              class="mb-6 mb-md-0"
              reverse
              :title="$t('text.landingpage.got-equipement-to-share.title')"
              :link="{ name: $consts.urls.URL_SHARING_EQUIPMENT }"
              :text="$t('text.landingpage.got-equipement-to-share.text')"
              :imgSrc="require('@/assets/ambiance/kitchen.svg')"
            />
            <LinkTile
              :title="$t('text.landingpage.looking-for-equipment.title')"
              :link="{ name: $consts.urls.URL_LOOKING_FOR_EQUIPMENT }"
              :text="$t('text.landingpage.looking-for-equipment.text')"
              :imgSrc="require('@/assets/ambiance/chefs.svg')"
            />
          </div>
        </div>
      </div>
    </div>

    <div class="landing__manual section section--xl">
      <h2 class="text-center text-primary pb-md-3 mb-6">
        {{ $t("text.landingpage.manual") }}
      </h2>
      <div class="landing__cards">
        <div class="green landing__card">
          <StepCard class="step-card--green" :counter="1" :imgSrc="require('@/assets/icons/kid.svg')" img-alt-pos>
            <template #default>
              {{ $t("text.landingpage.manual.step1").split("##")[0]
              }}<RouterLink class="link-title" :to="{ name: $consts.urls.URL_USER_SUBSCRIBE }">{{
                $t("text.landingpage.manual.step1").split("##")[1]
              }}</RouterLink
              >{{ $t("text.landingpage.manual.step1").split("##")[2] }}
            </template>
          </StepCard>
        </div>
        <div class="yellow landing__card">
          <StepCard class="step-card--yellow" :counter="2" :imgSrc="require('@/assets/icons/checklist.svg')">
            <template #default>
              {{ $t("text.landingpage.manual.step2").split("##")[0]
              }}<RouterLink class="link-title" :to="{ name: $consts.urls.URL_CREATE_AD }">{{
                $t("text.landingpage.manual.step2").split("##")[1]
              }}</RouterLink
              >{{ $t("text.landingpage.manual.step2").split("##")[2] }}
            </template>
          </StepCard>
        </div>
        <div class="red landing__card">
          <StepCard class="step-card--red" :counter="3" :imgSrc="require('@/assets/icons/bubbles.svg')">
            <template #default> {{ $t("text.landingpage.manual.step3") }}</template>
          </StepCard>
        </div>
      </div>
    </div>

    <div class="yellow landing__block landing__block--yellow">
      <div class="landing__faq section section--md">
        <LinkList :links="faqLinks">
          <template #title>
            <h2 class="landing__title text-center pb-md-3 mb-6" v-html="$t('text.landingpage.faq-cta-title')"></h2>
          </template>
        </LinkList>
        <div class="text-center mt-6 pt-md-6">
          <a class="landing__btn-white btn btn-primary btn-lg" :href="$t('text.landingpage.faq-cta-url')" target="_blank">
            {{ $t("text.landingpage.faq-cta-btn") }}
          </a>
        </div>
      </div>
    </div>

    <Cta />

    <div class="landing__block landing__block--gray">
      <div class="landing__partners section section--xl">
        <Logos :logos="partnersLogos">
          <template #title>
            <h2 class="landing__partners-title text-center h3 pb-md-3 mb-6" v-html="$t('text.landingpage.support-title')"></h2>
          </template>
        </Logos>
      </div>
    </div>
  </div>
</template>

<script>
import Cta from "@/components/anonymous/cta";
import Heading from "@/components/anonymous/heading";
import LinkList from "@/components/anonymous/link-list";
import LinkTile from "@/components/anonymous/link-tile";
import Logos from "@/components/anonymous/logos";

import StepCard from "@/components/anonymous/step-card";
import Video from "@/components/anonymous/video";

export default {
  components: {
    Cta,
    Heading,
    LinkList,
    LinkTile,
    Logos,
    StepCard,
    Video
  },
  data() {
    return {
      faqLinks: [
        {
          title: this.$t("text.landingpage.faq-link1-title"),
          href: this.$t("text.landingpage.faq-link1-url")
        },
        {
          title: this.$t("text.landingpage.faq-link2-title"),
          href: this.$t("text.landingpage.faq-link2-url")
        },
        {
          title: this.$t("text.landingpage.faq-link3-title"),
          href: this.$t("text.landingpage.faq-link3-url")
        },
        {
          title: this.$t("text.landingpage.faq-link4-title"),
          href: this.$t("text.landingpage.faq-link4-url")
        }
      ],
      partnersLogos: [
        {
          src: require("@/assets/logos/" + this.$t("text.landingpage.partners1-logo")),
          alt: this.$t("text.landingpage.partners1-alt")
        },
        {
          src: require("@/assets/logos/" + this.$t("text.landingpage.partners2-logo")),
          alt: this.$t("text.landingpage.partners2-alt")
        },
        {
          src: require("@/assets/logos/" + this.$t("text.landingpage.partners3-logo")),
          alt: this.$t("text.landingpage.partners3-alt")
        }
      ]
    };
  }
};
</script>

<style lang="scss" scoped>
.landing {
  &__block {
    &--yellow {
      background-color: $yellow;
    }

    &--green-lighter {
      background-color: $green-lighter;
    }

    &--gray {
      background-color: $gray-900;
    }

    &--include-margin {
      &::after {
        content: "";
        display: table;
        clear: both;
      }
    }
  }

  &__tiles {
    margin: $spacer * 4 0;
    display: flex;
    flex-direction: column;
    align-items: center;

    @include media-breakpoint-up(md) {
      margin: $spacer * 6 0;
      flex-direction: row;
      align-items: flex-start;
      justify-content: space-between;
    }
  }

  &__manual {
    margin-top: $spacer * 4;
    margin-bottom: $spacer * 4;

    @include media-breakpoint-up(md) {
      margin-top: $spacer * 6;
      margin-bottom: $spacer * 6;
    }
  }

  &__cards {
    display: flex;
    flex-direction: column;
    align-items: center;
    margin: 0 0 $spacer * -1;

    @include media-breakpoint-up(md) {
      flex-direction: row;
      align-items: stretch;
      justify-content: space-between;
      margin: 0 $spacer * -1 $spacer * -1;
    }
  }

  &__card {
    margin: 0 0 $spacer;

    @include media-breakpoint-up(md) {
      width: calc(33.33% - #{$spacer} * 2);
      margin: 0 $spacer $spacer;
    }
  }

  &__title {
    strong {
      color: $yellow;
    }
  }

  &__faq {
    padding-top: $spacer * 4;
    padding-bottom: $spacer * 4;

    @include media-breakpoint-up(md) {
      padding-top: $spacer * 6;
      padding-bottom: $spacer * 8;
    }
  }

  &__partners {
    padding-top: $spacer * 3;
    padding-bottom: $spacer * 2;

    @include media-breakpoint-up(md) {
      padding-top: $spacer * 5;
      padding-bottom: $spacer * 4;
    }

    &-title {
      color: $green-lighter;
    }
  }

  &__video-anchor {
    position: relative;
    top: -140px;
  }
}
</style>
