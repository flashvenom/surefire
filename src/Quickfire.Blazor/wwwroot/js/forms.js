(() => {
    window.downloadFileFromStream = async (fileName, contentStreamReference) => {
        if (!contentStreamReference) {
            console.warn("downloadFileFromStream called without contentStreamReference");
            return;
        }

        const arrayBuffer = await contentStreamReference.arrayBuffer();
        const blob = new Blob([arrayBuffer]);
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement("a");
        anchor.href = url;
        anchor.download = fileName || "download";
        anchor.style.display = "none";
        document.body.appendChild(anchor);
        anchor.click();
        document.body.removeChild(anchor);
        URL.revokeObjectURL(url);
    };

    const registry = {};                // keeps controllers by id

    /** init – returns a token string */
    window.parColInit = () => {

        const scrollEl = document.querySelector('.page-content') || window;
        const cols = [...document.querySelectorAll('.parCol')];
        if (!cols.length) return null;

        const id = `pcol-${Date.now()}-${Math.random().toString(36).slice(2, 6)}`;

        /* ---------- helpers ---------- */
        const vH = () =>
            (scrollEl === window) ? innerHeight : scrollEl.clientHeight;

        let speeds = [];                         // 0‥1

        function recalc() {
            const extra = cols.map(c => Math.max(0, c.scrollHeight - vH()));
            const max = Math.max(...extra) || 1;
            speeds = extra.map(e => e / max);
            onScroll();
        }

        function onScroll() {
            const y = (scrollEl === window) ? (scrollY || pageYOffset)
                : scrollEl.scrollTop;
            cols.forEach((c, i) => {
                const off = y * (1 - speeds[i]);
                c.style.transform = `translateY(${off}px)`;
            });
        }

        /* ---------- listeners ---------- */
        scrollEl.addEventListener('scroll', onScroll, { passive: true });
        addEventListener('resize', recalc);

        recalc();   // first run

        /* ---------- store dispose handle ---------- */
        registry[id] = () => {
            scrollEl.removeEventListener('scroll', onScroll);
            removeEventListener('resize', recalc);
            cols.forEach(c => c.style.transform = '');
            delete registry[id];
        };

        return id;
    };

    /** dispose – called by .NET with the token */
    window.parColDispose = id => {
        if (registry[id]) registry[id]();
    };
})();

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
function downloadFileFromBytes(base64Data, contentType, fileName) {
    // Create a blob from the base64 data
    const byteCharacters = atob(base64Data);
    const byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += 512) {
        const slice = byteCharacters.slice(offset, offset + 512);

        const byteNumbers = new Array(slice.length);
        for (let i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }

    const blob = new Blob(byteArrays, { type: contentType });

    // Create download link
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;

    // Append to body, click to download, then clean up
    document.body.appendChild(link);
    link.click();

    // Cleanup
    setTimeout(() => {
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
    }, 100);
}
function updateUrl(newPathAndQuery) {
    const newUrl = `${window.location.origin}${newPathAndQuery}`;
    window.history.pushState({}, '', newUrl);
}
function blurField(elementId) {
    document.getElementById(elementId).blur();
}
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

/**
 * Listeners
 */

window.addEventListener('visibilitychange', function () {
    // Focuses on the FireSearch field when Surefire is brought back into focus
    var fluentTextField = document.getElementById('fSearcher');
    document.getElementById('fSearcher').focus();

    if (fluentTextField && fluentTextField.shadowRoot) {
        var input = fluentTextField.shadowRoot.querySelector('input');

        if (input) {
            input.focus();
        }

    } else {
        fluentTextField.focus();
    }
});

window.scrollMessagesToBottom = function () {
    // Find the messages list container
    Console.Log("Scrolling messages to bottom");
    const messagesContainer = document.querySelector('.messages-list');
    if (messagesContainer) {
        // Scroll to the bottom
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }
};

/**
 * Logo Animation System
 */
let logoImagesPreloaded = false;
let logoImages = [];
let logoAnimationInterval = null;
let logoLoadingInterval = null;
let isLogoLoading = false;

function preloadLogoImages() {
    if (logoImagesPreloaded) return;

    logoImages = [];
    let loadedCount = 0;
    const totalImages = 74; // 0-73

    for (let i = 0; i <= 73; i++) {
        const img = new Image();
        const frameNumber = i.toString().padStart(5, '0');
        img.src = `/img/home/spinq/qspin${frameNumber}.png`;

        img.onload = () => {
            loadedCount++;
            if (loadedCount === totalImages) {
                logoImagesPreloaded = true;
                console.log('Logo animation images preloaded successfully');
            }
        };

        logoImages.push(img);
    }
}

