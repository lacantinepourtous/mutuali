export default {
  install(Vue) {
    Vue.prototype.$slugiffy = (str) => {
      return slugiffy(str);
    };
    Vue.prototype.$slugiffyAd = (ad) => {
      return `${slugiffy(ad.translationOrDefault.title)}-${ad.id}`;
    };
  }
};

function slugiffy(str) {
  str = str.replace(/^\s+|\s+$/g, ""); // trim
  str = str.toLowerCase();

  // remove accents, swap ñ for n, etc
  let from = "àáäâèéëêìíïîòóöôùúüûñç·/_,:;";
  let to = "aaaaeeeeiiiioooouuuunc------";

  for (let i = 0, l = from.length; i < l; i++) str = str.replace(new RegExp(from.charAt(i), "g"), to.charAt(i));

  str = str
    .replace(/&/g, "-and-")
    .replace(/[^a-z0-9 -]/g, "") // remove invalid chars
    .replace(/\s+/g, "-") // collapse whitespace and replace by -
    .replace(/-+/g, "-"); // collapse dashes

  return str;
}
