<template>
  <div class="bg-light w-100">
    <div class="section section--xl">
      <BackLink />
    </div>

    <div class="section section--lg">
      <Heading
        :title="$t('text.about-us.title')"
        :description="$t('text.about-us.description')"
        :imgSrc="require('@/assets/ambiance/a-propos.jpg')"
        :imgAlt="$t('alt.about-heading')"
        :imgSrcMobile="require('@/assets/ambiance/a-propos-300x360.jpg')"
      />
    </div>

    <GreenSection>
      <div v-html="$t('text.about-us.cantine')"></div>
      <div v-html="$t('text.about-us.pooling')"></div>
      <div v-html="$t('text.about-us.mutuali')"></div>
    </GreenSection>

    <FooterTop />
  </div>
</template>

<script>
import BackLink from "@/components/anonymous/back-link";
import FooterTop from "@/components/anonymous/footer-top";
import GreenSection from "@/components/anonymous/green-section";
import Heading from "@/components/anonymous/alt-heading";

export default {
  components: {
    BackLink,
    FooterTop,
    GreenSection,
    Heading
  }
};
</script>
