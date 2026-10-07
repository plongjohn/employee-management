# 0009 – Live search and keeping the list up to date

- **Status:** Accepted
- **Date:** 2026-10-07

## Context

Both applications work on the same data. A change made in the desktop app only appeared in
the web app after a manual reload, and vice versa. The web search also needed `Enter`, while
the desktop app already searched while typing. Both no longer match what users expect from a
current application.

## Decision

- **Search while typing:** both apps search 300 ms after the last keystroke. The web app
  loads only the table from the new fragment endpoint `GET /employees/list` and swaps it in;
  the address bar and the "add" link keep the list state, so bookmarks and the return after
  saving still work. Without JavaScript the form works with `Enter` and full page loads.
- **Polling instead of push:** both apps reload the page that is currently shown every
  30 seconds and as soon as the window or browser tab becomes active again. The reload is
  the same indexed query as a normal page change
  ([ADR 0004](0004-search-sorting-paging.md)); no new SQL and no extra service.
- **No disturbance:** the reload is skipped while the user types, a form or dialog is open,
  or a load is already running. The list is only redrawn when something changed (desktop:
  ids and versions of the page, web: the rendered fragment); selection and scroll position
  are kept.
- **Errors:** a failed background reload is logged but shows no dialog; it would otherwise
  repeat every 30 seconds while the database is down. The next manual action reports the
  error as usual. In the web app, a slow search dims the list and shows a spinner after
  300 ms.

### Alternatives considered

- **Push (WebSockets, SignalR, server-sent events):** changes would appear instantly, but
  MariaDB cannot notify clients, so both apps would need an additional server that every
  write goes through. Too much infrastructure for this scope.
- **Change check first** (`SELECT MAX(updated_at), COUNT(*)` before reloading): saves
  little, because reloading one page is already cheap, and needs its own rules for what
  counts as a change – a delete, for example, leaves `MAX(updated_at)` untouched.
- **Short interval (a few seconds):** feels instant, but multiplies the queries per open
  window without real benefit; activating the window already covers the common case of
  switching between the apps.

## Consequences

- A change by someone else shows up after at most 30 seconds, or immediately when switching
  to the window.
- Every open window sends one small query every 30 seconds.
- The web list exists twice as an entry point (page and fragment), both rendered from the
  same template `employees/_list.html.twig`.
