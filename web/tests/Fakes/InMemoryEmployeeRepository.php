<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\Fakes;

use DateTimeImmutable;
use EmployeeManagement\Models\Employee;
use EmployeeManagement\Models\EmployeeInput;
use EmployeeManagement\Models\EmployeeQuery;
use EmployeeManagement\Models\PagedResult;
use EmployeeManagement\Repositories\DepartmentNotFoundException;
use EmployeeManagement\Repositories\DuplicateEmailException;
use EmployeeManagement\Repositories\EmployeeRepositoryInterface;

/**
 * Mimics the database rules the service relies on: version check, unique email
 * (case-insensitive like the collation) and the department foreign key.
 */
final class InMemoryEmployeeRepository implements EmployeeRepositoryInterface
{
    private const array DEPARTMENTS = [1 => 'Produktion', 2 => 'IT'];
    private const string FIXED_TIMESTAMP = '2026-10-05 12:00:00';

    /** @var array<int, Employee> */
    private array $employees = [];
    private int $nextId = 1;

    public int $writeCount = 0;
    public ?EmployeeQuery $lastQuery = null;

    public function add(EmployeeInput $input): Employee
    {
        $employee = $this->toEmployee($this->nextId++, $input, 1);
        $this->employees[$employee->id] = $employee;

        return $employee;
    }

    /** Simulates another user saving the employee in the meantime. */
    public function changeByOtherUser(int $id): void
    {
        $current = $this->employees[$id];
        $this->employees[$id] = $this->toEmployee($id, $current->toInput(), $current->version + 1);
    }

    /** Simulates another user deleting the employee in the meantime. */
    public function deleteByOtherUser(int $id): void
    {
        unset($this->employees[$id]);
    }

    public function search(EmployeeQuery $query): PagedResult
    {
        $this->lastQuery = $query;
        $items = array_values($this->employees);

        return new PagedResult($items, count($items), $query->page, $query->pageSize);
    }

    public function findById(int $id): ?Employee
    {
        return $this->employees[$id] ?? null;
    }

    public function insert(EmployeeInput $input): int
    {
        $this->writeCount++;
        $this->ensureDatabaseRules($input, null);

        return $this->add($input)->id;
    }

    public function update(int $id, int $expectedVersion, EmployeeInput $input): bool
    {
        $this->writeCount++;
        $current = $this->employees[$id] ?? null;
        if ($current === null || $current->version !== $expectedVersion) {
            return false;
        }

        $this->ensureDatabaseRules($input, $id);
        $this->employees[$id] = $this->toEmployee($id, $input, $current->version + 1);

        return true;
    }

    public function delete(int $id, int $expectedVersion): bool
    {
        $this->writeCount++;
        $current = $this->employees[$id] ?? null;
        if ($current === null || $current->version !== $expectedVersion) {
            return false;
        }

        unset($this->employees[$id]);

        return true;
    }

    private function ensureDatabaseRules(EmployeeInput $input, ?int $ownId): void
    {
        foreach ($this->employees as $employee) {
            if ($employee->id !== $ownId && mb_strtolower($employee->email) === mb_strtolower($input->email)) {
                throw new DuplicateEmailException();
            }
        }

        if (!isset(self::DEPARTMENTS[$input->departmentId ?? 0])) {
            throw new DepartmentNotFoundException();
        }
    }

    private function toEmployee(int $id, EmployeeInput $input, int $version): Employee
    {
        $departmentId = $input->departmentId ?? 0;

        return new Employee(
            id: $id,
            firstName: $input->firstName,
            lastName: $input->lastName,
            email: $input->email,
            departmentId: $departmentId,
            departmentName: self::DEPARTMENTS[$departmentId] ?? '',
            hireDate: new DateTimeImmutable($input->hireDate),
            version: $version,
            createdAt: new DateTimeImmutable(self::FIXED_TIMESTAMP),
            updatedAt: new DateTimeImmutable(self::FIXED_TIMESTAMP),
        );
    }
}
