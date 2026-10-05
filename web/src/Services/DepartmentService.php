<?php

declare(strict_types=1);

namespace EmployeeManagement\Services;

use EmployeeManagement\Models\Department;
use EmployeeManagement\Repositories\DepartmentRepositoryInterface;

final readonly class DepartmentService
{
    public function __construct(
        private DepartmentRepositoryInterface $departmentRepository,
    ) {
    }

    /**
     * @return list<Department>
     */
    public function findAll(): array
    {
        return $this->departmentRepository->findAll();
    }
}
