<?php

declare(strict_types=1);

namespace EmployeeManagement\Services;

use EmployeeManagement\Models\Employee;
use EmployeeManagement\Models\EmployeeInput;
use EmployeeManagement\Models\EmployeeQuery;
use EmployeeManagement\Models\PagedResult;
use EmployeeManagement\Repositories\DepartmentNotFoundException;
use EmployeeManagement\Repositories\DuplicateEmailException;
use EmployeeManagement\Repositories\EmployeeRepositoryInterface;
use Psr\Log\LoggerInterface;

// Log messages contain ids only – names and email addresses are personal data and stay out of the logs.
final readonly class EmployeeService
{
    public function __construct(
        private EmployeeRepositoryInterface $employeeRepository,
        private EmployeeValidator $validator,
        private LoggerInterface $logger,
    ) {
    }

    /**
     * @return PagedResult<Employee>
     */
    public function search(EmployeeQuery $query): PagedResult
    {
        return $this->employeeRepository->search($query);
    }

    public function findById(int $id): ?Employee
    {
        return $this->employeeRepository->findById($id);
    }

    /**
     * @return OperationResult<Employee>
     */
    public function create(EmployeeInput $input): OperationResult
    {
        $trimmed = $input->trimmed();
        $errors = $this->validator->validate($trimmed);
        if ($errors !== []) {
            return OperationResult::validationFailed($errors);
        }

        try {
            $id = $this->employeeRepository->insert($trimmed);
            $this->logger->info('Created employee {employeeId}', ['employeeId' => $id]);

            return $this->reload($id);
        } catch (DuplicateEmailException) {
            $this->logger->warning('Rejected new employee: email address already in use');

            return OperationResult::failure(OperationStatus::DuplicateEmail);
        } catch (DepartmentNotFoundException) {
            $this->logger->warning('Rejected new employee: department no longer exists');

            return OperationResult::validationFailed([ValidationError::DepartmentNotFound]);
        }
    }

    /**
     * @return OperationResult<Employee>
     */
    public function update(int $id, int $version, EmployeeInput $input): OperationResult
    {
        $trimmed = $input->trimmed();
        $errors = $this->validator->validate($trimmed);
        if ($errors !== []) {
            return OperationResult::validationFailed($errors);
        }

        try {
            if (!$this->employeeRepository->update($id, $version, $trimmed)) {
                $status = $this->explainMissedWrite($id);
                $this->logger->warning('Update of employee {employeeId} failed: {status}', [
                    'employeeId' => $id,
                    'status' => $status->name,
                ]);

                return OperationResult::failure($status);
            }

            $this->logger->info('Updated employee {employeeId}', ['employeeId' => $id]);

            return $this->reload($id);
        } catch (DuplicateEmailException) {
            $this->logger->warning(
                'Update of employee {employeeId} rejected: email address already in use',
                ['employeeId' => $id],
            );

            return OperationResult::failure(OperationStatus::DuplicateEmail);
        } catch (DepartmentNotFoundException) {
            $this->logger->warning(
                'Update of employee {employeeId} rejected: department no longer exists',
                ['employeeId' => $id],
            );

            return OperationResult::validationFailed([ValidationError::DepartmentNotFound]);
        }
    }

    /**
     * @return OperationResult<null>
     */
    public function delete(int $id, int $version): OperationResult
    {
        if (!$this->employeeRepository->delete($id, $version)) {
            $status = $this->explainMissedWrite($id);
            $this->logger->warning('Delete of employee {employeeId} failed: {status}', [
                'employeeId' => $id,
                'status' => $status->name,
            ]);

            return OperationResult::failure($status);
        }

        $this->logger->info('Deleted employee {employeeId}', ['employeeId' => $id]);

        return OperationResult::success();
    }

    /**
     * A write that matched no row either lost a race against a delete (NotFound) or against
     * another update that already raised the version (Conflict).
     */
    private function explainMissedWrite(int $id): OperationStatus
    {
        return $this->employeeRepository->findById($id) === null
            ? OperationStatus::NotFound
            : OperationStatus::Conflict;
    }

    /**
     * Reloading returns the values the database actually stored, including the new version.
     *
     * @return OperationResult<Employee>
     */
    private function reload(int $id): OperationResult
    {
        $employee = $this->employeeRepository->findById($id);

        return $employee === null
            ? OperationResult::failure(OperationStatus::NotFound)
            : OperationResult::success($employee);
    }
}
