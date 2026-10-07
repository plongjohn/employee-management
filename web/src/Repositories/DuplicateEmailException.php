<?php

declare(strict_types=1);

namespace EmployeeManagement\Repositories;

use RuntimeException;
use Throwable;

final class DuplicateEmailException extends RuntimeException
{
    public function __construct(?Throwable $previous = null)
    {
        parent::__construct('The email address is already used by another employee.', 0, $previous);
    }
}
