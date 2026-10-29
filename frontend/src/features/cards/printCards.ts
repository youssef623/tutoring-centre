export async function printCards() {
  await document.fonts.ready;
  window.print();
}
