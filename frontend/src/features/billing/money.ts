/** Decimal strings stay strings; only integer BigInts are passed to Intl, with no monetary arithmetic. */
export function formatMoney(amount: string, locale: string): string {
  const parts = /^(-?)(\d+)\.(\d{2})$/.exec(amount);
  if (!parts || parts[2] === undefined || parts[3] === undefined)
    throw new Error("billing.amount_invalid");
  const numericLocale = locale.startsWith("ar") ? "ar-EG-u-nu-arab" : locale;
  const integer = new Intl.NumberFormat(numericLocale, { maximumFractionDigits: 0 }).format(
    BigInt(parts[2]),
  );
  const fraction = new Intl.NumberFormat(numericLocale, {
    useGrouping: false,
    minimumIntegerDigits: 2,
  }).format(BigInt(parts[3]));
  const decimal =
    new Intl.NumberFormat(numericLocale).formatToParts(1.1).find((p) => p.type === "decimal")
      ?.value ?? ".";
  return `${parts[1] ?? ""}${integer}${decimal}${fraction}`;
}
