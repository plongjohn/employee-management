<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\Services;

use EmployeeManagement\Services\OperationResult;
use EmployeeManagement\Services\OperationStatus;
use EmployeeManagement\Services\ValidationError;
use InvalidArgumentException;
use PHPUnit\Framework\TestCase;

final class OperationResultTest extends TestCase
{
    public function testSuccessCarriesValue(): void
    {
        $result = OperationResult::success('value');

        self::assertTrue($result->isSuccess());
        self::assertSame('value', $result->value);
    }

    public function testFailureWithSuccessStatusThrows(): void
    {
        $this->expectException(InvalidArgumentException::class);

        OperationResult::failure(OperationStatus::Success);
    }

    public function testFailureWithValidationFailedStatusThrows(): void
    {
        $this->expectException(InvalidArgumentException::class);

        OperationResult::failure(OperationStatus::ValidationFailed);
    }

    public function testValidationFailedWithoutErrorsThrows(): void
    {
        $this->expectException(InvalidArgumentException::class);

        OperationResult::validationFailed([]);
    }

    public function testValidationFailedCarriesErrors(): void
    {
        $result = OperationResult::validationFailed([ValidationError::EmailInvalid]);

        self::assertFalse($result->isSuccess());
        self::assertSame([ValidationError::EmailInvalid], $result->errors);
    }
}
