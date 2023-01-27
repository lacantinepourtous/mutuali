<template>
  <div class="landing w-100">
    <Heading />

    <div class="green-lighter landing__block landing__block--green-darker landing__block--blob position-relative">
      <div class="landing__bg-blob">
        <svg
          class="landing__blob"
          width="990"
          height="1010"
          viewBox="0 0 990 1010"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
          preserveAspectRatio="xMidYMax"
        >
          <path
            d="M694.904 792.667C567.799 716.771 427.082 1128.95 167.679 1051.21C-91.7567 973.459 -32.3522 645.908 225.536 466.845C483.493 287.741 733.931 -58.8799 926.249 9.3601C1118.63 77.5697 821.371 868.054 694.911 792.672L694.904 792.667Z"
            fill="#597369"
          />
        </svg>
      </div>
      <div class="position-relative">
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
      <h2 class="text-center text-primary pb-md-3 mb-6">{{ $t("text.landingpage.manual") }}</h2>
      <div class="landing__cards">
        <div class="cyan-darker landing__card">
          <StepCard :counter="1" :imgSrc="require('@/assets/icons/kid.svg')" img-alt-pos>
            <template #default>
              {{ $t("text.landingpage.manual.step1").split("##")[0]
              }}<RouterLink class="link-title" :to="{ name: $consts.urls.URL_USER_SUBSCRIBE }">{{
                $t("text.landingpage.manual.step1").split("##")[1]
              }}</RouterLink
              >{{ $t("text.landingpage.manual.step1").split("##")[2] }}
            </template>
          </StepCard>
        </div>
        <div class="cyan-darker landing__card">
          <StepCard :counter="2" :imgSrc="require('@/assets/icons/checklist.svg')">
            <template #default>
              {{ $t("text.landingpage.manual.step2").split("##")[0]
              }}<RouterLink class="link-title" :to="{ name: $consts.urls.URL_CREATE_AD }">{{
                $t("text.landingpage.manual.step2").split("##")[1]
              }}</RouterLink
              >{{ $t("text.landingpage.manual.step2").split("##")[2] }}
            </template>
          </StepCard>
        </div>
        <div class="cyan-darker landing__card">
          <StepCard :counter="3" :imgSrc="require('@/assets/icons/bubbles.svg')">
            <template #default> {{ $t("text.landingpage.manual.step3") }}</template>
          </StepCard>
        </div>
      </div>
    </div>

    <div class="green-lighter landing__block landing__block--green">
      <div class="landing__faq section section--md">
        <LinkList :links="faqLinks">
          <template #title>
            <h2 class="landing__title text-center text-white pb-md-3 mb-6" v-html="$t('text.landingpage.faq-cta-title')"></h2>
          </template>
        </LinkList>
        <div class="text-center mt-6 pt-md-6">
          <a class="landing__btn-dark-green btn btn-primary btn-lg" :href="$t('text.landingpage.faq-cta-url')" target="_blank">
            {{ $t("text.landingpage.faq-cta-btn") }}
          </a>
        </div>
      </div>
    </div>

    <Cta />

    <div class="landing__block landing__block--green-darker">
      <div class="landing__partners section section--xl">
        <Logos :logos="partnersLogos">
          <template #title>
            <h2 class="landing__title text-center text-white h3 pb-md-3 mb-6" v-html="$t('text.landingpage.support-title')"></h2>
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
  & {
    background-color: $gray-100;
  }

  &__block {
    &--green {
      background-color: $green-dark;
    }

    &--green-darker {
      background-color: $green-darker;
    }

    &--blob {
      &::after {
        content: "";
        display: table;
        clear: both;
      }
    }
  }

  &__bg-blob {
    position: absolute;
    overflow: hidden;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
    width: 100%;
    height: 100%;
  }

  &__blob {
    position: absolute;
    bottom: 0;
    left: 0;
    right: 0;
    width: 100%;
  }

  &__tiles {
    margin: $spacer * 6 0;
    display: flex;
    flex-direction: column;
    align-items: center;

    @include media-breakpoint-up(md) {
      margin: $spacer * 8 0;
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
  }
}
</style>
