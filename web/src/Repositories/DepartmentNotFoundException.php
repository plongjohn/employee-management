<?php

declare(strict_types=1);

namespace EmployeeManagement\Repositories;

use RuntimeException;
use Throwable;

final class DepartmentNotFoundException extends RuntimeException
{
    public function __construct(?Throwable $previous = null)
    {
        parent::__construct('The department does not exist.', 0, $previous);
    }
}
