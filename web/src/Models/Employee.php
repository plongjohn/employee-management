<?php

declare(strict_types=1);

namespace EmployeeManagement\Models;

use DateTimeImmutable;

final readonly class Employee
{
    public function __construct(
        public int $id,
        public string $firstName,
        public string $lastName,
        public string $email,
        public int $departmentId,
        public string $departmentName,
        public DateTimeImmutable $hireDate,
        public int $version,
        public DateTimeImmutable $createdAt,
        public DateTimeImmutable $updatedAt,
    ) {
    }

    public function toInput(): EmployeeInput
    {
        return new EmployeeInput(
            $this->firstName,
            $this->lastName,
            $this->email,
            $this->departmentId,
            $this->hireDate->format(EmployeeInput::DATE_FORMAT),
        );
    }
}
