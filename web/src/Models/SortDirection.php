<?php

declare(strict_types=1);

namespace EmployeeManagement\Models;

enum SortDirection: string
{
    case Ascending = 'asc';
    case Descending = 'desc';

    public function reversed(): self
    {
        return $this === self::Ascending ? self::Descending : self::Ascending;
    }
}
