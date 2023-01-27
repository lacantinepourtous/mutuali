export default {
  install(Vue, options) {
    Date.prototype.isToday = function() {
      let today = new Date(Date.now());
      return (
        this.getDate() === today.getDate() && this.getFullYear() === today.getFullYear() && this.getMonth() === today.getMonth()
      );
    };
  }
};
