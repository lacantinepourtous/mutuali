<template>
  <b-carousel
    :style="`--icon-next: url('${require('@/assets/icons/icon-next.svg')}'); --icon-prev: url('${require('@/assets/icons/icon-prev.svg')}'); --icon-next-light: url('${require('@/assets/icons/icon-next-light.svg')}'); --icon-prev-light: url('${require('@/assets/icons/icon-prev-light.svg')}');`"
    controls
    indicators
    :interval="0"
    :class="[isFullWidth ? 'carousel--full-width' : 'carousel--contained', { 'carousel--light': isLight }]"
  >
    <slot></slot>
  </b-carousel>
</template>

<script>
export default {
  props: {
    isFullWidth: Boolean,
    isLight: Boolean
  }
};
</script>

<style lang="scss">
.carousel-indicators li {
  background-color: $white;
  border: 1px solid $black;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  opacity: 1;
  margin: 0 5px;

  &.active {
    background-color: $black;
    cursor: default;
  }

  &:hover:not(.active) {
    opacity: 0.5;
  }
}

.carousel-control-next,
.carousel-control-prev {
  opacity: 1;

  &:hover {
    opacity: 0.5;
  }
}

.carousel-control-next-icon,
.carousel-control-prev-icon {
  color: black;
  border-color: black;
}

.carousel-control-next-icon {
  background-image: var(--icon-next);
}

.carousel-control-prev-icon {
  background-image: var(--icon-prev);
}

.carousel--full-width {
  .carousel-item {
    width: 100%;
    height: 50vh;
    position: relative;
    background-color: $gray-900;
  }

  .carousel-control-next {
    background: linear-gradient(90deg, rgba(0, 0, 0, 5e-5) 0%, rgba(0, 0, 0, 0.3) 100%);
  }

  .carousel-control-prev {
    background: linear-gradient(90deg, rgba(0, 0, 0, 0.3) 0%, rgba(0, 0, 0, 5e-5) 100%);
  }

  .carousel-control-prev-icon,
  .carousel-control-next-icon {
    padding: $spacer;
    border-width: 1px;
    border-style: solid;
    border-radius: 50%;
    background-size: 17px;
  }

  .carousel-control-next-icon {
    background-position: 6px center;
  }

  .carousel-control-prev-icon {
    background-position: 10px center;
  }
}

.carousel--contained {
  .carousel-control-next,
  .carousel-control-prev {
    width: 32px;
  }

  .carousel-control-next {
    right: $spacer * -0.75;
  }

  .carousel-control-prev {
    left: $spacer * -0.75;
  }
}

.carousel--light {
  .carousel-control-next-icon,
  .carousel-control-prev-icon {
    color: white;
    border-color: white;
  }

  .carousel-control-next-icon {
    background-image: var(--icon-next-light);
  }

  .carousel-control-prev-icon {
    background-image: var(--icon-prev-light);
  }

  .carousel-indicators li {
    background-color: transparent;
    border: 1px solid $white;

    &.active {
      background-color: $white;
    }
  }
}
</style>
