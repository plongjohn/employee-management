# 0004 – Search, sorting and paging in the database

- **Status:** Accepted
- **Date:** 2026-10-05

## Context

The employee list needs a search field, a department filter (including "all departments"),
sortable columns and a count of matching employees. The sample data has 20 employees, but
the real number is not known in advance, so the list must stay fast without loading every
row.

## Decision

Search, filter, sorting and paging run in the database; the client only receives one page.

- **Prefix search:** the search text is split at whitespace into at most five terms. Every
  term must match the start of the first name, last name **or** email
  (`LIKE 'term%'`), so "anna mü" finds Anna Müller. A prefix pattern can use the column
  indexes; a contains search (`'%term%'`) would always scan the whole table. Wildcards typed
  by the user (`%`, `_`, `\`) are escaped.
- **Department filter:** the condition is only added when a department is selected
  (see [ADR 0002](0002-departments-table.md)).
- **Sorting:** by name, email, department or hire date, ascending or descending. The
  `ORDER BY` clause comes from a fixed mapping of an enum – user input never reaches the SQL
  text. Sorts on non-unique columns end with `e.id`, so rows with equal values keep a stable
  order across pages; the unique email needs no tie-breaker.
- **Offset paging:** `LIMIT @PageSize OFFSET @Offset`, page size 1–100 (UI default 25). A
  second `COUNT(*)` with the same conditions returns the total; both statements run in one
  round trip.
- Indexes support each search column and every sort order except the department name, which
  comes from the joined table (see `database/README.md`). Measured with 20,000 employees: the
  first page takes well under a millisecond for name, email and hire date; sorting by
  department or jumping to the last page takes about 70 ms.

### Alternatives considered

- **Filtering in memory:** simple, but every list refresh would load the whole table.
- **Keyset paging** (`WHERE (last_name, id) > (@last, @id)`): constant speed on deep pages,
  but complex with switchable sort columns and no direct jump to page *n*. Offset paging is
  fast enough for tens of thousands of rows.
- **FULLTEXT index:** word-based and meant for longer text; not a good fit for names.

## Consequences

- Searching for a part in the middle of a name (e.g. "ller" for "Müller") finds nothing.
- Very deep pages get slower with offset paging; acceptable at this scale.
- The web application has to implement the same rules to behave identically.
