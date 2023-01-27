<template>
  <div>
    <s-form @submit="submitForm">
      <s-form-datepicker
        v-model="form.startDate"
        id="startDate"
        :label="$t('label.contract-startDate')"
        name="startDate"
        rules="required"
        required
      />
      <s-form-datepicker
        v-model="form.endDate"
        :initialDate="form.startDate"
        id="endDate"
        :label="$t('label.contract-endDate')"
        name="endDate"
        rules="required|dateMin:@startDate"
        required
      />
      <s-form-input
        id="datePrecision"
        :label="$t('label.contract-date-precision')"
        name="datePrecision"
        rules="required"
        v-model="form.datePrecision"
        :placeholder="$t('placeholder.contract-date-precision')"
        required
      />
      <s-form-input
        v-model="form.price"
        id="price"
        type="number"
        :label="$t('label.contract-price')"
        name="price"
        rules="requiredNumeric"
        :placeholder="$t('placeholder.contract-price')"
        required
        append="$"
        :description="$t('description.contract-price')"
      />
      <s-form-rich-text-editor
        v-model="form.terms"
        name="terms"
        rules="emptyHtml"
        :label="$t('label.contract-terms')"
        :description="$t('placeholder.contract-terms')"
        :headingOn="false"
      />
      <s-form-multi-file
        v-model="form.files"
        :currentFiles="form.currentFiles"
        id="files"
        :label="$t('label.contract-files')"
        name="files"
        :placeholder="$t('placeholder.contract-files')"
        :drop-placeholder="$t('placeholder.ad-drop-file')"
      />
      <div v-html="$t('contract-form.practical-info-mutuali')"></div>
      <s-form-checkbox
        v-if="displayDisclaimer"
        id="disclaimer"
        :label="$t('label.disclaimer-accepted')"
        name="disclaimer"
        :rules="{ required: { allowFalse: false } }"
        v-model="disclaimerAccepted"
        required
      />
      <b-button :disabled="!disclaimerAccepted || disabledBtn" block type="submit" variant="primary" size="lg">
        <b-icon icon="pencil" class="mr-2" aria-hidden="true"></b-icon>
        {{ btnLabel }}
      </b-button>
    </s-form>
  </div>
</template>

<script>
import SForm from "@/components/form/s-form";
import SFormDatepicker from "@/components/form/s-form-datepicker";
import SFormInput from "@/components/form/s-form-input";
import SFormRichTextEditor from "@/components/form/s-form-rich-text-editor";
import SFormMultiFile from "@/components/form/s-form-multi-file";
import SFormCheckbox from "@/components/form/s-form-checkbox";

export default {
  props: {
    startDate: {
      type: [String, Date]
    },
    endDate: {
      type: [String, Date]
    },
    datePrecision: {
      type: String,
      default: ""
    },
    price: {
      type: Number
    },
    terms: {
      type: String,
      default: "<p></p>"
    },
    files: {
      type: Array,
      default: null
    },
    btnLabel: {
      type: String,
      required: true
    },
    displayDisclaimer: {
      type: Boolean
    },
    disabledBtn: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      form: {
        startDate: this.startDate,
        endDate: this.endDate,
        datePrecision: this.datePrecision,
        price: this.price,
        terms: this.terms,
        files: this.files || [],
        currentFiles: this.files || []
      },
      disclaimerAccepted: !this.displayDisclaimer
    };
  },
  components: {
    SForm,
    SFormDatepicker,
    SFormInput,
    SFormRichTextEditor,
    SFormMultiFile,
    SFormCheckbox
  },
  methods: {
    submitForm: function() {
      let input = {};

      const maybeEditedFields = ["startDate", "endDate", "datePrecision", "price", "terms"];
      for (let maybeEditedField of maybeEditedFields) {
        if (this[maybeEditedField] !== this.form[maybeEditedField]) {
          input[maybeEditedField] = this.form[maybeEditedField];
        }
      }

      if (
        this.files === null ||
        this.files.length !== this.form.files.length ||
        this.haveDifferentFiles(this.files, this.form.files)
      ) {
        input.fileItems = this.form.files;
      }

      this.$emit("submitForm", input);
    },
    haveDifferentFiles: function(files, formFiles) {
      for (let i = 0; i < files.length; i++) {
        if (files[i] !== formFiles[i].name) {
          return true;
        }
      }

      return false;
    }
  }
};
</script>
