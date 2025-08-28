export default function haveHtmlContent(content) {
  if (!content) return false;
  let divEl = document.createElement("div");
  divEl.innerHTML = content;
  return divEl.textContent.trim() !== "";
}