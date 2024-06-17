<template>
  <div class="table">
    <div class="table__container">
      <div class="table__inside-container">
        <div class="table__content">
          <table class="table__table">
            <slot name="caption">
              <caption v-if="this.caption">
                {{
                  this.caption
                }}
              </caption>
            </slot>
            <thead>
              <tr>
                <th
                  v-for="(col, index) in this.cols"
                  :key="index"
                  scope="col"
                  class="px-4 py-3"
                  :class="col.hasHiddenLabel ? 'sr-only' : ''"
                >
                  {{ col.label }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in this.items" :key="index">
                <slot :item="item" />
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    caption: {
      type: String,
      default: ""
    },
    cols: {
      type: Array,
      default() {
        return [];
      }
    },
    items: {
      type: Array,
      default() {
        return [];
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.table {
  display: flex;
  flex-direction: column;
  &__container {
    margin: -0.5rem 0;
    overflow-x: auto;

    @include media-breakpoint-up(sm) {
      margin: -0.5rem -2rem;
    }

    @include media-breakpoint-up(lg) {
      margin: -0.5rem -3rem;
    }
  }

  &__inside-container {
    padding: 0.5rem 0;
    display: inline-block;
    text-align: middle;
    min-width: 100%;

    @include media-breakpoint-up(sm) {
      padding: 0.5rem 2rem;
    }

    @include media-breakpoint-up(lg) {
      padding: 0.5rem 3rem;
    }
  }

  &__content {
    position: relative;
    width: 100%;
    overflow: hidden;
  }

  &__table {
    min-width: 100%;
    border-top-width: 0px;
    border-bottom-width: 2px;
    border-color: $gray-600;

    thead {
      th {
        border-top-width: 0;
        border-bottom-width: 1px;
      }
    }
  }
}
</style>
