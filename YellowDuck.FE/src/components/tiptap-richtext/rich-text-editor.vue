<template>
  <div class="rich-text-editor">
    <editor-menu-bar :editor="editor" v-slot="{ commands, isActive, getMarkAttrs }">
      <div>
        <!-- Edit link form -->
        <b-modal v-model="linkMenuActive" :title="$t('label.link-url')" hide-footer centered>
          <s-form
            class="my-n4"
            @submit="
              commands.link({ href: linkUrl });
              linkMenuActive = false;
            "
          >
            <s-form-input
              id="link"
              :label="$t('label.link-url')"
              label-sr-only
              rules="required|url"
              :placeholder="$t('placeholder.url')"
              name="link"
              v-model="linkUrl"
              type="url"
            >
              <template #input-group-append="{ sState }">
                <b-input-group-append>
                  <b-button
                    type="submit"
                    v-b-tooltip.hover
                    :title="$t('tooltip.link-set')"
                    :aria-label="$t('tooltip.link-set')"
                    :variant="sState === false ? 'outline-danger' : 'outline-success'"
                  >
                    <b-icon icon="check-circle" aria-hidden="true"></b-icon>
                  </b-button>
                  <b-button
                    type="button"
                    variant="outline-danger"
                    @click="
                      commands.link({ href: null });
                      linkMenuActive = false;
                    "
                    v-b-tooltip.hover
                    :title="$t('tooltip.link-remove')"
                    :aria-label="$t('tooltip.link-remove')"
                  >
                    <b-icon icon="x-circle" aria-hidden="true"></b-icon>
                  </b-button>
                </b-input-group-append>
              </template>
            </s-form-input>
          </s-form>
        </b-modal>
        <!-- Toolbar -->
        <b-button-group class="mb-2 mr-2">
          <rich-text-tool
            @click="commands.bold"
            :is-active="isActive.bold()"
            :label="$t('tooltip.bold-button')"
            icon="type-bold"
          />
          <rich-text-tool
            @click="commands.italic"
            :is-active="isActive.italic()"
            :label="$t('tooltip.italic-button')"
            icon="type-italic"
          />
        </b-button-group>
        <b-button-group class="mb-2 mr-2" v-if="headingOn">
          <rich-text-tool
            @click="commands.heading({ level: 3 })"
            :is-active="isActive.heading({ level: 3 })"
            :label="$t('tooltip.h3-button')"
          >
            <strong>H3</strong>
          </rich-text-tool>
          <rich-text-tool
            @click="commands.heading({ level: 4 })"
            :is-active="isActive.heading({ level: 4 })"
            :label="$t('tooltip.h4-button')"
          >
            <strong>H4</strong>
          </rich-text-tool>
          <rich-text-tool
            @click="commands.heading({ level: 5 })"
            :is-active="isActive.heading({ level: 5 })"
            :label="$t('tooltip.h5-button')"
          >
            <strong>H5</strong>
          </rich-text-tool>
        </b-button-group>
        <b-button-group class="mb-2 mr-2">
          <rich-text-tool
            v-if="linkOn"
            @click="showLinkMenu(getMarkAttrs('link'))"
            :is-active="isActive.link()"
            :label="$t('tooltip.link-button')"
            icon="link45deg"
          />
          <rich-text-tool
            v-if="listOn"
            @click="commands.bullet_list"
            :is-active="isActive.bullet_list()"
            :label="$t('tooltip.ul-button')"
            icon="list-ul"
          />
          <rich-text-tool
            v-if="listOn"
            @click="commands.ordered_list"
            :is-active="isActive.ordered_list()"
            :label="$t('tooltip.ol-button')"
            icon="list-ol"
          />
        </b-button-group>
      </div>
    </editor-menu-bar>
    <editor-content :id="inputId" :class="['form-control', { 'is-invalid': state === false }]" :editor="editor" />
  </div>
</template>

<script>
import { Editor, EditorContent, EditorMenuBar } from "tiptap";
import { Bold, Italic, Link, Heading, ListItem, BulletList, OrderedList } from "tiptap-extensions";

import SForm from "@/components/form/s-form";
import RichTextTool from "@/components/tiptap-richtext/rich-text-tool";
import SFormInput from "@/components/form/s-form-input";

export default {
  components: { EditorContent, EditorMenuBar, SForm, RichTextTool, SFormInput },
  props: {
    value: String,
    state: Boolean,
    listOn: Boolean,
    linkOn: Boolean,
    headingOn: Boolean,
    inputId: String
  },
  data() {
    return {
      linkMenuActive: false,
      linkUrl: null,

      editor: new Editor({
        onUpdate: ({ getHTML }) => {
          this.$emit("input", getHTML());
        },
        onBlur: () => {
          this.$emit("input", this.editor.getHTML());
        },
        extensions: [
          // Basic formatting
          new Bold(),
          new Italic(),
          new Link({
            openOnClick: false
          }),

          // Headings
          new Heading({
            levels: [3, 4, 5]
          }),

          // Lists
          new ListItem(),
          new BulletList(),
          new OrderedList()
        ],
        content: this.value
      })
    };
  },
  computed: {
    isSelectionEmpty() {
      return this.editor.view.state.selection.empty;
    }
  },
  methods: {
    showLinkMenu(attrs) {
      this.linkMenuActive = !this.linkMenuActive;
      this.linkUrl = attrs.href;
    }
  },
  beforeDestroy() {
    this.editor.destroy();
  }
};
</script>

<style lang="scss">
.rich-text-editor {
  .form-control {
    height: auto;

    [contenteditable] {
      min-height: 3rem * $line-height-base;

      &:focus {
        outline: none;
      }
      :first-child {
        margin-top: 0;
      }
      :last-child {
        margin-bottom: 0;
      }
    }
  }
}
</style>
