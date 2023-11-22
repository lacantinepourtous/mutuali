<template>
  <div class="w-100">
    <div v-if="title || image || description" class="section section--sm my-4">
      <h1 v-if="title" class="h2 my-4">{{ title }}</h1>
      <b-img v-if="image" class="my-4" :src="image" alt="" height="100" block></b-img>
      <p v-if="description" class="my-4">{{ description }}</p>
    </div>
    <div
      v-if="htmlTitle || htmlDescription"
      class="section section--sm my-4"
      :class="{ 'section--border-top': htmlTitle && title }"
    >
      <h2 v-if="htmlTitle" class="h3 text-primary my-4">{{ htmlTitle }}</h2>
      <div v-if="htmlDescription" class="my-4" v-html="htmlDescription"></div>
    </div>
    <div
      v-if="linkTitle || links || ctas"
      class="section section--sm my-4"
      :class="{ 'section--border-top': linkTitle && (title || htmlTitle) }"
    >
      <h2 v-if="linkTitle" class="h3 text-primary my-4">{{ linkTitle }}</h2>
      <ul v-if="links && links.length > 1" class="my-4">
        <li v-for="link in links" :key="link.text">
          <a :href="link.href">{{ link.text }}</a>
        </li>
      </ul>
      <p v-else-if="links && links[0]" class="my-4">
        <a :key="links[0].text" :href="links[0].href">{{ links[0].text }}</a>
      </p>
      <template v-if="ctas">
        <b-button
          v-for="cta in ctas"
          :key="cta.text"
          :variant="getBtnVariant(cta)"
          :class="{ 'mt-3': cta !== ctas[0] }"
          size="lg"
          block
          @click="cta.action()"
          >{{ cta.text }}</b-button
        >
      </template>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    title: String,
    image: String,
    description: String,

    htmlTitle: String,
    htmlDescription: String,

    linkTitle: String,
    ctas: Array,
    links: Array,

    isAdmin: Boolean
  },
  methods: {
    getBtnVariant(cta) {
      let variant = "";
      if (cta !== this.ctas[0]) {
        variant = "outline-";
      }
      this.isAdmin ? (variant += "admin") : (variant += "primary");
      return variant;
    }
  }
};
</script>
