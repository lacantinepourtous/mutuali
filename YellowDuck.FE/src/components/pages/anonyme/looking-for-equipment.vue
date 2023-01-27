<template>
  <div class="bg-light w-100">
    <div class="section section--xl">
      <BackLink />
    </div>

    <div class="section section--lg">
      <Heading
        :title="$t('text.looking-for-equipment.title')"
        :subtitle="$t('text.looking-for-equipment.subtitle')"
        :imgSrc="require('@/assets/ambiance/looking-for-equipment.jpg')"
        :imgAlt="$t('alt.needing-equipment-heading')"
        :imgSrcMobile="require('@/assets/ambiance/looking-for-equipment-300x360.jpg')"
      >
        <div v-html="$t('text.looking-for-equipment.description')"></div>
      </Heading>
    </div>

    <GreenSection>
      <div v-html="$t('text.looking-for-equipment.check-ads')"></div>
      <RouterLink class="btn btn-primary btn-lg" :to="{ name: $consts.urls.URL_LIST_AD }">{{
        $t("text.looking-for-equipment.btn.platform-access")
      }}</RouterLink>
      <div v-html="$t('text.looking-for-equipment.why-using-mutuali')"></div>
      <div v-html="$t('text.looking-for-equipment.got-questions')"></div>

      <template #form>
        <contact-form origin="Je recherche de l'équipement" />
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