// Map section name -> title image path helper
function getTitleImagePath(section) {
    if (!section) return null;
    const key = section.toLowerCase();
    const map = {
        'home': '/img/topbar/home.png',
        'clients': '/img/topbar/clients.png',
        'renewals': '/img/topbar/renewals.png',
        'contacts': '/img/topbar/searchContacts.png',
        'leads': '/img/topbar/pursueLeads.png',
        'carriers': '/img/topbar/hustleCarriers.png',
        'locations': '/img/topbar/locations.png',
        'policies': '/img/topbar/servicePolicies.png',
        'agent': '/img/topbar/agent.png',
        'profile': '/img/topbar/settings.png',
        'settings': '/img/topbar/settings.png',
        'system': '/img/topbar/settings.png',
        'other': '/img/topbar/home.png' // default to home to avoid broken image
    };
    // Fallback to home to avoid broken images
    return map[key] || '/img/topbar/home.png';
}

// Preload common title images
function preloadTitleImages() {
    if (titleImagesPreloaded) return;
    const sections = ['home', 'clients', 'renewals', 'contacts', 'leads', 'carriers', 'locations', 'policies', 'agent', 'profile', 'settings', 'system'];
    let loaded = 0;
    const total = sections.length;
    sections.forEach(s => {
        const img = new Image();
        img.onload = () => {
            loaded++;
            if (loaded >= total) titleImagesPreloaded = true;
        };
        img.onerror = () => {
            loaded++;
            if (loaded >= total) titleImagesPreloaded = true;
        };
        const src = getTitleImagePath(s);
        img.src = src;
        titleImages[s] = img;
    });
}

// Animate the title change (slide/fade out, swap, slide/fade in)
function animateTitleToSection(section, immediate = false) {
    const titleImg = document.getElementById('topbar-title-img');
    const titleLayer = document.getElementById('topbar-title-layer');
    if (!titleImg || !titleLayer) return;

    const src = getTitleImagePath(section);
    if (!src) {
        // No image known: hide
        titleImg.style.opacity = '0';
        currentTitleSection = null;
        return;
    }

    // If the same section, ensure visible and no re-anim
    if (currentTitleSection === section) {
        if (immediate) {
            titleImg.style.transition = 'none';
            titleImg.style.opacity = '1';
            titleImg.style.transform = 'translateY(-50%) translateX(0)';
            // Force reflow then restore transition
            void titleImg.offsetWidth;
            titleImg.style.transition = 'transform 250ms cubic-bezier(0.7, 0, 0.2, 1), opacity 200ms ease';
        }
        return;
    }

    // Clear any pending timers
    if (titleAnimTimeout) {
        clearTimeout(titleAnimTimeout);
        titleAnimTimeout = null;
    }

    if (immediate || titleImg.getAttribute('src') === '') {
        // First-time or immediate: just set and fade in
        titleImg.style.transition = 'none';
        titleImg.style.opacity = '0';
        titleImg.style.transform = 'translateY(-50%) translateX(120px)';
        // Fallback to home.png if image missing
        titleImg.onerror = () => {
            if (!titleImg.dataset.fallback) {
                titleImg.dataset.fallback = '1';
                titleImg.src = '/img/topbar/home.png';
            }
        };
        titleImg.setAttribute('src', src);
        // Force reflow then animate in
        void titleImg.offsetWidth;
        titleImg.style.transition = 'transform 250ms cubic-bezier(0.7, 0, 0.2, 1), opacity 200ms ease';
        titleImg.style.opacity = '1';
        titleImg.style.transform = 'translateY(-50%) translateX(0)';
        currentTitleSection = section;
        return;
    }

    // Slide/fade out
    titleImg.style.transition = 'transform 250ms cubic-bezier(0.7, 0, 0.2, 1), opacity 200ms ease';
    titleImg.style.transform = 'translateY(-50%) translateX(-20px)';
    titleImg.style.opacity = '0';

    titleAnimTimeout = setTimeout(() => {
        // Swap image while hidden
        // Reset fallback flag and set error handler before swapping
        delete titleImg.dataset.fallback;
        titleImg.onerror = () => {
            if (!titleImg.dataset.fallback) {
                titleImg.dataset.fallback = '1';
                titleImg.src = '/img/topbar/home.png';
            }
        };
        titleImg.setAttribute('src', src);
        // Prepare from right
        titleImg.style.transition = 'none';
        titleImg.style.transform = 'translateY(-50%) translateX(120px)';
        // Force reflow
        void titleImg.offsetWidth;
        // Slide/fade in
        titleImg.style.transition = 'transform 1000ms cubic-bezier(0.2, 0.9, 0.4, 1), opacity 500ms ease';
        titleImg.style.opacity = '1';
        titleImg.style.transform = 'translateY(-50%) translateX(0)';
        currentTitleSection = section;
        titleAnimTimeout = null;
    }, 160);
}

