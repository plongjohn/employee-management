<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests;

use DateTimeImmutable;
use EmployeeManagement\Models\EmployeeInput;
use EmployeeManagement\Tests\Fakes\FixedClock;

final class TestData
{
    public const string TODAY = '2026-10-05';

    public static function clock(): FixedClock
    {
        return new FixedClock(new DateTimeImmutable(self::TODAY . ' 14:30:00'));
    }

    public static function validInput(
        string $firstName = 'Anna',
        string $lastName = 'Müller',
        string $email = 'anna.mueller@example.com',
        ?int $departmentId = 1,
        string $hireDate = '2012-04-01',
    ): EmployeeInput {
        return new EmployeeInput($firstName, $lastName, $email, $departmentId, $hireDate);
    }
}
