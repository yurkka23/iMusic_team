export const downloadBlob = (blob: Blob, fileName?: string, openInNewTab?: boolean, revokeUrl?: boolean): void => {
  const url = URL.createObjectURL(blob);
  const anchor = document.createElement('a');

  anchor.href = url;

  if (openInNewTab) {
    anchor.target = '_blank';
  }

  if (fileName) {
    anchor.download = fileName;
  }

  anchor.click();

  if (revokeUrl) {
    URL.revokeObjectURL(url);
  }

  anchor.remove();
};
