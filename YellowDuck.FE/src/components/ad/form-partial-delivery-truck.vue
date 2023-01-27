<template>
  <div>
    <s-form-rich-text-editor
      v-model="form.conditions"
      @input="$emit('input', value)"
      name="conditions"
      :label="$t('label.ad-conditions')"
      :description="$t('placeholder.ad-conditionsTypeDeliveryTruck')"
      :headingOn="false"
    />
    <s-form-select
      v-model="form.deliveryTruckType"
      @input="$emit('input', value)"
      id="deliveryTruckType"
      :label="$t('label.ad-deliveryTruckType')"
      name="deliveryTruckType"
      rules="required"
      :placeholder="$t('placeholder.ad-deliveryTruckType')"
      :options="deliveryTruckTypesOptions"
      required
    />
    <s-form-input
      v-if="form.deliveryTruckType === TRUCK_TYPE_OTHER"
      @input="$emit('input', value)"
      v-model="form.deliveryTruckTypeOther"
      id="deliveryTruckTypeOther"
      :label="$t('label.ad-deliveryTruckTypeOther')"
      name="deliveryTruckTypeOther"
      rules="required"
      :placeholder="$t('placeholder.ad-deliveryTruckTypeOther')"
      required
    />
    <s-form-checkbox
      v-model="form.refrigerated"
      @input="$emit('input', value)"
      id="refrigerated"
      :label="$t('label.ad-refrigerated')"
      name="refrigerated"
    />
    <s-form-checkbox
      v-model="form.canSharedRoad"
      @input="$emit('input', value)"
      id="canSharedRoad"
      :label="$t('label.ad-canSharedRoad')"
      name="canSharedRoad"
    />
    <s-form-checkbox
      v-model="form.canHaveDriver"
      @input="$emit('input', value)"
      id="canHaveDriver"
      :label="$t('label.ad-canHaveDriver')"
      name="canHaveDriver"
    />
  </div>
</template>

<script>
import SFormRichTextEditor from "@/components/form/s-form-rich-text-editor";
import SFormInput from "@/components/form/s-form-input";
import SFormCheckbox from "@/components/form/s-form-checkbox";
import SFormSelect from "@/components/form/s-form-select";
import { AdDeliveryTruckType } from "@/mixins/ad-delivery-truck-type";
import { TRUCK_TYPE_OTHER } from "@/consts/delivery-truck-types";

export default {
  mixins: [AdDeliveryTruckType],
  props: {
    value: {
      type: Object,
      default() {
        return {
          conditions: "",
          deliveryTruckType: null,
          deliveryTruckTypeOther: "",
          refrigerated: false,
          canSharedRoad: false,
          canHaveDriver: false
        };
      }
    }
  },
  data: function () {
    return {
      TRUCK_TYPE_OTHER
    };
  },
  computed: {
    form() {
      return this.value;
    }
  },
  components: {
    SFormRichTextEditor,
    SFormInput,
    SFormCheckbox,
    SFormSelect
  }
};
</script>