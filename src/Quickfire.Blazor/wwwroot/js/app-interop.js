let beforeUnloadRegistered = false;

export function playTopBarVideo(section) {
  if (typeof window.PlayTopBarVideoAsync === "function") {
    window.PlayTopBarVideoAsync(section);
  }
}

export function playTopBarVideoOther() {
  if (typeof window.PlayTopBarVideoAsyncOther === "function") {
    window.PlayTopBarVideoAsyncOther();
  } else {
    playTopBarVideo("Other");
  }
}

export function initializeTopBarAnimations() {
  if (typeof window.initializeTopBarAnimations === "function") {
    window.initializeTopBarAnimations();
  }
}

export function startLogoLoading() {
  if (typeof window.startLogoLoading === "function") {
    window.startLogoLoading();
  }
}

export function stopLogoLoading() {
  if (typeof window.stopLogoLoading === "function") {
    window.stopLogoLoading();
  }
}

export function blurField(elementId) {
  if (typeof window.blurField === "function") {
    window.blurField(elementId);
    return;
  }

  const element = document.getElementById(elementId);
  if (element) {
    element.blur();
  }
}

export function addRenewalStatusColors(datasource, id) {
  if (typeof window.AddRenewalStatusColors === "function") {
    window.AddRenewalStatusColors(datasource, id);
  }
}

export async function downloadFileFromStream(fileName, contentStreamReference) {
  if (typeof window.downloadFileFromStream === "function") {
    await window.downloadFileFromStream(fileName, contentStreamReference);
  }
}

export function openPdfInNewWindow(base64Pdf) {
  if (typeof window.openPdfInNewWindow === "function") {
    window.openPdfInNewWindow(base64Pdf);
  }
}

export function downloadPdf(base64Pdf, fileName) {
  if (typeof window.downloadPdf === "function") {
    window.downloadPdf(base64Pdf, fileName);
  }
}

export function openWindow(url, target) {
  if (!url) {
    return;
  }

  const safeTarget = target || "_blank";
  const opened = window.open(url, safeTarget, "noopener,noreferrer");
  if (opened && typeof opened.focus === "function") {
    opened.focus();
  }
}

export async function copyToClipboard(text) {
  const value = text ?? "";
  if (navigator.clipboard && navigator.clipboard.writeText) {
    await navigator.clipboard.writeText(value);
    return;
  }

  const textarea = document.createElement("textarea");
  textarea.value = value;
  textarea.setAttribute("readonly", "readonly");
  textarea.style.position = "absolute";
  textarea.style.left = "-9999px";
  document.body.appendChild(textarea);
  textarea.select();
  document.execCommand("copy");
  document.body.removeChild(textarea);
}

export function showAlert(message) {
  window.alert(message);
}

export function showConfirm(message) {
  return window.confirm(message);
}

export function setDragData(filePath) {
  const currentEvent = window.event;
  if (currentEvent && currentEvent.dataTransfer) {
    currentEvent.dataTransfer.setData("text/plain", filePath || "");
  }
}

export function setCheckboxState(taskId, isIndeterminate, isChecked) {
  const checkbox = document.querySelector(`input[data-task-id="${taskId}"]`);
  if (!checkbox) {
    return;
  }

  checkbox.indeterminate = !!isIndeterminate;
  if (!checkbox.indeterminate) {
    checkbox.checked = !!isChecked;
  }
}

export function registerBeforeUnloadAutoSave() {
  if (beforeUnloadRegistered) {
    return;
  }

  beforeUnloadRegistered = true;
  window.addEventListener("beforeunload", () => {
    console.log("Page is about to unload - auto-save should trigger");
  });
}

export function parColInit() {
  if (typeof window.parColInit === "function") {
    return window.parColInit();
  }

  return null;
}

export function parColDispose(id) {
  if (typeof window.parColDispose === "function") {
    window.parColDispose(id);
  }
}

export function localStorageGetItem(key) {
  try {
    return window.localStorage.getItem(key);
  } catch {
    return null;
  }
}

export function localStorageSetItem(key, value) {
  try {
    window.localStorage.setItem(key, value);
  } catch {
  }
}

export function localStorageRemoveItem(key) {
  try {
    window.localStorage.removeItem(key);
  } catch {
  }
}
