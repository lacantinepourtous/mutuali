<template>
  <div class="anonymous-cta">
    <div class="anonymous-cta__bg">
      <img class="anonymous-cta__bg-img d-sm-none" :src="require('@/assets/ambiance/parlez-nous-800x630.jpg')" alt="" />
      <img class="anonymous-cta__bg-img d-none d-sm-block" :src="require('@/assets/ambiance/parlez-nous.jpg')" alt="" />
    </div>
    <div class="section section--xl">
      <div class="anonymous-cta__content">
        <b-card class="anonymous-cta__card border-info" header-bg-variant="transparent" header-border-variant="info">
          <template #header>
            <h2 class="anonymous-cta__card-title h3 my-2">
              <svg class="mr-3" width="39" height="33" viewBox="0 0 39 33" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path
                  d="M25.2818 0.223389H13.7186C6.17539 0.223389 0.0253906 6.37339 0.0253906 13.9166C0.0253906 18.4684 2.28039 22.6914 5.97039 25.2334C5.35539 26.3817 4.74039 27.4067 4.12539 28.2667C3.38706 29.2501 3.38706 30.5217 4.08375 31.5467C4.61708 32.3251 5.4771 32.7767 6.42042 32.7767C6.66706 32.7767 6.9121 32.7351 7.15875 32.695C11.6269 31.5067 15.7269 29.3334 18.4756 27.6516H25.2824C32.8256 27.6516 38.9756 21.5016 38.9756 13.9585C38.9756 6.41364 32.8256 0.223463 25.2824 0.223463L25.2818 0.223389ZM25.2818 25.0686H18.1068C17.8602 25.0686 17.6551 25.1502 17.4501 25.2736C15.3185 26.5852 11.0951 29.0052 6.54332 30.1936C6.42001 30.2352 6.29669 30.1936 6.21501 30.0702C6.13333 29.9469 6.13333 29.8236 6.21501 29.7019C7.11668 28.4719 7.97833 26.9952 8.79833 25.2735C9.08501 24.6585 8.83997 23.9618 8.26501 23.6335C4.73995 21.6667 2.56683 17.9353 2.56683 13.9169C2.56683 7.76689 7.56842 2.7653 13.7184 2.7653H25.28C31.43 2.7653 36.4316 7.76689 36.4316 13.9169C36.4332 20.0669 31.4316 25.0685 25.2816 25.0685L25.2818 25.0686Z"
                  fill="#34a49a"
                />
              </svg>
              {{ $t("text.landingpage.cta.title") }}
            </h2>
          </template>
          <ul class="anonymous-cta__card-list" :style="`--icon-check: url('${require('@/assets/icons/check.svg')}');`">
            <li v-for="(item, i) in $t('text.landingpage.cta.list').split('\n')" :key="i">
              {{ item }}
            </li>
          </ul>
          <RouterLink class="anonymous-cta__btn btn btn-info btn-lg w-full d-block" :to="{ name: $consts.urls.URL_CONTACT }">
            <svg
              class="anonymous-cta__btn-icon"
              width="22"
              height="22"
              viewBox="0 0 22 22"
              fill="currentColor"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M19.3898 4.0506C19.3898 6.25104 16.0898 6.25104 16.0898 4.0506C16.0898 1.85016 19.3898 1.85016 19.3898 4.0506Z"
                fill="white"
              />
              <path
                d="M10.81 3.06066C11.7226 3.06195 12.6275 3.2205 13.4861 3.52988C13.5969 2.62882 13.9914 1.78837 14.6114 1.12709C11.577 -0.0446769 8.17248 0.249221 5.38299 1.91985C2.59218 3.59176 0.728343 6.45603 0.330033 9.68376C-0.0695642 12.9116 1.04158 16.1432 3.34128 18.4443C5.63967 20.744 8.87142 21.8565 12.0992 21.4594C15.3269 21.0623 18.1926 19.1996 19.8657 16.4114C21.5389 13.6218 21.8328 10.2173 20.6636 7.1816C20.0036 7.80035 19.1657 8.19351 18.2686 8.30436C19.1464 10.7574 18.761 13.4863 17.2386 15.6003C16.509 14.5846 15.5499 13.757 14.44 13.1821C15.5345 11.7912 15.7394 9.89753 14.966 8.30564C14.1939 6.71494 12.5799 5.70303 10.81 5.70303C9.04002 5.70303 7.42615 6.71494 6.65395 8.30564C5.8805 9.89763 6.08546 11.7911 7.17991 13.1821C6.06616 13.7531 5.10579 14.582 4.37877 15.6003C3.23925 14.0148 2.7262 12.0644 2.93759 10.1243C3.149 8.18298 4.06939 6.38871 5.52215 5.08521C6.97491 3.78197 8.85812 3.0601 10.8097 3.0601L10.81 3.06066ZM8.82997 10.3207C8.82997 9.52014 9.31207 8.79827 10.052 8.49147C10.7919 8.18467 11.644 8.35483 12.2099 8.92073C12.7758 9.48661 12.946 10.3387 12.6392 11.0786C12.3324 11.8186 11.6105 12.3007 10.81 12.3007C9.71685 12.3007 8.82997 11.4138 8.82997 10.3207ZM10.81 14.9407C12.6546 14.9419 14.3651 15.9075 15.3217 17.4853C13.9966 18.4082 12.4213 18.9019 10.806 18.9019C9.19212 18.9019 7.61686 18.4082 6.29161 17.4853C7.24551 15.901 8.96125 14.9342 10.8096 14.9407H10.81Z"
              />
            </svg>
            {{ $t("text.landingpage.cta.btn") }}
          </RouterLink>
        </b-card>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      checkIcon: require("@/assets/icons/check.svg")
    };
  }
};
</script>

<style lang="scss" scoped>
$card-width: 400px;
.anonymous-cta {
  position: relative;
  overflow: hidden;
  padding: $spacer * 4 0;

  @include media-breakpoint-up(md) {
    padding: $spacer * 12 0 $spacer * 8;
  }

  &__bg {
    position: absolute;
    top: 0;
    left: 0;
    bottom: 0;
    width: 100%;
    height: 100%;
    display: none;

    @include media-breakpoint-up(sm) {
      display: block;
      left: -120px;
    }

    @include media-breakpoint-up(lg) {
      left: 0;
    }
  }

  &__bg-img {
    position: absolute;
    top: 0;
    left: 0;
    bottom: 0;
    width: 100%;
    height: 100%;
    object-fit: contain;
    object-position: left center;
  }

  &__content {
    max-width: $card-width;
    margin-left: auto;
    margin-right: auto;

    @include media-breakpoint-up(sm) {
      margin-right: 0;
    }
  }

  &__card {
    border-radius: 20px;
  }

  &__card-title {
    display: flex;
    align-items: center;
  }

  &__card-list {
    padding-left: 24px;
    list-style-type: none;
    li {
      position: relative;
      margin-bottom: 8px;

      &:before {
        content: "";
        display: inline-block;
        background-image: var(--icon-check);
        background-repeat: no-repeat;
        background-position: center;
        background-size: contain;
        width: 16px;
        height: 16px;
        position: absolute;
        top: 4px;
        left: -24px;
      }
    }
  }

  &__btn {
    position: relative;
  }

  &__btn-icon {
    position: absolute;
    transform: translate(0, -50%);
    top: 50%;
    left: $spacer * 0.75;
  }
}
</style>
