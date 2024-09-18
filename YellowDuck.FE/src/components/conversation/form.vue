<template>
  <div class="hasDropzone" @dragover.prevent="onDragOver" @dragleave.prevent="onDragLeave" @drop.prevent="onDrop">
    <div v-if="uploadedFiles.length > 0" class="file-preview-container">
      <div v-for="(file, index) in uploadedFiles" :key="index" class="file-preview">
        <div v-if="file.isImage">
          <img :src="file.preview" alt="Image preview" class="file-thumb" />
        </div>
        <div v-else class="file-info">
          <div class="file-placeholder">
            <small>{{ file.extension }}</small>
            <br />
            {{ file.name }}
          </div>
        </div>
        <button class="remove-btn" @click="removeFile(index)">x</button>
      </div>
    </div>

    <s-form class="my-n4" @submit="submitForm">
      <s-form-input
        v-model="message"
        id="message"
        name="message"
        :label="inputPlaceholder"
        label-sr-only
        :placeholder="inputPlaceholder"
        :class="{ 'dropzone-active': isDragOver }"
      >
        <template #input-group-append>
          <b-input-group-append>
            <b-button type="button" variant="outline-secondary" :aria-label="$t('sr.upload-files')" @click="triggerFileUpload">
              <b-icon icon="file-earmark-plus" aria-hidden="true"></b-icon>
            </b-button>
            <b-button type="submit" variant="outline-secondary" :aria-label="$t('sr.send-message')">
              <b-icon icon="reply-fill" aria-hidden="true"></b-icon>
            </b-button>
          </b-input-group-append>
        </template>
      </s-form-input>
      <input ref="fileInput" type="file" style="display: none" multiple @change="handleFileUpload" />

      <div v-if="isDragOver" class="dropzone">
        {{ $t("label.drop-files") }}
      </div>
    </s-form>
  </div>
</template>

<script>
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";
import TwilioService from "@/services/twilio";
import NotificationService from "@/services/notification";
import { DANGEROUS_MIME_TYPES } from "@/consts/mime-types";

export default {
  data() {
    return {
      message: "",
      isDragOver: false,
      uploadedFiles: []
    };
  },
  components: {
    SForm,
    SFormInput
  },
  computed: {
    inputPlaceholder() {
      return this.$t("placeholder.new-message", {
        user: this.otherParticipantName
      });
    }
  },
  props: {
    conversationSid: {
      type: String
    },
    otherParticipantName: {
      type: String,
      required: true
    }
  },
  methods: {
    triggerFileUpload() {
      this.$refs.fileInput.click();
    },

    handleFileUpload(event) {
      const files = event.target.files;
      if (files.length > 0) {
        let totalBusted = false;
        let totalSize = this.uploadedFiles.reduce((acc, file) => acc + file.file.size, 0);

        for (let i = 0; i < files.length; i++) {
          const file = files[i];
          const reader = new FileReader();
          const fileExtension = file.name.split(".").pop();
          const isImage = file.type.startsWith("image/");

          if (DANGEROUS_MIME_TYPES.includes(file.type)) {
            NotificationService.showError(this.$t("notification.conversation-no-dangerous-files"));
            continue;
          }

          totalSize += file.size;
          // Max total size is 100MB
          if (totalSize > 100 * 1024 * 1024) {
            totalSize -= file.size;
            totalBusted = true;
            continue;
          }

          if (isImage) {
            reader.onload = (e) => {
              this.uploadedFiles.push({
                file: file,
                name: file.name,
                extension: fileExtension,
                preview: e.target.result,
                isImage: true
              });
            };
            reader.readAsDataURL(file);
          } else {
            this.uploadedFiles.push({
              file: file,
              name: file.name,
              extension: fileExtension,
              isImage: false
            });
          }
        }

        if (totalBusted) {
          NotificationService.showError(this.$t("notification.conversation-file-size-limit"));
        }
      }
    },

    removeFile(index) {
      this.uploadedFiles.splice(index, 1);
    },

    onDrop(event) {
      event.stopPropagation();
      const files = event.dataTransfer.files;
      if (files.length > 0) {
        this.isDragOver = false;
        this.handleFileUpload({ target: { files } });
      }
    },

    onDragOver(event) {
      event.stopPropagation();
      this.isDragOver = true;
    },

    onDragLeave(event) {
      event.stopPropagation();
      this.isDragOver = false;
    },

    async submitForm() {
      if (this.message !== "" || this.uploadedFiles.length > 0) {
        const mediaFiles = await Promise.all(
          this.uploadedFiles.map(async (uploadedFile) => {
            const fileBlob = await uploadedFile.file.arrayBuffer();
            return {
              contentType: uploadedFile.file.type,
              filename: uploadedFile.file.name,
              media: new Blob([fileBlob], { type: uploadedFile.file.type })
            };
          })
        );

        if (this.conversationSid) {
          await TwilioService.addMessageToConversation({
            sid: this.conversationSid,
            body: this.message,
            medias: mediaFiles
          });
        } else {
          this.$emit("sendMessage", this.message);
        }

        this.message = "";
        this.uploadedFiles = [];
      }
    }
  }
};
</script>

<style scoped>
.hasDropzone {
  position: relative;
}
.dropzone {
  pointer-events: none;
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(255, 255, 255, 0.7);
  color: rgb(0, 0, 0);
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 18px;
  z-index: 10;
  border: 2px dashed rgba(0, 0, 0, 0.5);
}

.file-preview-container {
  display: flex;
  flex-wrap: wrap;
  padding-bottom: 15px;
}

.file-preview {
  position: relative;
  margin-right: 10px;
}

.file-thumb,
.file-info {
  width: 100px;
  height: 100px;
  object-fit: cover;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.file-info {
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #f0f0f0;
  text-align: center;
  font-size: 12px;
  padding: 10px;
}

.remove-btn {
  position: absolute;
  top: 0;
  right: 0;
  background-color: rgba(255, 0, 0, 0.7);
  color: white;
  border: none;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  cursor: pointer;
  font-size: 14px;
  line-height: 14px;
}
</style>