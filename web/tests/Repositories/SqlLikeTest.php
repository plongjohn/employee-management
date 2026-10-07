<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\Repositories;

use EmployeeManagement\Repositories\SqlLike;
use PHPUnit\Framework\Attributes\TestWith;
use PHPUnit\Framework\TestCase;

final class SqlLikeTest extends TestCase
{
    #[TestWith(['anna', 'anna%'])]
    #[TestWith(['50%', '50\\%%'])]
    #[TestWith(['a_b', 'a\\_b%'])]
    #[TestWith(['a\\b', 'a\\\\b%'])]
    #[TestWith(['\\%', '\\\\\\%%'])]
    public function testPrefixEscapesWildcardsAndAppendsPercent(string $term, string $expected): void
    {
        self::assertSame($expected, SqlLike::prefix($term));
    }
}
