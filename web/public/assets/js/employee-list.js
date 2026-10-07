'use strict';

// Same delay as the desktop app: searches once the user pauses typing.
const SEARCH_DELAY_MS = 300;

// Same interval as the desktop app: short enough that changes from the desktop app show up
// while both are open side by side, and reloading one page is a cheap indexed query.
const REFRESH_INTERVAL_MS = 30_000;

// Fast answers show no loading state at all, so the list does not flicker on every keystroke.
const LOADING_INDICATOR_DELAY_MS = 300;

const filterForm = document.getElementById('employee-filter');
const searchInput = document.getElementById('search');
const addEmployeeLink = document.querySelector('[data-add-employee-link]');
const resultAnnouncement = document.querySelector('[data-result-announcement]');

let searchTimer = null;
let runningRequest = null;

// Without JavaScript the filter form still works with Enter and full page loads.
filterForm.addEventListener('submit', (event) => {
    event.preventDefault();
    cancelPendingSearch();
    showList(filterFragmentUrl());
});

searchInput.addEventListener('input', () => {
    cancelPendingSearch();
    searchTimer = setTimeout(() => {
        searchTimer = null;
        showList(filterFragmentUrl());
    }, SEARCH_DELAY_MS);
});

// Listeners sit on the document because the page size select and the delete dialog are
// replaced together with the list.
document.addEventListener('change', (event) => {
    if (event.target.matches('[data-auto-submit]')) {
        filterForm.requestSubmit();
    }
});

// One dialog serves all rows: the delete button passes URL, version and name of its employee.
document.addEventListener('show.bs.modal', (event) => {
    if (event.target.id !== 'delete-dialog') {
        return;
    }

    const button = event.relatedTarget;
    const form = event.target.querySelector('[data-delete-form]');
    const text = event.target.querySelector('[data-delete-text]');

    form.action = button.dataset.deleteUrl;
    form.querySelector('[data-version-field]').value = button.dataset.version;
    // textContent, not innerHTML: the name is user input.
    // A replacer function keeps patterns like "$&" in the name from being interpreted.
    text.textContent = text.dataset.template.replace('{name}', () => button.dataset.employeeName);
});

setInterval(refreshInBackground, REFRESH_INTERVAL_MS);

// Switching back from the desktop app shows its changes right away, without waiting for the timer.
document.addEventListener('visibilitychange', () => {
    if (!document.hidden) {
        refreshInBackground();
    }
});
window.addEventListener('focus', refreshInBackground);

function cancelPendingSearch() {
    clearTimeout(searchTimer);
    searchTimer = null;
}

// Skipped while the user is busy, so it never interrupts typing, an open dialog or a running search.
function refreshInBackground() {
    const isUserBusy = document.hidden || searchTimer !== null || runningRequest !== null
        || document.querySelector('.modal.show') !== null;
    if (!isUserBusy) {
        showList(fragmentUrl(currentList().dataset.listUrl), { isBackgroundRefresh: true });
    }
}

async function showList(url, { isBackgroundRefresh = false } = {}) {
    // A newer search replaces the running one, so a slow old result can never overwrite a newer one.
    runningRequest?.abort();
    const request = new AbortController();
    runningRequest = request;
    const loadingTimer = isBackgroundRefresh
        ? null
        : setTimeout(() => showLoading(true), LOADING_INDICATOR_DELAY_MS);
    let isLeavingPage = false;

    try {
        const response = await fetch(url, { signal: request.signal });
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        const newList = parseList(await response.text());
        if (!newList.isEqualNode(currentList())) {
            replaceList(newList);
        }

        // Only searches the user started are announced; a background refresh stays silent.
        if (!isBackgroundRefresh) {
            resultAnnouncement.textContent = newList.querySelector('[data-employee-count]').textContent.trim();
        }
    } catch (error) {
        if (request.signal.aborted) {
            return;
        }

        if (isBackgroundRefresh) {
            console.warn('Background refresh of the employee list failed', error);
            return;
        }

        // A full page load shows the usual error page, e.g. when the database is unreachable.
        // It can take as long as the failed search, so the loading state stays until then.
        isLeavingPage = true;
        showLoading(true);
        window.location.assign(listPageUrl(url));
    } finally {
        clearTimeout(loadingTimer);
        // A replaced request leaves the loading state to the newer one.
        if (runningRequest === request) {
            runningRequest = null;
            showLoading(isLeavingPage);
        }
    }
}

function showLoading(isLoading) {
    filterForm.classList.toggle('is-loading', isLoading);
    // Removed rather than set to "false", so the list still equals a freshly loaded one.
    if (isLoading) {
        currentList().setAttribute('aria-busy', 'true');
    } else {
        currentList().removeAttribute('aria-busy');
    }
}

function replaceList(newList) {
    const oldList = currentList();
    const focusedIdInList = oldList.contains(document.activeElement) ? document.activeElement.id : '';

    oldList.replaceWith(newList);
    if (focusedIdInList !== '') {
        document.getElementById(focusedIdInList)?.focus();
    }

    // The address bar and the add link keep the list state, as after a full page load.
    history.replaceState(null, '', newList.dataset.listUrl);
    addEmployeeLink.search = new URL(newList.dataset.listUrl, window.location.origin).search;
}

function currentList() {
    return document.querySelector('[data-employee-list]');
}

function parseList(html) {
    const template = document.createElement('template');
    template.innerHTML = html.trim();

    return template.content.querySelector('[data-employee-list]');
}

function filterFragmentUrl() {
    const parameters = new URLSearchParams();
    for (const [name, value] of new FormData(filterForm)) {
        if (value !== '') {
            parameters.append(name, value);
        }
    }

    return fragmentUrl('/employees?' + parameters);
}

function fragmentUrl(listUrl) {
    const url = new URL(listUrl, window.location.origin);
    url.pathname = '/employees/list';

    return url.toString();
}

function listPageUrl(listFragmentUrl) {
    const url = new URL(listFragmentUrl);
    url.pathname = '/employees';

    return url.toString();
}
