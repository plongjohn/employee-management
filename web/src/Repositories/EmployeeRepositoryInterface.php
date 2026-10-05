<?php

declare(strict_types=1);

namespace EmployeeManagement\Repositories;

use EmployeeManagement\Models\Employee;
use EmployeeManagement\Models\EmployeeInput;
use EmployeeManagement\Models\EmployeeQuery;
use EmployeeManagement\Models\PagedResult;

interface EmployeeRepositoryInterface
{
    /**
     * @return PagedResult<Employee>
     */
    public function search(EmployeeQuery $query): PagedResult;

    public function findById(int $id): ?Employee;

    /**
     * Expects validated input.
     *
     * @return int The id of the new employee.
     * @throws DuplicateEmailException
     * @throws DepartmentNotFoundException
     */
    public function insert(EmployeeInput $input): int;

    /**
     * @return bool False if no employee with this id and version exists.
     * @throws DuplicateEmailException
     * @throws DepartmentNotFoundException
     */
    public function update(int $id, int $expectedVersion, EmployeeInput $input): bool;

    /**
     * @return bool False if no employee with this id and version exists.
     */
    public function delete(int $id, int $expectedVersion): bool;
}