// Compute and set the left position of the title so it sits just right of the Quickfire logo
function updateTitlePosition() {
    try {
        const topBar = document.getElementById('sf-top-bar');
        const titleImg = document.getElementById('topbar-title-img');
        const logoEl = document.querySelector('.surefire-main-logo');
        if (!topBar || !titleImg || !logoEl) return;

        const barRect = topBar.getBoundingClientRect();
        const logoRect = logoEl.getBoundingClientRect();
        const margin = 12; // px spacing from logo
        const left = Math.max(0, (logoRect.right - barRect.left) + margin);
        titleImg.style.left = `${Math.round(left)}px`;
    } catch { /* noop */ }
}

function playLogoSequenceWithTopBar() {
    const logoImg = document.querySelector('.app-logo');
    if (!logoImg) return;

    // If an animation is already running, let it finish instead of restarting
    if (logoAnimationInterval) {
        return;
    }

    // Stop logo loading if active
    stopLogoLoading();

    let currentFrame = 0;
    const totalFrames = 74; // 0-73
    const frameDuration = 30; // 30 FPS to match fire animation

    logoAnimationInterval = setInterval(() => {
        const frameNumber = currentFrame.toString().padStart(5, '0');
        const imageUrl = `/img/home/spinq/qspin${frameNumber}.png`;
        logoImg.src = imageUrl;

        currentFrame++;

        if (currentFrame >= totalFrames) {
            clearInterval(logoAnimationInterval);
            logoAnimationInterval = null;
            // Return to first frame
            logoImg.src = '/img/home/spinq/qspin00000.png';
        }
    }, frameDuration);
}

function startLogoLoading() {
    if (isLogoLoading) return;

    const logoImg = document.querySelector('.app-logo');
    if (!logoImg) return;

    // Stop any existing animations
    stopLogoAnimation();
    
    isLogoLoading = true;
    let currentFrame = 0;
    const totalFrames = 74; // 0-73
    const frameDuration = 30; // Slightly slower for loading state

    logoLoadingInterval = setInterval(() => {
        const frameNumber = currentFrame.toString().padStart(5, '0');
        const imageUrl = `/img/home/spinq/qspin${frameNumber}.png`;
        logoImg.src = imageUrl;

        currentFrame++;

        // Loop continuously
        if (currentFrame >= totalFrames) {
            currentFrame = 0;
        }
    }, frameDuration);
}

function stopLogoLoading() {
    if (!isLogoLoading) return;

    isLogoLoading = false;
    
    if (logoLoadingInterval) {
        clearInterval(logoLoadingInterval);
        logoLoadingInterval = null;
    }

    // Always end at first frame
    const logoImg = document.querySelector('.app-logo');
    if (logoImg) {
        logoImg.src = '/img/home/spinq/qspin00000.png';
    }
}

function stopLogoAnimation() {
    if (logoAnimationInterval) {
        clearInterval(logoAnimationInterval);
        logoAnimationInterval = null;
    }
}

/**
 * Top Bar Fire Animation System
 */
let isLoadingMode = false;
let currentGradientPosition = 0; // 0=Home, 1=Clients, 2=Renewals
let fireImagesPreloaded = false;
let fireImages = [];
let isFireSequencePlaying = false;
let fireSequenceInterval = null;
// Title layer state
let currentTitleSection = null;
let titleImagesPreloaded = false;
let titleImages = {};
let titleAnimTimeout = null;

function preloadFireImages() {
    if (fireImagesPreloaded) return;

    fireImages = [];
    let loadedCount = 0;
    const totalImages = 50;

    for (let i = 1; i <= totalImages; i++) {
        const img = new Image();
        const frameNumber = i.toString().padStart(5, '0');
        img.src = `/img/home/firestarter/fire_${frameNumber}.png`;

        img.onload = () => {
            loadedCount++;
            if (loadedCount === totalImages) {
                fireImagesPreloaded = true;
                console.log('Fire animation images preloaded successfully');
            }
        };

        fireImages.push(img);
    }
}

