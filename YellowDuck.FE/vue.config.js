module.exports = {
  css: {
    loaderOptions: {
      sass: {
        additionalData: `@import '@/scss/_config';`
      }
    }
  },
  devServer: {
    host: "localhost",
    port: 8082,
    https: false,
    transportMode: "ws"
  },
  pluginOptions: {
    i18n: {
      locale: "fr",
      fallbackLocale: "fr",
      localeDir: "locales",
      enableInSFC: false
    }
  },
  chainWebpack: (config) => {
    config.module
      .rule()
      .resourceQuery(/blockType=graphql/)
      .use()
      .loader("vue-graphql-loader")
      .end();

    // Disable CssNano calc() optimisation
    // Inspect Webpack configuration using:
    // vue-cli-service inspect --modern --mode production  > output.js
    if (config.get("mode") === "production") {
      config.plugin("optimize-css").tap((args) => {
        args[0].cssnanoOptions.preset[1].calc = false;
        return args;
      });
    }
  }
};
