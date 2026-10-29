/** This module is imported dynamically only when the scanner's camera starts. */
export async function startDecoder(stream: MediaStream, video: HTMLVideoElement, decoded: (text: string) => void) {
  const { BrowserQRCodeReader } = await import("@zxing/browser");
  const reader = new BrowserQRCodeReader(undefined, { delayBetweenScanAttempts: 100, delayBetweenScanSuccess: 100 });
  return reader.decodeFromStream(stream, video, (result) => { if (result) decoded(result.getText()); });
}
