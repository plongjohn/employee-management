<?php

declare(strict_types=1);

namespace EmployeeManagement\Repositories;

use EmployeeManagement\Models\Department;

interface DepartmentRepositoryInterface
{
    /**
     * @return list<Department> Sorted by name.
     */
    public function findAll(): array;
}
