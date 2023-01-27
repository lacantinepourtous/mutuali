import consts from "@/consts";

export default {
  install(Vue) {
    Vue.prototype.$consts = consts;
  }
};
