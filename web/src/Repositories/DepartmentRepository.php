<?php

declare(strict_types=1);

namespace EmployeeManagement\Repositories;

use EmployeeManagement\Models\Department;
use PDO;

final readonly class DepartmentRepository implements DepartmentRepositoryInterface
{
    private const string SELECT_ALL_SQL = 'SELECT id, name FROM departments ORDER BY name';

    public function __construct(
        private PDO $pdo,
    ) {
    }

    public function findAll(): array
    {
        $statement = $this->pdo->prepare(self::SELECT_ALL_SQL);
        $statement->execute();

        /** @var list<array{id: int|string, name: string}> $rows */
        $rows = $statement->fetchAll();

        return array_map(
            static fn (array $row): Department => new Department((int) $row['id'], $row['name']),
            $rows,
        );
    }
}
