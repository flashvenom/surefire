/**
 * Company Manual Lightbox - Video & Image Theater Mode
 * Provides fullscreen viewing for embedded training videos and images
 */
window.CompanyManualLightbox = (function () {
    let dotNetRef = null;
    let isInitialized = false;

    function handleKeydown(e) {
        if (e.key === 'Escape' && dotNetRef) {
            dotNetRef.invokeMethodAsync('HandleEscapeKey');
        }
    }

    function getVideoSource(video) {
        // Get the source - could be from src attribute or first source element
        let src = video.src;
        if (!src || src === window.location.href) {
            const sourceEl = video.querySelector('source');
            if (sourceEl) {
                src = sourceEl.src;
            }
        }
        return src;
    }

    function handleVideoClick(video) {
        if (!dotNetRef) return;
        
        const src = getVideoSource(video);
        
        if (src) {
            // Pause the inline video
            video.pause();
            dotNetRef.invokeMethodAsync('OpenVideoLightbox', src);
        }
    }

    function handleImageClick(e) {
        const img = e.currentTarget;
        if (!dotNetRef) return;
        
        const src = img.src;
        const alt = img.alt || '';
        
        if (src) {
            dotNetRef.invokeMethodAsync('OpenImageLightbox', src, alt);
        }
    }

    function hideFallbackText(video) {
        // Hide any <p> fallback text inside the video element
        const fallbacks = video.querySelectorAll('p, a');
        fallbacks.forEach(function(el) {
            el.style.display = 'none';
        });
    }

    function wrapVideoWithContainer(video) {
        // Check if already wrapped
        if (video.parentElement && video.parentElement.classList.contains('manual-video-container')) {
            // Just ensure expand button exists and click handler
            ensureExpandButton(video.parentElement, video);
            ensureContainerClickHandler(video.parentElement, video);
            return;
        }
        
        if (video.parentElement && video.parentElement.classList.contains('manual-video-wrapper')) {
            video.parentElement.classList.add('manual-video-container');
            ensureExpandButton(video.parentElement, video);
            ensureContainerClickHandler(video.parentElement, video);
            return;
        }

        // Create wrapper
        const wrapper = document.createElement('div');
        wrapper.className = 'manual-video-container';
        
        // Wrap video
        video.parentNode.insertBefore(wrapper, video);
        wrapper.appendChild(video);
        
        // Add expand button
        ensureExpandButton(wrapper, video);
        // Make container clickable
        ensureContainerClickHandler(wrapper, video);
    }

    function ensureContainerClickHandler(container, video) {
        // Skip if already has click handler
        if (container.dataset.clickHandlerAttached) return;
        container.dataset.clickHandlerAttached = 'true';
        
        // Add click handler to container
        container.addEventListener('click', function (e) {
            // Don't expand if clicking on the expand button
            const target = e.target;
            if (target.classList.contains('manual-video-expand') || 
                target.closest('.manual-video-expand')) {
                return;
            }
            
            // If clicking on the video element, check if it's in the controls area
            if (target === video || target.closest('video') === video) {
                const rect = video.getBoundingClientRect();
                const clickY = e.clientY - rect.top;
                
                // Video controls are at the bottom. If click is in bottom 15% of video, 
                // it's likely the controls - let the video handle it normally
                const isControlsArea = clickY > rect.height * 0.85;
                
                if (!isControlsArea) {
                    // Click is on video but not controls - expand
                    e.preventDefault();
                    e.stopPropagation();
                    handleVideoClick(video);
                }
                // Otherwise, let the video controls handle the click
            } else {
                // Clicking on container background - expand
                e.preventDefault();
                e.stopPropagation();
                handleVideoClick(video);
            }
        });
    }

    function ensureExpandButton(container, video) {
        if (container.querySelector('.manual-video-expand')) return;
        
        const expandBtn = document.createElement('button');
        expandBtn.className = 'manual-video-expand';
        expandBtn.setAttribute('type', 'button');
        expandBtn.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2"><path stroke-linecap="round" stroke-linejoin="round" d="M4 8V4m0 0h4M4 4l5 5m11-1V4m0 0h-4m4 0l-5 5M4 16v4m0 0h4m-4 0l5-5m11 5v-4m0 4h-4m4 0l-5-5" /></svg><span>Expand</span>';
        
        expandBtn.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            handleVideoClick(video);
        });
        
        container.appendChild(expandBtn);
    }

    return {
        init: function (ref) {
            dotNetRef = ref;
            
            if (!isInitialized) {
                document.addEventListener('keydown', handleKeydown);
                isInitialized = true;
            }
            
            // Attach handlers to existing media
            this.attachMediaHandlers(ref);
        },

        attachMediaHandlers: function (ref) {
            if (ref) {
                dotNetRef = ref;
            }
            
            // Use a small delay to ensure DOM is updated
            setTimeout(function () {
                // Find all videos in the manual view body
                const viewBody = document.querySelector('.manual-view__body');
                if (!viewBody) return;

                // Handle videos
                const videos = viewBody.querySelectorAll('video');
                videos.forEach(function (video) {
                    // Hide fallback text
                    hideFallbackText(video);
                    
                    // Skip if already has handler
                    if (video.dataset.lightboxAttached) return;
                    
                    video.dataset.lightboxAttached = 'true';
                    video.style.cursor = 'pointer';
                    video.title = 'Click to expand';
                    
                    // Wrap with container for expand button and click handling
                    wrapVideoWithContainer(video);
                });

                // Handle images
                const images = viewBody.querySelectorAll('img');
                images.forEach(function (img) {
                    // Skip if already has handler
                    if (img.dataset.lightboxAttached) return;
                    
                    img.dataset.lightboxAttached = 'true';
                    img.style.cursor = 'zoom-in';
                    img.title = 'Click to view full size';
                    
                    img.addEventListener('click', handleImageClick);
                });
            }, 150);
        },

        dispose: function () {
            document.removeEventListener('keydown', handleKeydown);
            dotNetRef = null;
            isInitialized = false;
        }
    };
})();

