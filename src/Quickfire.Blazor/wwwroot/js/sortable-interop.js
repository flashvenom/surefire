let sortableInstance = null;
let dotNetRef = null;

export function initializeSortable(elementId, dotNetReference) {
    dotNetRef = dotNetReference;
    const element = document.getElementById(elementId);
    
    if (!element) {
        console.error('Sortable element not found:', elementId);
        return;
    }

    // Destroy existing instance if it exists
    if (sortableInstance) {
        sortableInstance.destroy();
    }

    sortableInstance = new Sortable(element, {
        animation: 150,
        handle: '.sortable-item-handle',
        draggable: '.sortable-item',
        ghostClass: 'sortable-ghost',
        chosenClass: 'sortable-chosen',
        dragClass: 'sortable-drag',
        
        onEnd: function(evt) {
            const item = evt.item;
            const taskId = item.getAttribute('data-task-id');
            const taskType = item.getAttribute('data-task-type');
            const oldIndex = evt.oldIndex;
            const newIndex = evt.newIndex;
            
            // Don't process if item didn't move
            if (oldIndex === newIndex) {
                return;
            }
            
            const allItems = Array.from(element.children);
            let parentId = null;
            let adjustedIndex = 0;
            
            if (taskType === 'subtask') {
                // Find which parent this subtask is now under
                let currentParentId = null;
                let subtaskCount = 0;
                
                // Scan backwards from newIndex to find the parent
                for (let i = newIndex; i >= 0; i--) {
                    const itemType = allItems[i].getAttribute('data-task-type');
                    if (itemType === 'parent') {
                        currentParentId = allItems[i].getAttribute('data-task-id');
                        break;
                    }
                }
                
                // Count subtasks before this one under the same parent
                if (currentParentId) {
                    for (let i = 0; i < newIndex; i++) {
                        const itemType = allItems[i].getAttribute('data-task-type');
                        const itemParentId = allItems[i].getAttribute('data-parent-id');
                        
                        if (itemType === 'subtask' && itemParentId === currentParentId) {
                            subtaskCount++;
                        }
                    }
                }
                
                parentId = currentParentId;
                adjustedIndex = subtaskCount;
            } else if (taskType === 'parent') {
                // Count parent tasks before this one
                let parentCount = 0;
                for (let i = 0; i < newIndex; i++) {
                    if (allItems[i].getAttribute('data-task-type') === 'parent') {
                        parentCount++;
                    }
                }
                adjustedIndex = parentCount;
            }
            
            console.log('Drag ended:', { taskId, taskType, oldIndex, newIndex, adjustedIndex, parentId });
            
            // Call back to .NET
            dotNetRef.invokeMethodAsync('OnSortEnd', taskId, taskType, adjustedIndex, parentId, oldIndex, newIndex)
                .catch(err => console.error('Error calling OnSortEnd:', err));
        },
        
        // Determine if an item can be moved to a specific position
        onMove: function(evt) {
            const draggedType = evt.dragged.getAttribute('data-task-type');
            const relatedType = evt.related.getAttribute('data-task-type');
            
            // Allow all moves - we'll handle the logic in C#
            return true;
        }
    });
}

export function refreshSortable(elementId) {
    // Reinitialize sortable when the list changes
    if (sortableInstance && dotNetRef) {
        initializeSortable(elementId, dotNetRef);
    }
}

export function destroySortable() {
    if (sortableInstance) {
        sortableInstance.destroy();
        sortableInstance = null;
    }
    dotNetRef = null;
}

