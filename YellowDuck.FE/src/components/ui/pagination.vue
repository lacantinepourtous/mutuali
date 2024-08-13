<template>
  <nav class="pagination">
    <div class="pagination__prev">
      <button v-if="value > 1" @click="onPageClick(value - 1)" class="pagination__btn pagination__btn--prev">
        {{ $t("pagination-previous") }}
      </button>
    </div>

    <div class="pagination__numbers">
      <template v-for="(pageItem, index) in paginationItems">
        <span v-if="pageItem === '...'" :key="index" class="pagination__separator">{{ pageItem }}</span>
        <span v-else-if="parseInt(pageItem) === value" class="pagination__current" :key="index" :aria-current="pageItem">
          {{ pageItem }}
        </span>
        <button v-else :key="index" class="pagination__btn" @click="onPageClick(parseInt(pageItem))">
          {{ pageItem }}
        </button>
      </template>
    </div>
    <div class="pagination__next">
      <button v-if="value < totalPages" class="pagination__btn pagination__btn--next" @click="onPageClick(value + 1)">
        {{ $t("pagination-next") }}
      </button>
    </div>
  </nav>
</template>

<script>
const contiguousElements = 3;

export default {
  props: {
    totalPages: {
      type: Number,
      required: true
    },
    value: {
      type: Number,
      required: true
    }
  },
  methods: {
    getPageBoundaries: function () {
      let first = Math.max(this.value - contiguousElements, 1);
      let last = Math.min(this.value + contiguousElements, this.totalPages);

      const maxItems = 1 + contiguousElements * 2;
      const missing = maxItems - (last - first) - 1;

      if (missing > 0) {
        if (first > 1) first -= missing;
        if (last < this.totalPages) last += missing;

        if (first < 1) first = 1;
        if (last > this.totalPages) last = this.totalPages;
      }

      return { first, last };
    },
    onPageClick: function (pageIndex) {
      this.$emit("input", pageIndex);
    }
  },
  computed: {
    paginationItems() {
      const boundaries = this.getPageBoundaries();
      const pagination = [];

      if (boundaries.first > 1) {
        if (boundaries.first > 2) pagination.push("...");
      }

      for (var i = boundaries.first; i <= boundaries.last; i++) {
        pagination.push(i.toString());
      }

      if (boundaries.last < this.totalPages) {
        if (boundaries.last < this.totalPages - 1) pagination.push("...");
      }

      return pagination;
    }
  }
};
</script>

<style lang="scss" scoped>
.pagination {
  border-top: solid 1px $gray-300;
  padding: 0 1rem;
  display: flex;
  align-items: center;
  justify-content: flex-between;

  @include media-breakpoint-up(sm) {
    padding: 0;
  }

  &__prev {
    display: flex;
    flex: 1 1 0%;
    width: 0;
    margin-top: -1px;
  }

  &__next {
    display: flex;
    flex: 1 1 0%;
    width: 0;
    margin-top: -1px;
    justify-content: flex-end;
  }

  &__btn {
    background-color: transparent;
    border: 0;
    border-top: solid 2px transparent;
    padding: 1rem;
    display: inline-flex;
    align-items: center;
    font-size: 14px;
    color: $gray-600;
    transition: color 0.2s ease-in-out, border 0.2s ease-in-out;

    &:hover,
    &:focus {
      color: $gray-900;
      border-color: $gray-900;
    }

    &--prev {
      padding-left: 0;
      padding-right: 0.25rem;
    }

    &--next {
      padding-right: 0;
      padding-left: 0.25rem;
    }
  }

  &__numbers {
    display: none;

    @include media-breakpoint-up(md) {
      margin-top: -1px;
      display: flex;
    }
  }

  &__current,
  &__separator {
    border: 0;
    border-top: solid 2px transparent;
    padding: 1rem;
    display: inline-flex;
    align-items: center;
    font-size: 14px;
  }

  &__current {
    color: $green;
    font-weight: 700;
  }

  &__separator {
    color: $gray-500;
  }
}
</style>
