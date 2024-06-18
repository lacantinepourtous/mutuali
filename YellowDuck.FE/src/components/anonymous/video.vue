<template>
  <div class="anonymous-video section section--md">
    <div ref="video" class="anonymous-video__video">
      <img
        ref="poster"
        class="anonymous-video__poster anonymous-video__poster--mobile"
        :alt="$t('text.landingpage.video.alt')"
        :src="require('@/assets/ambiance/woman-in-kitchen-810x456.jpg')"
      />
      <img
        ref="poster"
        class="anonymous-video__poster anonymous-video__poster--desktop"
        :alt="$t('text.landingpage.video.alt')"
        :src="require('@/assets/ambiance/woman-in-kitchen.jpg')"
      />
      <button
        class="anonymous-video__play-btn"
        :class="{ 'd-none': showVideo }"
        :aria-label="$t('sr.play-video')"
        @click.prevent="checkForConsent"
      >
        <svg fill="currentColor" width="82" height="82" viewBox="0 0 82 82" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
          <path
            d="M41 0C18.379 0 0 18.379 0 41C0 63.621 18.379 82 41 82C63.621 82 82 63.621 82 41C82 18.379 63.621 0 41 0ZM41 4C61.457 4 78 20.543 78 41C78 61.457 61.457 78 41 78C20.543 78 4 61.457 4 41C4 20.543 20.543 4 41 4ZM30.375 22C28.0625 22.0508 26.0586 23.9102 26 26.5625V55.4065C26.0781 58.9456 29.6133 61.0901 32.7188 59.4065L57.7188 44.9375C59.0782 44.1484 60 42.6953 60 41C60 39.3047 59.0781 37.8516 57.7188 37.0625L32.7188 22.5935C31.9415 22.1716 31.1446 21.9841 30.375 21.9998V22Z"
          />
        </svg>
      </button>
      <div v-if="askForConsent" class="anonymous-video__consent">
        <div class="vertical-centered">
          <p class="text-white text-center">{{ $t("text.landingpage.video.cookie-notice") }}</p>
          <p class="text-white text-center">{{ $t("text.landingpage.video.youtube-consent") }}</p>
          <button @click="initializeVideo" class="btn btn-primary" @click.prevent="initializeVideo">
            {{ $t("text.landingpage.video.consent-button") }}
          </button>
        </div>
      </div>
    </div>
    <p class="text-center mt-4 mb-0">{{ $t("text.landingpage.video.description") }}</p>
  </div>
</template>

<script>
export default {
  props: {
    videoId: {
      type: String,
      default: ""
    },
    videoType: {
      type: String,
      default: ""
    }
  },
  data() {
    return {
      showVideo: false,
      askForConsent: false,
      videoEmbed: false
    };
  },
  methods: {
    checkForConsent() {
      if (this.$termsFeed.hasConsent("targeting")) {
        this.initializeVideo();
      } else {
        this.askForConsent = true;
      }
    },
    initializeVideo() {
      if (this.videoEmbed) return;
      this.videoEmbed = true;
      this.askForConsent = false;
      const video = this.$refs.video;
      const poster = this.$refs.poster;
      const videoId = this.videoId;
      const videoType = this.videoType;

      if (!videoId || !videoType) return;

      if (poster) {
        poster.classList.add("d-none", "d-sm-none");
      }

      const iframe = document.createElement("iframe");
      iframe.className = "anonymous-video__iframe";

      if (videoType === "Youtube") {
        iframe.src = `https://www.youtube.com/embed/${videoId}?autoplay=1`;
      } else if (videoType === "Vimeo") {
        iframe.src = `https://player.vimeo.com/video/${videoId}?autoplay=1`;
      }

      iframe.width = "100%";
      iframe.height = "100%";
      iframe.frameBorder = 0;
      iframe.allow = "accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture";
      iframe.allowfullscreen = "";

      video.appendChild(iframe);
    }
  }
};
</script>

<style lang="scss" scoped>
$video-overlap: 100px;
.anonymous-video {
  & {
    margin-top: $video-overlap;
  }

  &__video {
    position: relative;
    height: 0;
    padding-bottom: 56.25%;
    border-radius: 20px;
    overflow: hidden;
    top: $video-overlap * -1;
    margin-bottom: $video-overlap * -1;
  }

  &__poster {
    position: absolute;
    top: 0;
    left: 0;
    bottom: 0;
    right: 0;
    width: 100%;
    height: 100%;
    object-fit: cover;

    &--mobile {
      @include media-breakpoint-up(sm) {
        display: none;
      }
    }

    &--desktop {
      display: none;
      @include media-breakpoint-up(sm) {
        display: block;
      }
    }
  }

  &__play-btn {
    position: absolute;
    transform: translate(-50%, -50%);
    top: 50%;
    left: 50%;
    background-color: transparent;
    outline: none;
    border: 0;
    color: $yellow;
    transition: color 0.2s ease-in-out;

    &:hover,
    &:focus {
      color: $gray-900;
    }
  }

  &__consent {
    position: absolute;
    top: 0;
    left: 0;
    bottom: 0;
    right: 0;
    background-color: rgba(0, 0, 0, 0.95);
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    padding: $spacer * 2;
    z-index: 1;

    .vertical-centered {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;

      p {
        margin-bottom: $spacer;
      }

      button {
        margin-top: $spacer;
      }
    }
  }
}
</style>

<style lang="scss">
/* Unscoped style for appearing iframe */
.anonymous-video {
  &__iframe {
    position: absolute;
    top: 0;
    left: 0;
    bottom: 0;
    right: 0;
    width: 100%;
    height: 100%;
  }
}
</style>
