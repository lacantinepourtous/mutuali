<template>
  <div class="my-4 position-relative" style="z-index: 0">
    <template v-for="(localImage, index) in localImages">
      <div :key="index" class="border border-green p-3 rounded rm-child-margin mb-4">
        <b-card :img-src="localImage.src" :img-alt="localImage.alt ? localImage.alt : ''" img-bottom no-body class="mb-4">
          <div class="d-flex align-items-center justify-content-between p-2 pl-3">
            <p class="m-0">{{ $t("label.preview-image") }}</p>
            <b-button
              @click="deleteLocalImage(localImage)"
              variant="outline-danger"
              class="px-2"
              :aria-label="$t('sr.delete-image')"
            >
              <b-icon icon="x" aria-hidden="true"></b-icon>
            </b-button>
          </div>
        </b-card>
        <s-form-input
          :id="`${id}-${index}-alt`"
          :label="$t('label.alt-text')"
          :name="`${id}-${index}-alt`"
          :value="localImage.alt"
          :description="$t('description.alt-text')"
          @input="(v) => onAltChange(v, index)"
        />
      </div>
    </template>
    <s-form-file
      v-if="!haveReachedLimit"
      :id="id"
      :label="label"
      :name="name"
      :rules="computedRules"
      v-model="computedFile"
      :placeholder="placeholder"
      :drop-placeholder="dropPlaceholder"
      :required="required"
      :description="description"
      accept="image/jpeg, image/png"
    />
  </div>
</template>

<script>
import SFormFile from "@/components/form/s-form-file";
import SFormInput from "@/components/form/s-form-input";

export default {
  props: {
    id: String,
    label: String,
    name: String,
    rules: {
      type: [String, Object]
    },
    value: {
      type: Array
    },
    currentImages: {
      type: Array
    },
    placeholder: String,
    dropPlaceholder: String,
    required: Boolean,
    description: String,
    limit: {
      type: Number,
      default: -1
    }
  },
  components: {
    SFormFile,
    SFormInput
  },
  async mounted() {
    let values = [];

    if (this.currentImages) {
      for (let i = 0; i < this.currentImages.length; i++) {
        let image = this.currentImages[i];

        let result = await fetch(image.src);
        let blob = await result.blob();
        values.push({
          file: new File([blob], image.src, {
            type: blob.type
          }),
          alt: image.alt
        });
      }
      this.computedValue = values;
    }
  },
  data() {
    return {
      localImages: [],
      file: null
    };
  },
  computed: {
    computedFile: {
      get() {
        return this.file;
      },
      set(val) {
        this.file = val;
        if (val !== null) {
          let files = this.computedValue;
          files.push({ file: val, alt: "" });
          this.computedValue = files;
        }
      }
    },
    computedValue: {
      get() {
        return this.value;
      },
      set(val) {
        this.onImagePicked(val);
        this.$emit("input", val);
      }
    },
    computedRules: function() {
      return this.value.length > 0 ? "" : this.rules;
    },
    haveReachedLimit: function() {
      if (this.value && this.limit !== -1) {
        return this.limit <= this.value.length;
      } else {
        return false;
      }
    }
  },
  methods: {
    deleteLocalImage(localImage) {
      this.computedValue = this.computedValue.filter((x) => x.file !== localImage.file);
      this.localImages = this.localImages.filter((x) => x.file !== localImage.file);
    },
    onImagePicked(images) {
      if (images !== undefined && images !== null && images.length > 0) {
        this.localImages = [];
        images.map((x) => {
          const fileReader = new FileReader();
          fileReader.readAsDataURL(x.file);
          fileReader.addEventListener("load", () => {
            this.localImages.push({ file: x.file, src: fileReader.result, alt: x.alt });
          });
        });
        this.computedFile = null;
      } else {
        this.localImages = [];
      }
    },
    onAltChange(value, index) {
      this.computedValue[index].alt = value;
    }
  }
};
</script>
