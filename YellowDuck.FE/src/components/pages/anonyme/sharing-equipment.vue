<template>
  <div class="bg-light w-100">
    <div class="section section--xl">
      <BackLink />
    </div>

    <div class="section section--lg">
      <Heading
        :title="$t('text.sharing-equipment.title')"
        :subtitle="$t('text.sharing-equipment.subtitle')"
        :description="$t('text.sharing-equipment.description')"
        :imgSrc="require('@/assets/ambiance/sharing-equipment.jpg')"
        :imgAlt="$t('alt.sharing-equipment-heading')"
        :imgSrcMobile="require('@/assets/ambiance/sharing-equipment-300x360.jpg')"
      />
    </div>

    <GreenSection>
      <div v-html="$t('text.sharing-equipment.create-my-first-ad')"></div>
      <RouterLink class="btn btn-primary btn-lg" :to="{ name: $consts.urls.URL_USER_SUBSCRIBE }">{{
        $t("text.sharing-equipment.btn.platform-access")
      }}</RouterLink>
      <div v-html="$t('text.sharing-equipment.why-using-mutuali')"></div>
      <div v-html="$t('text.sharing-equipment.where-to-start')"></div>

      <template #form>
        <contact-form origin="J'ai de l'équipement à partager" />
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
