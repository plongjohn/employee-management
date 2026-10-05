<?php

declare(strict_types=1);

namespace EmployeeManagement\Services;

use InvalidArgumentException;

/**
 * Outcome of a write operation. Expected failures (validation, duplicate email, conflicts)
 * are results, not exceptions; exceptions are reserved for unexpected errors.
 *
 * @template-covariant T
 */
final readonly class OperationResult
{
    /**
     * @param list<ValidationError> $errors
     * @param T|null $value Set only when the operation succeeded.
     */
    private function __construct(
        public OperationStatus $status,
        public array $errors,
        public mixed $value,
    ) {
    }

    /**
     * @template TValue
     * @param TValue $value
     * @return self<TValue>
     */
    public static function success(mixed $value = null): self
    {
        return new self(OperationStatus::Success, [], $value);
    }

    /**
     * @param list<ValidationError> $errors
     * @return self<never>
     */
    public static function validationFailed(array $errors): self
    {
        if ($errors === []) {
            throw new InvalidArgumentException('A validation failure needs at least one error.');
        }

        return new self(OperationStatus::ValidationFailed, $errors, null);
    }

    /**
     * @return self<never>
     */
    public static function failure(OperationStatus $status): self
    {
        if ($status === OperationStatus::Success || $status === OperationStatus::ValidationFailed) {
            throw new InvalidArgumentException('Use success() or validationFailed() for this status.');
        }

        return new self($status, [], null);
    }

    public function isSuccess(): bool
    {
        return $this->status === OperationStatus::Success;
    }
}
