<?php

declare(strict_types=1);

namespace EmployeeManagement\Models;

use DateTimeImmutable;

/**
 * The values a user enters to create or change an employee, as they arrive from the form.
 * Department and hire date stay loosely typed until validation, so an empty or malformed
 * value can be reported to the user instead of failing while reading the request.
 */
final readonly class EmployeeInput
{
    // Format of <input type="date"> and of MariaDB DATE literals.
    public const string DATE_FORMAT = 'Y-m-d';

    public function __construct(
        public string $firstName,
        public string $lastName,
        public string $email,
        public ?int $departmentId,
        public string $hireDate,
    ) {
    }

    /**
     * @param array<string, mixed> $values
     */
    public static function fromForm(array $values): self
    {
        $departmentId = filter_var(self::stringValue($values, 'departmentId'), FILTER_VALIDATE_INT);

        return new self(
            self::stringValue($values, 'firstName'),
            self::stringValue($values, 'lastName'),
            self::stringValue($values, 'email'),
            $departmentId === false ? null : $departmentId,
            self::stringValue($values, 'hireDate'),
        );
    }

    public static function empty(DateTimeImmutable $hireDate): self
    {
        return new self('', '', '', null, $hireDate->format(self::DATE_FORMAT));
    }

    public function trimmed(): self
    {
        return new self(
            mb_trim($this->firstName),
            mb_trim($this->lastName),
            mb_trim($this->email),
            $this->departmentId,
            mb_trim($this->hireDate),
        );
    }

    /**
     * Returns null for an empty or malformed date, including impossible ones like 2026-02-30.
     */
    public function parsedHireDate(): ?DateTimeImmutable
    {
        // "!" resets the time to midnight, so the date compares cleanly against "today".
        $date = DateTimeImmutable::createFromFormat('!' . self::DATE_FORMAT, $this->hireDate);

        return $date !== false && $date->format(self::DATE_FORMAT) === $this->hireDate ? $date : null;
    }

    /**
     * @return array{firstName: string, lastName: string, email: string, departmentId: string, hireDate: string}
     */
    public function toForm(): array
    {
        return [
            'firstName' => $this->firstName,
            'lastName' => $this->lastName,
            'email' => $this->email,
            'departmentId' => $this->departmentId === null ? '' : (string) $this->departmentId,
            'hireDate' => $this->hireDate,
        ];
    }

    /**
     * @param array<string, mixed> $values
     */
    private static function stringValue(array $values, string $key): string
    {
        $value = $values[$key] ?? '';

        return is_string($value) ? $value : '';
    }
}