function PlayTopBarVideoAsync(section) {
    // Map section names to positions
    const positions = {
        'Home': 0,
        'Clients': 1,
        'Renewals': 2,
        'Leads': 2.5,
        'Contacts': 3,
        'Carriers': 3.5,
        'Policies': 4,
        'Profile': 5,
        'Settings': 5,
        'System': 5,
        'Other': 5  // Position for all other nav items
    };

    const position = positions[section];
    if (position === undefined) return;

    // Stop loading mode if active
    if (isLoadingMode) {
        stopLoadingMode();
    }

    // Stop logo loading if active
    if (isLogoLoading) {
        stopLogoLoading();
    }

    // Animate gradient to target position
    animateGradientToPosition(position);
    // Animate title layer for this section
    updateTitlePosition();
    animateTitleToSection(section);

    // Play fire animation sequence immediately
    playFireSequence();

    // Play logo animation sequence in sync
    playLogoSequenceWithTopBar();

    currentGradientPosition = position;
    currentTitleSection = section;
}

// New function specifically for other nav items that go to the fourth position
function PlayTopBarVideoAsyncOther() {
    PlayTopBarVideoAsync('Other');
}

function animateGradientToPosition(targetPosition) {
    const gradientLayer = document.getElementById('topbar-gradient-layer');
    if (!gradientLayer) return;

    const targetLeft = -(targetPosition * 1920); // Each position is 1920px wide

    gradientLayer.style.transition = 'left 0.8s cubic-bezier(0.7, 0, 0.2, 1)';
    gradientLayer.style.left = `${targetLeft}px`;
}

function playFireSequence() {
    const fireLayer = document.getElementById('topbar-fire-layer');
    if (!fireLayer) return;
    if (isFireSequencePlaying) return;

    isFireSequencePlaying = true;

    if (fireSequenceInterval) {
        clearInterval(fireSequenceInterval);
        fireSequenceInterval = null;
    }

    let currentFrame = 1; // Start from frame 1
    const totalFrames = 50; // Play 50 frames (1-50)

    const updateFrame = () => {
        const frameNumber = currentFrame.toString().padStart(5, '0');
        const imageUrl = `/img/home/firestarter/fire_${frameNumber}.png`;
        fireLayer.style.backgroundImage = `url('${imageUrl}')`;
        currentFrame++;

        if (currentFrame > totalFrames) {
            clearInterval(fireSequenceInterval);
            fireSequenceInterval = null;
            isFireSequencePlaying = false;
            // No fade out - PNG sequence ends as transparent
            // Clear the background image to ensure clean end state
            fireLayer.style.backgroundImage = '';
        }
    };

    fireSequenceInterval = setInterval(updateFrame, 33); // 30 FPS (33ms per frame)
    updateFrame(); // Render the first frame immediately to avoid the initial 33ms delay
}

function startLoadingMode() {
    if (isLoadingMode) return;

    isLoadingMode = true;
    const gradientLayer = document.getElementById('topbar-gradient-layer');
    if (!gradientLayer) return;

    // Create seamless infinite scroll effect
    gradientLayer.style.transition = 'none';
    gradientLayer.style.backgroundImage = `url('/img/home/topbar-grad.jpg'), url('/img/home/topbar-grad.jpg')`;
    gradientLayer.style.backgroundPosition = '0 0, 5760px 0';
    gradientLayer.style.backgroundSize = '5760px 50px, 5760px 50px';
    gradientLayer.style.backgroundRepeat = 'no-repeat, no-repeat';

    // Start infinite animation
    gradientLayer.style.animation = 'topbar-infinite-scroll 10s linear infinite';
    // Hide title while in loading mode
    const titleImg = document.getElementById('topbar-title-img');
    if (titleImg) {
        titleImg.style.transition = 'opacity 200ms ease';
        titleImg.style.opacity = '0';
    }
}

function stopLoadingMode() {
    if (!isLoadingMode) return;

    isLoadingMode = false;
    const gradientLayer = document.getElementById('topbar-gradient-layer');
    if (!gradientLayer) return;

    // Stop animation and reset to single background
    gradientLayer.style.animation = 'none';
    gradientLayer.style.backgroundImage = `url('/img/home/topbar-grad.jpg')`;
    gradientLayer.style.backgroundPosition = '0 0';
    gradientLayer.style.backgroundSize = '5760px 50px';
    gradientLayer.style.backgroundRepeat = 'no-repeat';

    // Return to current position
    animateGradientToPosition(currentGradientPosition);
    // Restore title
    const titleImg2 = document.getElementById('topbar-title-img');
    if (titleImg2) {
        animateTitleToSection(currentTitleSection || 'Home', true);
        titleImg2.style.opacity = '1';
    }
}

