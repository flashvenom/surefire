export function initializeDragScroll(element) {
    if (!element) return;

    let isDown = false;
    let startX;
    let scrollLeft;

    element.addEventListener('mousedown', (e) => {
        isDown = true;
        element.classList.add('grabbing');
        startX = e.pageX - element.offsetLeft;
        scrollLeft = element.scrollLeft;
        element.style.cursor = 'grabbing'; // Change cursor immediately
    });

    element.addEventListener('mouseleave', () => {
        if (!isDown) return;
        isDown = false;
        element.classList.remove('grabbing');
        element.style.cursor = 'grab'; // Reset cursor
    });

    element.addEventListener('mouseup', () => {
        if (!isDown) return;
        isDown = false;
        element.classList.remove('grabbing');
        element.style.cursor = 'grab'; // Reset cursor
    });

    element.addEventListener('mousemove', (e) => {
        if (!isDown) return;
        e.preventDefault(); // Prevent default drag behavior (like selecting text/images)
        const x = e.pageX - element.offsetLeft;
        const walk = (x - startX) * 2; // The multiplier adjusts scroll speed
        element.scrollLeft = scrollLeft - walk;
    });
} 