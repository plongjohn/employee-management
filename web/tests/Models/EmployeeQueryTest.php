<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\Models;

use EmployeeManagement\Models\EmployeeQuery;
use EmployeeManagement\Models\EmployeeSortColumn;
use EmployeeManagement\Models\SortDirection;
use InvalidArgumentException;
use PHPUnit\Framework\Attributes\TestWith;
use PHPUnit\Framework\TestCase;

final class EmployeeQueryTest extends TestCase
{
    #[TestWith([''])]
    #[TestWith(['   '])]
    public function testSearchTermsOfEmptyTextAreEmpty(string $searchText): void
    {
        self::assertSame([], (new EmployeeQuery(searchText: $searchText))->searchTerms());
    }

    public function testSearchTermsSplitAtAnyWhitespace(): void
    {
        $query = new EmployeeQuery(searchText: "  anna \t mü\n");

        self::assertSame(['anna', 'mü'], $query->searchTerms());
    }

    public function testSearchTermsKeepOnlyTheFirstWords(): void
    {
        $query = new EmployeeQuery(searchText: 'a b c d e f g');

        self::assertSame(['a', 'b', 'c', 'd', 'e'], $query->searchTerms());
    }

    #[TestWith([1, 25, 0])]
    #[TestWith([2, 25, 25])]
    #[TestWith([3, 10, 20])]
    public function testOffsetSkipsPreviousPages(int $page, int $pageSize, int $expectedOffset): void
    {
        self::assertSame($expectedOffset, (new EmployeeQuery(page: $page, pageSize: $pageSize))->offset());
    }

    #[TestWith([0, 25])]
    #[TestWith([1, 0])]
    #[TestWith([1, 101])]
    public function testPagingOutOfRangeThrows(int $page, int $pageSize): void
    {
        $this->expectException(InvalidArgumentException::class);

        new EmployeeQuery(page: $page, pageSize: $pageSize);
    }

    public function testFromParametersReadsAllValues(): void
    {
        $query = EmployeeQuery::fromParameters([
            'q' => ' anna ',
            'department' => '3',
            'sort' => 'hireDate',
            'dir' => 'desc',
            'page' => '2',
            'size' => '50',
        ]);

        self::assertEquals(
            new EmployeeQuery('anna', 3, EmployeeSortColumn::HireDate, SortDirection::Descending, 2, 50),
            $query,
        );
    }

    public function testFromParametersFallsBackToDefaultsForInvalidValues(): void
    {
        $query = EmployeeQuery::fromParameters([
            'q' => ['array'],
            'department' => 'abc',
            'sort' => 'salary',
            'dir' => 'up',
            'page' => '-1',
            'size' => '7',
        ]);

        self::assertEquals(new EmployeeQuery(), $query);
    }

    public function testToParametersLeavesOutDefaults(): void
    {
        self::assertSame([], (new EmployeeQuery())->toParameters());
    }

    public function testToParametersRoundTripsThroughFromParameters(): void
    {
        $query = new EmployeeQuery('mü', 2, EmployeeSortColumn::Email, SortDirection::Descending, 4, 10);

        self::assertEquals($query, EmployeeQuery::fromParameters($query->toParameters()));
    }

    public function testSortedByOtherColumnStartsAscendingOnFirstPage(): void
    {
        $query = new EmployeeQuery(direction: SortDirection::Descending, page: 3);

        $sorted = $query->sortedBy(EmployeeSortColumn::Email);

        self::assertSame(EmployeeSortColumn::Email, $sorted->sortBy);
        self::assertSame(SortDirection::Ascending, $sorted->direction);
        self::assertSame(1, $sorted->page);
    }

    public function testSortedBySameColumnReversesDirection(): void
    {
        $query = new EmployeeQuery(sortBy: EmployeeSortColumn::Name, direction: SortDirection::Ascending);

        self::assertSame(SortDirection::Descending, $query->sortedBy(EmployeeSortColumn::Name)->direction);
    }
}
