<?php

declare(strict_types=1);

namespace EmployeeManagement\Models;

final readonly class Department
{
    public function __construct(
        public int $id,
        public string $name,
    ) {
    }
}
