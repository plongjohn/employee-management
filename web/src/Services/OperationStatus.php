<?php

declare(strict_types=1);

namespace EmployeeManagement\Services;

enum OperationStatus
{
    case Success;
    case ValidationFailed;
    case DuplicateEmail;
    case Conflict;
    case NotFound;
}
