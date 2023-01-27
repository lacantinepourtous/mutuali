export default {
  install(Vue, options) {
    Array.prototype.first = function() {
      if (this.length > 0) {
        for (let i in this) return this[i];
      }
      return null;
    };

    Array.prototype.last = function() {
      if (this.length > 0) {
        for (let i in this.reverse()) return this[i];
      }
      return null;
    };

    Array.prototype.flattenDeep = function() {
      return flattenDeep(this);
    };

    Array.prototype.findIndex = function(method) {
      for (let i = 0; i < this.length; i++) {
        if (method(this[i])) return i;
      }

      return -1;
    };
  }
};

function flattenDeep(arr) {
  return arr.reduce((acc, val) => (Array.isArray(val) ? acc.concat(flattenDeep(val)) : acc.concat(val)), []);
}
