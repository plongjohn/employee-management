<?php

declare(strict_types=1);

namespace EmployeeManagement\Repositories;

use DateTimeImmutable;
use EmployeeManagement\Models\Employee;
use EmployeeManagement\Models\EmployeeInput;
use EmployeeManagement\Models\EmployeeQuery;
use EmployeeManagement\Models\EmployeeSortColumn;
use EmployeeManagement\Models\PagedResult;
use EmployeeManagement\Models\SortDirection;
use PDO;
use PDOException;
use PDOStatement;

final readonly class EmployeeRepository implements EmployeeRepositoryInterface
{
    // MariaDB error codes – see https://mariadb.com/kb/en/mariadb-error-codes/
    private const int ER_DUP_ENTRY = 1062;
    private const int ER_NO_REFERENCED_ROW_2 = 1452;

    // Name of the unique index in database/setup.sql – change both together.
    private const string EMAIL_UNIQUE_INDEX = 'uq_employees_email';

    private const string SELECT_COLUMNS_SQL = <<<'SQL'
        SELECT e.id, e.first_name, e.last_name, e.email, e.department_id, d.name AS department_name,
               e.hire_date, e.version, e.created_at, e.updated_at
        FROM employees e
        JOIN departments d ON d.id = e.department_id
        SQL;

    private const string INSERT_SQL = <<<'SQL'
        INSERT INTO employees (first_name, last_name, email, department_id, hire_date)
        VALUES (:firstName, :lastName, :email, :departmentId, :hireDate)
        SQL;

    private const string UPDATE_SQL = <<<'SQL'
        UPDATE employees
        SET first_name = :firstName, last_name = :lastName, email = :email,
            department_id = :departmentId, hire_date = :hireDate, version = version + 1
        WHERE id = :id AND version = :expectedVersion
        SQL;

    private const string DELETE_SQL = 'DELETE FROM employees WHERE id = :id AND version = :expectedVersion';

    public function __construct(
        private PDO $pdo,
    ) {
    }

    public function search(EmployeeQuery $query): PagedResult
    {
        [$whereSql, $parameters] = $this->buildFilter($query);
        $orderBySql = $this->buildOrderBy($query->sortBy, $query->direction);

        $statement = $this->prepareWithValues(
            self::SELECT_COLUMNS_SQL . " {$whereSql} ORDER BY {$orderBySql} LIMIT :pageSize OFFSET :offset",
            $parameters + ['pageSize' => $query->pageSize, 'offset' => $query->offset()],
        );
        $statement->execute();

        /** @var list<array<string, mixed>> $rows */
        $rows = $statement->fetchAll();

        $countStatement = $this->prepareWithValues("SELECT COUNT(*) FROM employees e {$whereSql}", $parameters);
        $countStatement->execute();
        $totalCount = (int) $countStatement->fetchColumn();

        return new PagedResult(
            array_map($this->mapEmployee(...), $rows),
            $totalCount,
            $query->page,
            $query->pageSize,
        );
    }

    public function findById(int $id): ?Employee
    {
        $statement = $this->pdo->prepare(self::SELECT_COLUMNS_SQL . ' WHERE e.id = :id');
        $statement->execute(['id' => $id]);

        /** @var array<string, mixed>|false $row */
        $row = $statement->fetch();

        return $row === false ? null : $this->mapEmployee($row);
    }

    public function insert(EmployeeInput $input): int
    {
        $this->translateWriteErrors(
            fn (): bool => $this->pdo->prepare(self::INSERT_SQL)->execute($this->inputParameters($input)),
        );

        return (int) $this->pdo->lastInsertId();
    }

    public function update(int $id, int $expectedVersion, EmployeeInput $input): bool
    {
        $statement = $this->pdo->prepare(self::UPDATE_SQL);
        $parameters = $this->inputParameters($input) + ['id' => $id, 'expectedVersion' => $expectedVersion];

        $this->translateWriteErrors(fn (): bool => $statement->execute($parameters));

        return $statement->rowCount() === 1;
    }

    public function delete(int $id, int $expectedVersion): bool
    {
        $statement = $this->pdo->prepare(self::DELETE_SQL);
        $statement->execute(['id' => $id, 'expectedVersion' => $expectedVersion]);

        return $statement->rowCount() === 1;
    }

    /**
     * @return array{string, array<string, string|int>}
     */
    private function buildFilter(EmployeeQuery $query): array
    {
        $conditions = [];
        $parameters = [];

        // Native prepared statements do not allow reusing a placeholder, so every column
        // gets its own parameter with the same pattern.
        foreach ($query->searchTerms() as $index => $term) {
            $pattern = SqlLike::prefix($term);
            $conditions[] = "(e.first_name LIKE :firstName{$index} OR e.last_name LIKE :lastName{$index}"
                . " OR e.email LIKE :email{$index})";
            $parameters["firstName{$index}"] = $pattern;
            $parameters["lastName{$index}"] = $pattern;
            $parameters["email{$index}"] = $pattern;
        }

        // "All departments" means no condition at all: an ":id IS NULL OR ..." clause
        // would often keep MariaDB from using the department index.
        if ($query->departmentId !== null) {
            $conditions[] = 'e.department_id = :departmentId';
            $parameters['departmentId'] = $query->departmentId;
        }

        $whereSql = $conditions === [] ? '' : 'WHERE ' . implode(' AND ', $conditions);

        return [$whereSql, $parameters];
    }

    // Column names come only from this fixed mapping, never from user input.
    // e.id is the final tie-breaker so that rows with equal values keep a stable order across pages.
    // Email is unique and needs none; appending e.id there would stop MariaDB from reading
    // uq_employees_email in order and force a sort of the whole table.
    private function buildOrderBy(EmployeeSortColumn $sortBy, SortDirection $direction): string
    {
        $columns = match ($sortBy) {
            EmployeeSortColumn::Name => ['e.last_name', 'e.first_name', 'e.id'],
            EmployeeSortColumn::Email => ['e.email'],
            EmployeeSortColumn::Department => ['d.name', 'e.last_name', 'e.first_name', 'e.id'],
            EmployeeSortColumn::HireDate => ['e.hire_date', 'e.id'],
        };
        $keyword = $direction === SortDirection::Descending ? 'DESC' : 'ASC';

        return implode(', ', array_map(static fn (string $column): string => "{$column} {$keyword}", $columns));
    }

    /**
     * @return array<string, string|int>
     */
    private function inputParameters(EmployeeInput $input): array
    {
        return [
            'firstName' => $input->firstName,
            'lastName' => $input->lastName,
            'email' => $input->email,
            'departmentId' => (int) $input->departmentId,
            'hireDate' => $input->hireDate,
        ];
    }

    /**
     * LIMIT and OFFSET only accept integers, so values are bound with their PHP type
     * instead of passing them to execute(), which sends everything as a string.
     *
     * @param array<string, string|int> $values
     */
    private function prepareWithValues(string $sql, array $values): PDOStatement
    {
        $statement = $this->pdo->prepare($sql);
        foreach ($values as $name => $value) {
            $statement->bindValue($name, $value, is_int($value) ? PDO::PARAM_INT : PDO::PARAM_STR);
        }

        return $statement;
    }

    /**
     * @param array<string, mixed> $row
     */
    private function mapEmployee(array $row): Employee
    {
        return new Employee(
            id: (int) $row['id'],
            firstName: (string) $row['first_name'],
            lastName: (string) $row['last_name'],
            email: (string) $row['email'],
            departmentId: (int) $row['department_id'],
            departmentName: (string) $row['department_name'],
            hireDate: new DateTimeImmutable((string) $row['hire_date']),
            version: (int) $row['version'],
            createdAt: new DateTimeImmutable((string) $row['created_at']),
            updatedAt: new DateTimeImmutable((string) $row['updated_at']),
        );
    }

    /**
     * @param callable(): bool $write
     */
    private function translateWriteErrors(callable $write): void
    {
        try {
            $write();
        } catch (PDOException $exception) {
            throw match ($this->driverErrorCode($exception)) {
                self::ER_DUP_ENTRY => $this->isDuplicateEmail($exception)
                    ? new DuplicateEmailException($exception)
                    : $exception,
                self::ER_NO_REFERENCED_ROW_2 => new DepartmentNotFoundException($exception),
                default => $exception,
            };
        }
    }

    private function driverErrorCode(PDOException $exception): ?int
    {
        $code = $exception->errorInfo[1] ?? null;

        return is_int($code) ? $code : null;
    }

    // The error code alone would also match any future unique index, so the index name is checked too.
    private function isDuplicateEmail(PDOException $exception): bool
    {
        return str_contains($exception->getMessage(), self::EMAIL_UNIQUE_INDEX);
    }
}
