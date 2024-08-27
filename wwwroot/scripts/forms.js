function openPdfInNewWindow(base64Pdf) {
    // Convert the Base64 string to a binary string
    const binaryString = window.atob(base64Pdf);

    // Convert binary string to an ArrayBuffer
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }

    // Create a Blob from the ArrayBuffer
    const blob = new Blob([bytes.buffer], { type: 'application/pdf' });

    // Create a URL for the Blob and open it in a new window
    const url = URL.createObjectURL(blob);
    window.open(url, '_blank');
}