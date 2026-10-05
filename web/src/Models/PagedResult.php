<?php

declare(strict_types=1);

namespace EmployeeManagement\Models;

/**
 * @template-covariant T
 */
final readonly class PagedResult
{
    /**
     * @param list<T> $items
     */
    public function __construct(
        public array $items,
        public int $totalCount,
        public int $page,
        public int $pageSize,
    ) {
    }

    public function totalPages(): int
    {
        return $this->totalCount === 0 ? 0 : (int) ceil($this->totalCount / $this->pageSize);
    }

    /**
     * True when the requested page lies behind the last one, e.g. after the last employee
     * of the last page was deleted.
     */
    public function isBeyondLastPage(): bool
    {
        return $this->items === [] && $this->page > 1 && $this->totalCount > 0;
    }
}