// Initialize the animation layers on page load
window.addEventListener('DOMContentLoaded', function () {
    initializeTopBarAnimations();
    // Start preloading fire images
    preloadFireImages();
    // Start preloading logo images
    preloadLogoImages();
    // Start preloading mic spinner images
    preloadMicSpinnerImages();
    // Preload title images
    preloadTitleImages();
});

function initializeTopBarAnimations() {
    const topBar = document.getElementById('sf-top-bar');
    if (!topBar) return;

    // Prevent duplicate initialization
    if (document.getElementById('topbar-gradient-layer') ||
        document.getElementById('topbar-fire-layer') ||
        document.getElementById('topbar-title-layer')) {
        return;
    }

    // Create gradient layer (bottom layer)
    const gradientLayer = document.createElement('div');
    gradientLayer.id = 'topbar-gradient-layer';
    gradientLayer.style.cssText = `
        position: absolute;
        top: 0;
        left: 0;
        width: 5760px;
        height: 50px;
        background-image: url('/img/home/topbar-grad.jpg');
        background-repeat: no-repeat;
        z-index: -2;
        pointer-events: none;
    `;

    // Create title layer (middle layer)
    const titleLayer = document.createElement('div');
    titleLayer.id = 'topbar-title-layer';
    titleLayer.style.cssText = `
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 50px;
        z-index: -1;
        pointer-events: none;
        overflow: hidden;
    `;
    const titleImg = document.createElement('img');
    titleImg.id = 'topbar-title-img';
    titleImg.alt = 'Topbar Title';
    titleImg.style.cssText = `
        position: absolute;
        top: 50%;
        left: 0;
        transform: translateY(-50%);
        height: 50px;
        opacity: 0;
        pointer-events: none;
        image-rendering: auto;
        filter: drop-shadow(0 1px 1px rgba(0,0,0,0.25));
        transition: transform 500ms ease-out, opacity 500ms ease-out;
    `;
    // Set initial src to avoid broken image flash
    titleImg.onerror = () => {
        if (!titleImg.dataset.fallback) {
            titleImg.dataset.fallback = '1';
            titleImg.src = '/img/topbar/home.png';
        }
    };
    titleImg.src = getTitleImagePath('Home') || '/img/topbar/home.png';
    titleLayer.appendChild(titleImg);

    // Create fire layer (top layer)
    const fireLayer = document.createElement('div');
    fireLayer.id = 'topbar-fire-layer';
    fireLayer.style.cssText = `
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 50px;
        overflow: hidden;
        background-size: 100% 50px;
        background-repeat: no-repeat;
        background-position: left center;
        z-index: -1;
        pointer-events: none;
    `;

    // Make top bar relative positioned to contain the absolute layers
    //topBar.style.position = 'relative';
    //topBar.style.overflow = 'hidden';

    // Insert layers at the beginning
    topBar.insertBefore(gradientLayer, topBar.firstChild);
    topBar.insertBefore(titleLayer, gradientLayer.nextSibling);
    topBar.insertBefore(fireLayer, titleLayer.nextSibling);

    // Position and initialize default title (Home) without animation
    updateTitlePosition();
    animateTitleToSection('Home', true);

    // Keep title position in sync on resize
    window.addEventListener('resize', updateTitlePosition);
}

// Clipboard: Copy HTML as rich text for Outlook/Email
window.copyHtmlToClipboard = function(html) {
    if (navigator.clipboard && window.ClipboardItem) {
        const type = "text/html";
        const blob = new Blob([html], { type });
        const data = [new window.ClipboardItem({ [type]: blob })];
        navigator.clipboard.write(data);
    } else {
        // fallback: copy as plain text
        navigator.clipboard.writeText(html);
    }
}

// Set checkbox indeterminate state
window.setCheckboxIndeterminate = function(element, isIndeterminate) {
    if (element) {
        element.indeterminate = isIndeterminate;
    }
}

// Push-to-talk functionality for main logo
let pushToTalkMediaRecorder;
let pushToTalkAudioChunks = [];
let pushToTalkAudioStream;
let micSpinnerImages = [];
let micSpinnerPreloaded = false;
let micSpinnerInterval = null;
let micSpinnerCurrentFrame = 0;

