// Originally inspired by  David Walsh (https://davidwalsh.name/javascript-debounce-function)

const debounce = (func, wait) => {
  let timeout;
  //eslint-disable-next-line
  return function executedFunction(...args) {
    const later = () => {
      timeout = null;
      func(...args);
    };
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
  };
};

export default debounce;
