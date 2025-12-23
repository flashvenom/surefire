const wheelListeners = new WeakMap();
const scrollStates = new WeakMap();
const SCROLL_SENSITIVITY = 0.3;

export function initializePipelineScroll(element) {
    if (!element || wheelListeners.has(element)) {
        return;
    }

    element.style.scrollBehavior = "smooth";
    const state = { snapTimeout: null };

    const onWheel = (event) => {
        if (typeof event.deltaY !== "number") {
            return;
        }

        if (Math.abs(event.deltaY) <= Math.abs(event.deltaX ?? 0)) {
            return;
        }

        event.preventDefault();

        const maxScroll = Math.max(0, element.scrollWidth - element.clientWidth);
        let delta = event.deltaY * SCROLL_SENSITIVITY;
        const predicted = element.scrollLeft + delta;

        if (predicted < 0 || predicted > maxScroll) {
            const overshoot = predicted < 0 ? Math.abs(predicted) : Math.abs(predicted - maxScroll);
            const resistance = 1 + overshoot / Math.max(element.clientWidth, 1);
            delta = delta / resistance;
        }

        element.scrollBy({
            left: delta,
            behavior: "smooth"
        });

        if (state.snapTimeout) {
            clearTimeout(state.snapTimeout);
        }

        state.snapTimeout = setTimeout(() => {
            const current = element.scrollLeft;
            if (current < 1) {
                element.scrollTo({ left: 0, behavior: "smooth" });
            } else if (current > maxScroll - 1) {
                element.scrollTo({ left: maxScroll, behavior: "smooth" });
            }
        }, 180);
    };

    element.addEventListener("wheel", onWheel, { passive: false });
    wheelListeners.set(element, onWheel);
    scrollStates.set(element, state);
}

export function disposePipelineScroll(element) {
    if (!element) {
        return;
    }

    const handler = wheelListeners.get(element);
    if (handler) {
        element.removeEventListener("wheel", handler);
        wheelListeners.delete(element);
    }

    const state = scrollStates.get(element);
    if (state?.snapTimeout) {
        clearTimeout(state.snapTimeout);
    }

    scrollStates.delete(element);
}
