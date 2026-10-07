<?php

declare(strict_types=1);

namespace EmployeeManagement\Services;

use DateTimeImmutable;
use EmployeeManagement\Models\EmployeeInput;
use Psr\Clock\ClockInterface;

final readonly class EmployeeValidator
{
    // Match the column sizes in database/setup.sql – change both together.
    public const int NAME_MAX_LENGTH = 100;
    public const int EMAIL_MAX_LENGTH = 255;

    public const string EARLIEST_HIRE_DATE = '1950-01-01';

    // Planned hires may be entered in advance, but not further ahead than a year.
    public const int MAX_YEARS_IN_FUTURE = 1;

    // Deliberately simple and identical to the desktop app's pattern, so both apps accept the
    // same addresses. Built-in validators (filter_var, MailAddress) disagree on edge cases.
    private const string EMAIL_PATTERN = '/^[^@\s]+@[^@\s]+\.[^@\s]+$/u';

    public function __construct(
        private ClockInterface $clock,
    ) {
    }

    public static function earliestHireDate(): DateTimeImmutable
    {
        return new DateTimeImmutable(self::EARLIEST_HIRE_DATE);
    }

    public function latestHireDate(): DateTimeImmutable
    {
        return self::addYears($this->clock->now()->setTime(0, 0), self::MAX_YEARS_IN_FUTURE);
    }

    /**
     * Expects trimmed input (see EmployeeInput::trimmed()).
     *
     * @return list<ValidationError>
     */
    public function validate(EmployeeInput $input): array
    {
        return array_values(array_filter([
            $this->validateName($input->firstName, ValidationError::FirstNameRequired, ValidationError::FirstNameTooLong),
            $this->validateName($input->lastName, ValidationError::LastNameRequired, ValidationError::LastNameTooLong),
            $this->validateEmail($input->email),
            $this->validateDepartment($input->departmentId),
            $this->validateHireDate($input),
        ]));
    }

    private function validateName(string $name, ValidationError $required, ValidationError $tooLong): ?ValidationError
    {
        return match (true) {
            $name === '' => $required,
            mb_strlen($name) > self::NAME_MAX_LENGTH => $tooLong,
            default => null,
        };
    }

    private function validateEmail(string $email): ?ValidationError
    {
        return match (true) {
            $email === '' => ValidationError::EmailRequired,
            mb_strlen($email) > self::EMAIL_MAX_LENGTH => ValidationError::EmailTooLong,
            preg_match(self::EMAIL_PATTERN, $email) !== 1 => ValidationError::EmailInvalid,
            default => null,
        };
    }

    private function validateDepartment(?int $departmentId): ?ValidationError
    {
        return $departmentId === null || $departmentId <= 0 ? ValidationError::DepartmentRequired : null;
    }

    private function validateHireDate(EmployeeInput $input): ?ValidationError
    {
        if ($input->hireDate === '') {
            return ValidationError::HireDateRequired;
        }

        $hireDate = $input->parsedHireDate();

        return match (true) {
            $hireDate === null => ValidationError::HireDateInvalid,
            $hireDate < self::earliestHireDate() => ValidationError::HireDateTooEarly,
            $hireDate > $this->latestHireDate() => ValidationError::HireDateTooFarInFuture,
            default => null,
        };
    }

    // modify('+1 year') turns 29 February into 1 March; clamping to the last day of the month
    // gives 28 February instead, like DateOnly.AddYears in the desktop app.
    private static function addYears(DateTimeImmutable $date, int $years): DateTimeImmutable
    {
        $year = (int) $date->format('Y') + $years;
        $month = (int) $date->format('n');
        $daysInMonth = (int) $date->setDate($year, $month, 1)->format('t');

        return $date->setDate($year, $month, min((int) $date->format('j'), $daysInMonth));
    }
}
