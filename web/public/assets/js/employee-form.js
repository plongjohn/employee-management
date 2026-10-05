'use strict';

// Save is only possible when something was changed and every required field is filled –
// the same rule as in the desktop app. The server validates every submission regardless.

const form = document.getElementById('employee-form');
const saveButton = form.querySelector('[data-save]');
const saveHint = form.querySelector('[data-save-hint]');
const requiredFields = [...form.querySelectorAll('[data-required]')];
const original = readOriginalValues();
const saveTooltip = new bootstrap.Tooltip(saveHint, { title: () => saveHint.dataset.hint ?? '' });

let isLeavingOnPurpose = false;

function readOriginalValues() {
    try {
        return JSON.parse(form.querySelector('[data-original]').value);
    } catch {
        // Without the original values every input counts as a change.
        return {};
    }
}

function currentValue(field) {
    return field.value.trim();
}

function hasChanges() {
    return requiredFields.some((field) => currentValue(field) !== (original[field.name] ?? '').trim());
}

function allRequiredFilled() {
    return requiredFields.every((field) => currentValue(field) !== '');
}

function updateSaveButton() {
    const missingRequired = !allRequiredFilled();
    const unchanged = !hasChanges();

    saveButton.disabled = missingRequired || unchanged;
    if (missingRequired) {
        saveHint.dataset.hint = saveHint.dataset.hintRequired;
    } else if (unchanged) {
        saveHint.dataset.hint = saveHint.dataset.hintNoChanges;
    } else {
        delete saveHint.dataset.hint;
        saveTooltip.hide();
    }

    if (saveButton.disabled) {
        saveTooltip.enable();
    } else {
        saveTooltip.disable();
    }
}

form.addEventListener('input', updateSaveButton);
form.addEventListener('change', updateSaveButton);

form.addEventListener('submit', () => {
    isLeavingOnPurpose = true;
    // Prevents a second submission by a double click.
    saveButton.disabled = true;
});

// Leaving with unsaved changes asks first: our dialog for "Cancel", the browser's for everything else.
const discardDialogElement = document.getElementById('discard-dialog');
const discardDialog = new bootstrap.Modal(discardDialogElement);
const cancelLink = form.querySelector('[data-cancel]');

cancelLink.addEventListener('click', (event) => {
    if (hasChanges()) {
        event.preventDefault();
        discardDialog.show();
    }
});

discardDialogElement.querySelector('[data-discard-confirm]').addEventListener('click', () => {
    isLeavingOnPurpose = true;
    window.location.href = cancelLink.href;
});

window.addEventListener('beforeunload', (event) => {
    if (!isLeavingOnPurpose && hasChanges()) {
        event.preventDefault();
    }
});

updateSaveButton();
