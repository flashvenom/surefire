function openPdfInNewWindow(base64Pdf) {
    const binaryString = window.atob(base64Pdf);
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }

    const blob = new Blob([bytes.buffer], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);
    const printWindow = window.open(url, '_blank');
    printWindow.onload = function () {
        printWindow.focus();
        printWindow.print();
    };
}

function downloadPdf(base64String, fileName) {
    const linkSource = `data:application/pdf;base64,${base64String}`;
    const downloadLink = document.createElement("a");
    downloadLink.href = linkSource;
    downloadLink.download = fileName;
    downloadLink.click();
}

function openPdfInNewWindow(base64Pdf) {
    const binaryString = window.atob(base64Pdf);
    const len = binaryString.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }

    const blob = new Blob([bytes.buffer], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);
    window.open(url, '_blank');
}

function updateUrl(newPathAndQuery) {
    const newUrl = `${window.location.origin}${newPathAndQuery}`;
    window.history.pushState({}, '', newUrl);
}

function blurField(elementId) {https://localhost:7074/uploads/clients/64/c5b98c5b-9226-4d36-915d-360ec89a71de_thumb.jpg
    document.getElementById(elementId).blur();
}

function setDragData(filePath) {
    
    const uriPath = "file:" + filePath.replace(/\\/g, '/').replace(/ /g, '%20');
    console.log("Setting drag data for:", uriPath);
    document.addEventListener("dragstart", function (event) {
        if (event.dataTransfer) {
            event.dataTransfer.setData("text/uri-list", "C:\\Users\\flash\\Desktop\\SORTME\\test\\c5b98c5b-9226-4d36-915d-360ec89a71de.pdf"); // Compatible with Outlook
            //event.dataTransfer.setData("application/octet-stream", "C:\\Users\\flash\\Desktop\\SORTME\\test\\c5b98c5b-9226-4d36-915d-360ec89a71de.pdf"); // Compatible with Outlook
            console.log("Drag data set:", uriPath);
        } else {
            console.error("DataTransfer is not available on dragstart event.");
        }
    });
}

let isDragging = false;

function topbarStartDrag() {
    isDragging = true;
    window.chrome.webview.postMessage({ action: "drag_start" });
}

function topbarStopDrag() {
    isDragging = false;
    window.chrome.webview.postMessage({ action: "drag_stop" });
}

function topbarDrag() {
    window.chrome.webview.postMessage({ action: "drag_move" });
}

//This was used to focus input on the search field whenever the window was focused
//let blazorInstance;
//function registerBlazorInstance(instance) {
//    blazorInstance = instance;
//}
//function invokeFocusSearch() {
//    // Call the Blazor instance method
//    blazorInstance.invokeMethodAsync('FocusSearch')
//        .then(() => console.log("Focused search"))
//        .catch(error => console.error(error));
//}
//// You can call invokeFocusSearch when needed, like in a button click or on window focus
//window.addEventListener('focus', function () {
//    invokeFocusSearch();
//});


//const draggableBar = document.getElementById("menubar");



//console.log("Draggable bar:", draggableBar);



//draggableBar.addEventListener("mousedown", () => {
//    isDragging = true;
//    console.log("Drag:", isDragging);
//    window.chrome.webview.postMessage({action: "drag_start" });
//});

//document.addEventListener("mouseup", () => {
//    console.log("Drag1:", isDragging);
//    isDragging = false;
//});

//document.addEventListener("mousemove", (event) => {
//    console.log("Drag2:", isDragging);
//    if (isDragging) {
//    window.chrome.webview.postMessage({ action: "drag_move" });
//    }
//});