// Preload mic spinner images
function preloadMicSpinnerImages() {
    if (micSpinnerPreloaded) return;

    micSpinnerImages = [];
    let loadedCount = 0;
    const totalImages = 74; // 0-73

    for (let i = 0; i <= 73; i++) {
        const img = new Image();
        const frameNumber = i.toString().padStart(5, '0');
        img.src = `/img/home/spinq/qspin${frameNumber}.png`;

        img.onload = () => {
            loadedCount++;
            if (loadedCount === totalImages) {
                micSpinnerPreloaded = true;
                console.log('Mic spinner images preloaded successfully');
            }
        };

        img.onerror = () => {
            console.warn(`Failed to load mic spinner image: qspin${frameNumber}.png`);
            loadedCount++;
            if (loadedCount === totalImages) {
                micSpinnerPreloaded = true;
                console.log('Mic spinner images preloaded (some failed)');
            }
        };

        micSpinnerImages[i] = img;
    }
}

// Start mic spinner animation
function startMicSpinnerAnimation() {
    if (micSpinnerInterval) {
        clearInterval(micSpinnerInterval);
    }

    const logoImg = document.querySelector('.app-logo');
    if (!logoImg) return;

    // Start with frame 0
    micSpinnerCurrentFrame = 0;
    const frameNumber = micSpinnerCurrentFrame.toString().padStart(5, '0');
    logoImg.src = `/img/home/spinq/qspin${frameNumber}.png`;

    micSpinnerInterval = setInterval(() => {
        // Play frames 0–26 once, then loop 27–33
        const LOOP_START = 27;
        const LOOP_END = 33;
        const PRE_END = 26;

        // Advance frame
        micSpinnerCurrentFrame++;

        if (micSpinnerCurrentFrame <= PRE_END) {
            // Still in the initial segment 0–26, nothing special
        } else {
            // In the looping segment 27–33
            if (micSpinnerCurrentFrame > LOOP_END) {
                micSpinnerCurrentFrame = LOOP_START;
            }
        }

        const frameNumber = micSpinnerCurrentFrame.toString().padStart(5, '0');
        logoImg.src = `/img/home/spinq/qspin${frameNumber}.png`;
    }, 30); // ~30 FPS
}

// Stop mic spinner animation and play ending sequence
function stopMicSpinnerAnimation() {
    return new Promise((resolve) => {
        if (micSpinnerInterval) {
            clearInterval(micSpinnerInterval);
            micSpinnerInterval = null;
        }

        const logoImg = document.querySelector('.app-logo');
        if (!logoImg) {
            resolve();
            return;
        }

        // Continue from current frame to 73 to complete a full cycle
        let endingFrame = micSpinnerCurrentFrame; // Do not skip frames if we stopped early
        
        const endingInterval = setInterval(() => {
            const frameNumber = endingFrame.toString().padStart(5, '0');
            logoImg.src = `/img/home/spinq/qspin${frameNumber}.png`;
            
            endingFrame++;
            
            if (endingFrame > 73) { // 0..73 inclusive
                clearInterval(endingInterval);
                // Reset to original logo
                logoImg.src = '/img/home/spinq/qspin00000.png';
                resolve();
            }
        }, 30); // ~30 FPS
    });
}

window.startPushToTalkRecording = async function() {
    try {
        // Start the mic spinner animation
        startMicSpinnerAnimation();
        
        // Request microphone permission
        pushToTalkAudioStream = await navigator.mediaDevices.getUserMedia({ 
            audio: {
                echoCancellation: true,
                noiseSuppression: true,
                autoGainControl: true
            } 
        });
        
        // Reset audio chunks for new recording
        pushToTalkAudioChunks = [];
        
        // Create MediaRecorder with WebM format (widely supported)
        const options = { mimeType: 'audio/webm' };
        if (!MediaRecorder.isTypeSupported(options.mimeType)) {
            // Fallback to default format
            pushToTalkMediaRecorder = new MediaRecorder(pushToTalkAudioStream);
        } else {
            pushToTalkMediaRecorder = new MediaRecorder(pushToTalkAudioStream, options);
        }
        
        pushToTalkMediaRecorder.ondataavailable = function(event) {
            if (event.data.size > 0) {
                pushToTalkAudioChunks.push(event.data);
            }
        };
        
        pushToTalkMediaRecorder.start(1000); // Collect data every second
        return true;
    } catch (error) {
        console.error('Error starting push-to-talk recording:', error);
        // Stop animation on error
        await stopMicSpinnerAnimation();
        return false;
    }
};

