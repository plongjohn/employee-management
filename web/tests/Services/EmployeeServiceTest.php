<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\Services;

use EmployeeManagement\Models\Employee;
use EmployeeManagement\Models\EmployeeQuery;
use EmployeeManagement\Services\EmployeeService;
use EmployeeManagement\Services\EmployeeValidator;
use EmployeeManagement\Services\OperationResult;
use EmployeeManagement\Services\OperationStatus;
use EmployeeManagement\Services\ValidationError;
use EmployeeManagement\Tests\Fakes\InMemoryEmployeeRepository;
use EmployeeManagement\Tests\TestData;
use PHPUnit\Framework\TestCase;
use Psr\Log\NullLogger;

final class EmployeeServiceTest extends TestCase
{
    private InMemoryEmployeeRepository $repository;
    private EmployeeService $service;

    protected function setUp(): void
    {
        $this->repository = new InMemoryEmployeeRepository();
        $this->service = new EmployeeService(
            $this->repository,
            new EmployeeValidator(TestData::clock()),
            new NullLogger(),
        );
    }

    public function testSearchPassesQueryToRepository(): void
    {
        $query = new EmployeeQuery(searchText: 'anna', departmentId: 2, page: 3, pageSize: 10);

        $this->service->search($query);

        self::assertSame($query, $this->repository->lastQuery);
    }

    public function testCreateWithValidInputReturnsStoredEmployee(): void
    {
        $result = $this->service->create(TestData::validInput());

        $employee = $this->successValue($result);
        self::assertSame('Anna', $employee->firstName);
        self::assertSame('Produktion', $employee->departmentName);
        self::assertSame(1, $employee->version);
        self::assertSame($employee, $this->repository->findById($employee->id));
    }

    public function testCreateStoresTrimmedValues(): void
    {
        $result = $this->service->create(TestData::validInput(
            firstName: "  Anna\u{00A0}",
            lastName: "\tMüller ",
            email: ' anna.mueller@example.com ',
            hireDate: ' 2012-04-01 ',
        ));

        $employee = $this->successValue($result);
        self::assertSame('Anna', $employee->firstName);
        self::assertSame('Müller', $employee->lastName);
        self::assertSame('anna.mueller@example.com', $employee->email);
    }

    public function testCreateWithInvalidInputReturnsErrorsWithoutWriting(): void
    {
        $result = $this->service->create(TestData::validInput(firstName: '   '));

        self::assertSame(OperationStatus::ValidationFailed, $result->status);
        self::assertSame([ValidationError::FirstNameRequired], $result->errors);
        self::assertSame(0, $this->repository->writeCount);
    }

    public function testCreateWithEmailInDifferentCaseReturnsDuplicateEmail(): void
    {
        $this->repository->add(TestData::validInput());

        $result = $this->service->create(TestData::validInput(firstName: 'Anne', email: 'Anna.Mueller@Example.com'));

        self::assertSame(OperationStatus::DuplicateEmail, $result->status);
    }

    public function testCreateWithUnknownDepartmentReturnsDepartmentNotFound(): void
    {
        $result = $this->service->create(TestData::validInput(departmentId: 99));

        self::assertSame(OperationStatus::ValidationFailed, $result->status);
        self::assertSame([ValidationError::DepartmentNotFound], $result->errors);
    }

    public function testUpdateWithCurrentVersionReturnsEmployeeWithNewVersion(): void
    {
        $employee = $this->repository->add(TestData::validInput());

        $result = $this->service->update($employee->id, $employee->version, TestData::validInput(departmentId: 2));

        $updated = $this->successValue($result);
        self::assertSame('IT', $updated->departmentName);
        self::assertSame(2, $updated->version);
    }

    public function testUpdateAfterChangeByOtherUserReturnsConflict(): void
    {
        $employee = $this->repository->add(TestData::validInput());
        $this->repository->changeByOtherUser($employee->id);

        $result = $this->service->update($employee->id, $employee->version, TestData::validInput(firstName: 'Anne'));

        self::assertSame(OperationStatus::Conflict, $result->status);
        self::assertSame('Anna', $this->repository->findById($employee->id)?->firstName);
    }

    public function testUpdateAfterDeleteByOtherUserReturnsNotFound(): void
    {
        $employee = $this->repository->add(TestData::validInput());
        $this->repository->deleteByOtherUser($employee->id);

        $result = $this->service->update($employee->id, $employee->version, TestData::validInput());

        self::assertSame(OperationStatus::NotFound, $result->status);
    }

    public function testUpdateWithEmailOfOtherEmployeeReturnsDuplicateEmail(): void
    {
        $this->repository->add(TestData::validInput(email: 'taken@example.com'));
        $employee = $this->repository->add(TestData::validInput());

        $result = $this->service->update(
            $employee->id,
            $employee->version,
            TestData::validInput(email: 'taken@example.com'),
        );

        self::assertSame(OperationStatus::DuplicateEmail, $result->status);
    }

    public function testUpdateWithUnknownDepartmentReturnsDepartmentNotFound(): void
    {
        $employee = $this->repository->add(TestData::validInput());

        $result = $this->service->update($employee->id, $employee->version, TestData::validInput(departmentId: 99));

        self::assertSame([ValidationError::DepartmentNotFound], $result->errors);
    }

    public function testUpdateWithInvalidInputReturnsErrorsWithoutWriting(): void
    {
        $employee = $this->repository->add(TestData::validInput());

        $result = $this->service->update($employee->id, $employee->version, TestData::validInput(email: 'invalid'));

        self::assertSame([ValidationError::EmailInvalid], $result->errors);
        self::assertSame(0, $this->repository->writeCount);
    }

    public function testDeleteWithCurrentVersionRemovesEmployee(): void
    {
        $employee = $this->repository->add(TestData::validInput());

        $result = $this->service->delete($employee->id, $employee->version);

        self::assertTrue($result->isSuccess());
        self::assertNull($this->repository->findById($employee->id));
    }

    public function testDeleteAfterChangeByOtherUserReturnsConflictAndKeepsEmployee(): void
    {
        $employee = $this->repository->add(TestData::validInput());
        $this->repository->changeByOtherUser($employee->id);

        $result = $this->service->delete($employee->id, $employee->version);

        self::assertSame(OperationStatus::Conflict, $result->status);
        self::assertNotNull($this->repository->findById($employee->id));
    }

    public function testDeleteAfterDeleteByOtherUserReturnsNotFound(): void
    {
        $employee = $this->repository->add(TestData::validInput());
        $this->repository->deleteByOtherUser($employee->id);

        $result = $this->service->delete($employee->id, $employee->version);

        self::assertSame(OperationStatus::NotFound, $result->status);
    }

    /**
     * @param OperationResult<Employee> $result
     */
    private function successValue(OperationResult $result): Employee
    {
        self::assertSame(OperationStatus::Success, $result->status);
        self::assertNotNull($result->value);

        return $result->value;
    }
}
