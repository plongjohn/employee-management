<?php

declare(strict_types=1);

namespace EmployeeManagement\Models;

use InvalidArgumentException;

/**
 * Search, filter, sort order and page of the employee list. It travels in the query string,
 * so a list view can be bookmarked and restored after editing or deleting an employee.
 */
final readonly class EmployeeQuery
{
    public const int DEFAULT_PAGE_SIZE = 25;
    public const int MAX_PAGE_SIZE = 100;
    public const array PAGE_SIZE_OPTIONS = [10, 25, 50, 100];

    // Every term adds one OR group to the WHERE clause; the cap keeps the statement small
    // no matter what is typed. Five words are more than any name search needs.
    public const int MAX_SEARCH_TERMS = 5;

    // Far beyond any real list, but low enough that page × page size always fits into an int:
    // a page number like 9223372036854775807 from a hand-edited URL would turn the offset into a float.
    public const int MAX_PAGE = 1_000_000;

    public function __construct(
        public string $searchText = '',
        public ?int $departmentId = null,
        public EmployeeSortColumn $sortBy = EmployeeSortColumn::Name,
        public SortDirection $direction = SortDirection::Ascending,
        public int $page = 1,
        public int $pageSize = self::DEFAULT_PAGE_SIZE,
    ) {
        if ($page < 1 || $page > self::MAX_PAGE) {
            throw new InvalidArgumentException('The page must be between 1 and ' . self::MAX_PAGE . '.');
        }

        if ($pageSize < 1 || $pageSize > self::MAX_PAGE_SIZE) {
            throw new InvalidArgumentException('The page size must be between 1 and ' . self::MAX_PAGE_SIZE . '.');
        }
    }

    /**
     * Reads the list state from request parameters. Values that are missing, malformed or out
     * of range fall back to the defaults: a hand-edited URL shows a valid list, not an error.
     *
     * @param array<string, mixed> $parameters
     */
    public static function fromParameters(array $parameters): self
    {
        $pageSize = self::positiveInt($parameters['size'] ?? null);

        return new self(
            searchText: mb_trim(self::stringValue($parameters['q'] ?? null)),
            departmentId: self::positiveInt($parameters['department'] ?? null),
            sortBy: EmployeeSortColumn::tryFrom(self::stringValue($parameters['sort'] ?? null))
                ?? EmployeeSortColumn::Name,
            direction: SortDirection::tryFrom(self::stringValue($parameters['dir'] ?? null))
                ?? SortDirection::Ascending,
            // A page beyond the last one is corrected by the controller, so a huge number is capped, not reset.
            page: min(self::positiveInt($parameters['page'] ?? null) ?? 1, self::MAX_PAGE),
            pageSize: in_array($pageSize, self::PAGE_SIZE_OPTIONS, true) ? $pageSize : self::DEFAULT_PAGE_SIZE,
        );
    }

    /**
     * The opposite of fromParameters. Defaults are left out to keep URLs short.
     *
     * @return array<string, string|int>
     */
    public function toParameters(): array
    {
        $defaults = new self();

        return array_filter(
            [
                'q' => $this->searchText,
                'department' => $this->departmentId ?? '',
                'sort' => $this->sortBy === $defaults->sortBy ? '' : $this->sortBy->value,
                'dir' => $this->direction === $defaults->direction ? '' : $this->direction->value,
                'page' => $this->page === $defaults->page ? '' : $this->page,
                'size' => $this->pageSize === $defaults->pageSize ? '' : $this->pageSize,
            ],
            static fn (string|int $value): bool => $value !== '',
        );
    }

    /**
     * @return list<string>
     */
    public function searchTerms(): array
    {
        $terms = preg_split('/\s+/u', $this->searchText, -1, PREG_SPLIT_NO_EMPTY);

        return array_slice($terms === false ? [] : $terms, 0, self::MAX_SEARCH_TERMS);
    }

    public function offset(): int
    {
        return ($this->page - 1) * $this->pageSize;
    }

    public function withPage(int $page): self
    {
        return new self($this->searchText, $this->departmentId, $this->sortBy, $this->direction, $page, $this->pageSize);
    }

    /**
     * Clicking the column that is already sorted reverses the direction; a new column starts ascending.
     * Changing the order starts again at page 1, because the old page shows different rows now.
     */
    public function sortedBy(EmployeeSortColumn $column): self
    {
        $direction = $column === $this->sortBy ? $this->direction->reversed() : SortDirection::Ascending;

        return new self($this->searchText, $this->departmentId, $column, $direction, 1, $this->pageSize);
    }

    private static function stringValue(mixed $value): string
    {
        return is_string($value) ? $value : '';
    }

    private static function positiveInt(mixed $value): ?int
    {
        $number = filter_var($value, FILTER_VALIDATE_INT, ['options' => ['min_range' => 1]]);

        return $number === false ? null : $number;
    }
}