window.stopPushToTalkRecording = function() {
    return new Promise(async (resolve, reject) => {
        // Stop the mic spinner animation and play ending sequence
        await stopMicSpinnerAnimation();
        
        if (pushToTalkMediaRecorder && pushToTalkMediaRecorder.state !== 'inactive') {
            pushToTalkMediaRecorder.onstop = async function() {
                try {
                    // Stop all audio tracks
                    if (pushToTalkAudioStream) {
                        pushToTalkAudioStream.getTracks().forEach(track => track.stop());
                    }
                    
                    // Create blob from recorded chunks
                    const audioBlob = new Blob(pushToTalkAudioChunks, { type: 'audio/webm' });
                    
                    // Convert blob to base64 string for safer transfer
                    const reader = new FileReader();
                    reader.onloadend = function() {
                        const base64String = reader.result.split(',')[1]; // Remove data:audio/webm;base64,
                        resolve(base64String);
                    };
                    reader.onerror = function() {
                        reject(new Error('Failed to convert audio to base64'));
                    };
                    reader.readAsDataURL(audioBlob);
                } catch (error) {
                    console.error('Error processing push-to-talk audio:', error);
                    reject(error);
                }
            };
            
            pushToTalkMediaRecorder.stop();
        } else {
            resolve('');
        }
    });
};

// Function to send transcription to Enhanced AI Chat
window.sendTranscriptionToChat = function(transcription) {
    // This will be called after navigation to /agents
    // We'll store the transcription and let the Enhanced AI Chat page pick it up
    sessionStorage.setItem('pendingVoiceTranscription', transcription);
    
    // Trigger a custom event that the Enhanced AI Chat page can listen for
    window.dispatchEvent(new CustomEvent('voiceTranscriptionReady', { 
        detail: { transcription: transcription } 
    }));
};

// Function to show transcription errors
window.showTranscriptionError = function(errorMessage) {
    // Store error message for Enhanced AI Chat page to display
    sessionStorage.setItem('voiceTranscriptionError', errorMessage);
    
    // Trigger error event
    window.dispatchEvent(new CustomEvent('voiceTranscriptionError', { 
        detail: { error: errorMessage } 
    }));
};

// Function to clear pending voice transcriptions
window.clearPendingVoiceTranscriptions = function() {
    // Clear any pending transcriptions and errors
    sessionStorage.removeItem('pendingVoiceTranscription');
    sessionStorage.removeItem('voiceTranscriptionError');
    
    console.log('[PushToTalk] Cleared pending voice transcriptions');
};

// Global audio instance for staff notifications
let staffNotificationAudio = null;
let audioContextUnlocked = false;

// Initialize audio context after user interaction
window.initializeAudioContext = function() {
    if (audioContextUnlocked) return;
    
    try {
        // Create and preload the audio
        staffNotificationAudio = new Audio('/img/audio-alert.mp3');
        staffNotificationAudio.volume = 0.5;
        staffNotificationAudio.preload = 'auto';
        
        // Play silently to unlock audio context
        staffNotificationAudio.play().then(() => {
            staffNotificationAudio.pause();
            staffNotificationAudio.currentTime = 0;
            audioContextUnlocked = true;
            console.log('Audio context unlocked for notifications');
        }).catch(error => {
            console.warn('Failed to unlock audio context:', error);
        });
    } catch (error) {
        console.warn('Error initializing audio context:', error);
    }
};

// Add event listeners to unlock audio on first user interaction
document.addEventListener('click', window.initializeAudioContext, { once: true });
document.addEventListener('keydown', window.initializeAudioContext, { once: true });
document.addEventListener('touchstart', window.initializeAudioContext, { once: true });

// Function to play staff message notification sound
window.playStaffMessageSound = function(enableAudio = true) {
    try {
        // Check if audio is enabled in user preferences
        if (!enableAudio) {
            console.log('Staff message sound disabled by user preference');
            return;
        }
        
        // Use pre-initialized audio if available, otherwise create new
        const audio = staffNotificationAudio || new Audio('/img/StaffAlert.mp3');
        
        if (!staffNotificationAudio) {
            audio.volume = 0.5;
        } else {
            // Reset audio to beginning
            audio.currentTime = 0;
        }
        
        const playPromise = audio.play();
        
        if (playPromise !== undefined) {
            playPromise.then(() => {
                console.log('Staff message sound played successfully (audio context unlocked: ' + audioContextUnlocked + ')');
            }).catch(error => {
                if (error.name === 'NotAllowedError') {
                    console.warn('Staff message sound blocked by browser - user interaction required first');
                } else {
                    console.warn('Failed to play staff message sound:', error);
                }
            });
        }
    } catch (error) {
        console.warn('Error playing staff message sound:', error);
    }
};
// Function to play staff message reminder sound (every 5 minutes for unread messages)
window.playStaffMessageReminder = function (enableAudio = true) {
    try {
        // Check if audio is enabled in user preferences
        if (!enableAudio) {
            console.log('Staff message reminder disabled by user preference');
            return;
        }
        
        const audio = document.getElementById('staffMessageReminderAudio');
        if (audio) {
            // Reset audio to beginning
            audio.currentTime = 0;

            // Play the audio
            const playPromise = audio.play();

            if (playPromise !== undefined) {
                playPromise.then(() => {
                    console.log('Staff message reminder played successfully');
                }).catch(error => {
                    if (error.name === 'NotAllowedError') {
                        console.warn('Staff message reminder blocked by browser - user interaction required first');
                    } else {
                        console.warn('Failed to play staff message reminder:', error);
                    }
                });
            }
        } else {
            console.warn('Staff message reminder audio element not found');
        }
    } catch (error) {
        console.error('Error playing staff message reminder:', error);
    }
};

