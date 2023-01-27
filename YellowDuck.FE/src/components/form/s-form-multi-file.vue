<template>
  <div class="my-4 position-relative" style="z-index: 0">
    <template v-for="localFileUrl in localFileUrls">
      <b-badge class="mr-2" :key="localFileUrl.name">
        {{ cleanFileName(localFileUrl.name) }}
        <b-button @click="deleteLocalFile(localFileUrl)" variant="outline-danger" class="px-2" :aria-label="$t('sr.delete-file')">
          <b-icon icon="x" aria-hidden="true"></b-icon>
        </b-button>
      </b-badge>
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
    />
  </div>
</template>

<script>
import SFormFile from "@/components/form/s-form-file";

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
    currentFiles: {
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
    SFormFile
  },
  async mounted() {
    if (this.currentFiles) {
      let values = [];
      for (let i = 0; i < this.currentFiles.length; i++) {
        let file = this.currentFiles[i];
        let result = await fetch(file);
        let blob = await result.blob();
        values.push(
          new File([blob], file, {
            type: blob.type
          })
        );
      }

      this.computedValue = values;
    }
  },
  data() {
    return {
      localFileUrls: [],
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
          files.push(val);
          this.computedValue = files;
        }
      }
    },
    computedValue: {
      get() {
        return this.value;
      },
      set(val) {
        this.onFilePicked(val);
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
    cleanFileName(name) {
      let fileName = name.split("/").last();
      let queryIndex = fileName.indexOf("?");
      if (queryIndex !== -1) {
        return fileName.substring(0, queryIndex);
      }

      return name;
    },
    deleteLocalFile(localFileUrl) {
      this.computedValue = this.computedValue.filter((x) => x !== localFileUrl.file);
      this.localFileUrls = this.localFileUrls.filter((x) => x !== localFileUrl.url);
    },
    onFilePicked(files) {
      if (files !== undefined && files !== null && files.length > 0) {
        this.localFileUrls = [];
        files.map((x) => {
          this.localFileUrls.push({ file: x, name: x.name });
        });
        this.computedFile = null;
      } else {
        this.localFileUrls = [];
      }
    }
  }
};
</script>
