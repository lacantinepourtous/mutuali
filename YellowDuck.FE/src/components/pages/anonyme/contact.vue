<template>
  <div class="w-100">
    <div class="section section--xl">
      <BackLink />
    </div>

    <div class="section section--lg">
      <Heading
        :title="$t('text.contact.title')"
        :description="$t('text.contact.description')"
        :imgSrc="require('@/assets/ambiance/contact-us.jpg')"
        :imgAlt="$t('alt.contact-heading')"
        :imgSrcMobile="require('@/assets/ambiance/contact-us-300x360.jpg')"
      />
    </div>

    <GreenSection>
      <div v-html="$t('text.contact.got-question')"></div>
      <div v-html="$t('text.contact.talk-to-us')"></div>

      <template #form>
        <contact-form origin="Nous joindre" />
      </template>
    </GreenSection>

    <FooterTop />
  </div>
</template>

<script>
import BackLink from "@/components/anonymous/back-link";
import FooterTop from "@/components/anonymous/footer-top";
import GreenSection from "@/components/anonymous/green-section";
import Heading from "@/components/anonymous/alt-heading";
import ContactForm from "@/components/anonymous/contact-form";

export default {
  components: {
    BackLink,
    FooterTop,
    GreenSection,
    Heading,
    ContactForm
  }
};
</script>
