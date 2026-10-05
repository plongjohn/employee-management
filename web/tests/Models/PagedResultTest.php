<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\Models;

use EmployeeManagement\Models\PagedResult;
use PHPUnit\Framework\Attributes\TestWith;
use PHPUnit\Framework\TestCase;

final class PagedResultTest extends TestCase
{
    #[TestWith([0, 25, 0])]
    #[TestWith([1, 25, 1])]
    #[TestWith([25, 25, 1])]
    #[TestWith([26, 25, 2])]
    #[TestWith([100, 10, 10])]
    public function testTotalPagesRoundsUpToFullPages(int $totalCount, int $pageSize, int $expectedPages): void
    {
        self::assertSame($expectedPages, (new PagedResult([], $totalCount, 1, $pageSize))->totalPages());
    }

    public function testEmptyPageAfterTheLastIsBeyondLastPage(): void
    {
        self::assertTrue((new PagedResult([], 25, 2, 25))->isBeyondLastPage());
    }

    public function testEmptyFirstPageIsNotBeyondLastPage(): void
    {
        self::assertFalse((new PagedResult([], 0, 1, 25))->isBeyondLastPage());
    }

    public function testPageWithItemsIsNotBeyondLastPage(): void
    {
        self::assertFalse((new PagedResult(['item'], 26, 2, 25))->isBeyondLastPage());
    }
}
