<template>
  <p class="notification-alert section section--padding-x" :class="`notification-alert--${displayType}`">
    <span class="d-flex align-items-center justify-content-between">
      <span class="d-block mr-2">
        {{ item.text }}
      </span>
      <b-button
        v-if="item.dismissible"
        size="sm"
        pill
        :variant="displayType"
        :aria-label="$t('sr.dismiss-alert')"
        @click="dismiss"
      >
        <b-icon-x-circle-fill aria-hidden="true"></b-icon-x-circle-fill>
      </b-button>
    </span>
  </p>
</template>

<script>
import {
  NOTIFICATION_TYPE_SUCCESS,
  NOTIFICATION_TYPE_WARNING,
  NOTIFICATION_TYPE_ERROR,
  NOTIFICATION_TYPE_INFO
} from "@/consts/notifications";

export default {
  props: {
    item: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      visibility: true
    };
  },
  mounted() {
    if (this.item.duration !== -1) {
      this.timeout = setTimeout(this.dismiss, this.item.duration);
    }
  },
  computed: {
    displayType: function() {
      switch (this.item.type) {
        case NOTIFICATION_TYPE_SUCCESS: {
          return "success";
        }
        case NOTIFICATION_TYPE_WARNING: {
          return "warning";
        }
        case NOTIFICATION_TYPE_ERROR: {
          return "danger";
        }
        case NOTIFICATION_TYPE_INFO:
        default: {
          return "info";
        }
      }
    }
  },
  methods: {
    dismiss: function() {
      clearTimeout(this.timeout);
      this.visibility = false;
    }
  },
  watch: {
    visibility(val) {
      if (!val) {
        this.$emit("dismiss", this.item.id);
      }
    }
  }
};
</script>

<style lang="scss">
.notification-alert {
  line-height: 1.2;
  margin: 0 0 1px;
  padding-top: 0.75rem;
  padding-bottom: 0.75rem;

  .btn.rounded-pill {
    color: currentColor;
    margin-right: -0.2rem;
    padding: 0;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    height: 1rem * $line-height-base;
    width: 1rem * $line-height-base;
  }

  &--success {
    color: $white;
    background: $success;
  }

  &--warning {
    color: $dark;
    background: $warning;
  }

  &--danger {
    color: $white;
    background: $danger;
  }

  &--info {
    color: $white;
    background: $info;
  }
}
</style>
