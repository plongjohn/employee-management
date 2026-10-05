<?php

declare(strict_types=1);

namespace EmployeeManagement\Services;

/**
 * Language-neutral validation results. The view maps each code to a localized text.
 */
enum ValidationError
{
    case FirstNameRequired;
    case FirstNameTooLong;
    case LastNameRequired;
    case LastNameTooLong;
    case EmailRequired;
    case EmailTooLong;
    case EmailInvalid;
    case DepartmentRequired;
    case DepartmentNotFound;
    case HireDateRequired;
    case HireDateInvalid;
    case HireDateTooEarly;
    case HireDateTooFarInFuture;

    /**
     * The form field the error belongs to, named like the fields of EmployeeInput.
     */
    public function field(): string
    {
        return match ($this) {
            self::FirstNameRequired, self::FirstNameTooLong => 'firstName',
            self::LastNameRequired, self::LastNameTooLong => 'lastName',
            self::EmailRequired, self::EmailTooLong, self::EmailInvalid => 'email',
            self::DepartmentRequired, self::DepartmentNotFound => 'departmentId',
            self::HireDateRequired, self::HireDateInvalid, self::HireDateTooEarly,
            self::HireDateTooFarInFuture => 'hireDate',
        };
    }
}
