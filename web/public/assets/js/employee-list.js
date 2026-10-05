'use strict';

// Changing the department filter or the page size reloads the list right away.
document.querySelectorAll('[data-auto-submit]').forEach((select) => {
    select.addEventListener('change', () => select.form.requestSubmit());
});

// One dialog serves all rows: the delete button passes URL, version and name of its employee.
const deleteDialog = document.getElementById('delete-dialog');

deleteDialog.addEventListener('show.bs.modal', (event) => {
    const button = event.relatedTarget;
    const form = deleteDialog.querySelector('[data-delete-form]');
    const text = deleteDialog.querySelector('[data-delete-text]');

    form.action = button.dataset.deleteUrl;
    form.querySelector('[data-version-field]').value = button.dataset.version;
    // textContent, not innerHTML: the name is user input.
    // A replacer function keeps patterns like "$&" in the name from being interpreted.
    text.textContent = text.dataset.template.replace('{name}', () => button.dataset.employeeName);
});