// Function to add colors to renewal status dropdown options
function AddRenewalStatusColors(datasource, id) {
    console.log("AddRenewalStatusColors JS function called");
    console.log("Datasource:", datasource);
    console.log("ID:", id);
    
    setTimeout(() => {
        console.log("Timeout executed, looking for popup");
        // Get the popup element
        var popup = document.getElementById(id + "_popup");
        console.log("Popup element:", popup);
        if (!popup) {
            console.log("Popup not found!");
            return;
        }

        var listItems = popup.querySelectorAll('li');
        console.log("List items found:", listItems.length);
        
        for (var i = 0; i < listItems.length && i < datasource.length; i++) {
            // Try different ways to access the status value
            var statusValue = datasource[i].value || datasource[i].Value || datasource[i].text || datasource[i].Text;
            var listItem = listItems[i];
            
            console.log(`Processing item ${i}:`);
            console.log("Full datasource item:", datasource[i]);
            console.log(`statusValue = "${statusValue}"`);
            console.log("List item:", listItem);
            
            // Remove any existing status classes
            listItem.classList.remove('status-pending', 'status-renewed', 'status-unneeded', 'status-defected', 'status-ghosted', 'status-unplaceable', 'status-other', 'status-Purple', 'status-Orange', 'status-Yellow', 'status-Red', 'status-Green');
            
            // Add appropriate class based on status
            switch (statusValue) {
                case 'Renewed':
                    listItem.classList.add('status-renewed');
                    break;
                case 'Purple':
                case 'Orange':
                case 'Yellow':
                case 'Red':
                case 'Green':
                    listItem.classList.add("status-" + statusValue);
                    break;
                case 'Unneeded':
                    listItem.classList.add('status-unneeded');
                    break;
                case 'Defected':
                case 'Ghosted':
                case 'Unplaceable':
                    listItem.classList.add('status-defected');
                    break;
                case 'Pending':
                case 'Other':
                default:
                    console.log("No special styling for:", statusValue);
                    // No special styling for Pending and Other
                    break;
            }
            console.log("Final classes:", listItem.className);
        }

        // Add CSS styles if not already present
        if (!document.getElementById('renewal-status-styles')) {
            var style = document.createElement('style');
            style.id = 'renewal-status-styles';
            style.textContent = `
                .renewal-status-dropdown li.e-list-item.status-renewed {
                    color: #2e7d32 !important; /* Dark green */
                    font-weight: 500;
                }
                .renewal-status-dropdown li.e-list-item.status-unneeded {
                    color: #f57c00 !important; /* Dark orange */
                    font-weight: 500;
                }
                .renewal-status-dropdown li.e-list-item.status-defected {
                    color: #c62828 !important; /* Dark red */
                    font-weight: 500;
                }
                .renewal-status-dropdown li.e-list-item.status-Purple {
                    color: #8e24aa !important; /* Purple */
                    font-weight: 500;
                }
                .renewal-status-dropdown li.e-list-item.status-Orange {
                    color: #fb8c00 !important; /* Orange */
                    font-weight: 500;
                }
                .renewal-status-dropdown li.e-list-item.status-Yellow {
                    color: #fdd835 !important; /* Yellow */
                    font-weight: 500;
                }
                .renewal-status-dropdown li.e-list-item.status-Red {
                    color: #e53935 !important; /* Red */
                    font-weight: 500;
                }
                .renewal-status-dropdown li.e-list-item.status-Green {
                    color: #43a047 !important; /* Green */
                    font-weight: 500;
                }
            `;
            document.head.appendChild(style);
        }
    }, 100);
}
