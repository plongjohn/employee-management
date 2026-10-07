<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\Services;

use DateTimeImmutable;
use EmployeeManagement\Services\EmployeeValidator;
use EmployeeManagement\Services\ValidationError;
use EmployeeManagement\Tests\Fakes\FixedClock;
use EmployeeManagement\Tests\TestData;
use PHPUnit\Framework\Attributes\DataProvider;
use PHPUnit\Framework\Attributes\TestWith;
use PHPUnit\Framework\TestCase;

final class EmployeeValidatorTest extends TestCase
{
    private EmployeeValidator $validator;

    protected function setUp(): void
    {
        $this->validator = new EmployeeValidator(TestData::clock());
    }

    public function testValidInputHasNoErrors(): void
    {
        self::assertSame([], $this->validator->validate(TestData::validInput()));
    }

    public function testEmptyFirstNameIsRequired(): void
    {
        $errors = $this->validator->validate(TestData::validInput(firstName: ''));

        self::assertSame([ValidationError::FirstNameRequired], $errors);
    }

    public function testFirstNameAtMaxLengthIsValid(): void
    {
        $input = TestData::validInput(firstName: str_repeat('a', EmployeeValidator::NAME_MAX_LENGTH));

        self::assertSame([], $this->validator->validate($input));
    }

    public function testFirstNameOverMaxLengthIsTooLong(): void
    {
        $input = TestData::validInput(firstName: str_repeat('a', EmployeeValidator::NAME_MAX_LENGTH + 1));

        self::assertSame([ValidationError::FirstNameTooLong], $this->validator->validate($input));
    }

    public function testNameLengthCountsCharactersNotBytes(): void
    {
        // "ü" takes two bytes in UTF-8, but VARCHAR(100) counts characters.
        $input = TestData::validInput(lastName: str_repeat('ü', EmployeeValidator::NAME_MAX_LENGTH));

        self::assertSame([], $this->validator->validate($input));
    }

    public function testEmptyLastNameIsRequired(): void
    {
        $errors = $this->validator->validate(TestData::validInput(lastName: ''));

        self::assertSame([ValidationError::LastNameRequired], $errors);
    }

    public function testLastNameOverMaxLengthIsTooLong(): void
    {
        $input = TestData::validInput(lastName: str_repeat('a', EmployeeValidator::NAME_MAX_LENGTH + 1));

        self::assertSame([ValidationError::LastNameTooLong], $this->validator->validate($input));
    }

    public function testEmptyEmailIsRequired(): void
    {
        $errors = $this->validator->validate(TestData::validInput(email: ''));

        self::assertSame([ValidationError::EmailRequired], $errors);
    }

    #[TestWith(['a@b.c'])]
    #[TestWith(['max.mustermann@example.com'])]
    #[TestWith(['katharina.meier-huber@firma.co.at'])]
    public function testWellFormedEmailIsValid(string $email): void
    {
        self::assertSame([], $this->validator->validate(TestData::validInput(email: $email)));
    }

    #[TestWith(['anna'])]
    #[TestWith(['anna@example'])]
    #[TestWith(['@example.com'])]
    #[TestWith(['anna@@example.com'])]
    #[TestWith(['anna müller@example.com'])]
    public function testMalformedEmailIsInvalid(string $email): void
    {
        $errors = $this->validator->validate(TestData::validInput(email: $email));

        self::assertSame([ValidationError::EmailInvalid], $errors);
    }

    public function testEmailAtMaxLengthIsValid(): void
    {
        $domain = '@example.com';
        $email = str_repeat('a', EmployeeValidator::EMAIL_MAX_LENGTH - strlen($domain)) . $domain;

        self::assertSame([], $this->validator->validate(TestData::validInput(email: $email)));
    }

    public function testEmailOverMaxLengthIsTooLong(): void
    {
        $domain = '@example.com';
        $email = str_repeat('a', EmployeeValidator::EMAIL_MAX_LENGTH - strlen($domain) + 1) . $domain;

        self::assertSame([ValidationError::EmailTooLong], $this->validator->validate(TestData::validInput(email: $email)));
    }

    #[TestWith([null])]
    #[TestWith([0])]
    #[TestWith([-1])]
    public function testMissingDepartmentIsRequired(?int $departmentId): void
    {
        $errors = $this->validator->validate(TestData::validInput(departmentId: $departmentId));

        self::assertSame([ValidationError::DepartmentRequired], $errors);
    }

    public function testEmptyHireDateIsRequired(): void
    {
        $errors = $this->validator->validate(TestData::validInput(hireDate: ''));

        self::assertSame([ValidationError::HireDateRequired], $errors);
    }

    #[TestWith(['05.10.2026'])]
    #[TestWith(['2026-02-30'])]
    #[TestWith(['2026-13-01'])]
    #[TestWith(['yesterday'])]
    public function testMalformedHireDateIsInvalid(string $hireDate): void
    {
        $errors = $this->validator->validate(TestData::validInput(hireDate: $hireDate));

        self::assertSame([ValidationError::HireDateInvalid], $errors);
    }

    public function testHireDateOnEarliestDateIsValid(): void
    {
        $input = TestData::validInput(hireDate: EmployeeValidator::EARLIEST_HIRE_DATE);

        self::assertSame([], $this->validator->validate($input));
    }

    public function testHireDateBeforeEarliestDateIsTooEarly(): void
    {
        $errors = $this->validator->validate(TestData::validInput(hireDate: '1949-12-31'));

        self::assertSame([ValidationError::HireDateTooEarly], $errors);
    }

    public function testHireDateOneYearAheadIsValid(): void
    {
        self::assertSame([], $this->validator->validate(TestData::validInput(hireDate: '2027-10-05')));
    }

    public function testHireDateMoreThanOneYearAheadIsTooFarInFuture(): void
    {
        $errors = $this->validator->validate(TestData::validInput(hireDate: '2027-10-06'));

        self::assertSame([ValidationError::HireDateTooFarInFuture], $errors);
    }

    /**
     * @return iterable<string, array{string, string}>
     */
    public static function leapDayLimits(): iterable
    {
        yield 'next year has no 29 February' => ['2028-02-29', '2029-02-28'];
        yield 'regular day' => ['2026-10-05', '2027-10-05'];
    }

    #[DataProvider('leapDayLimits')]
    public function testLatestHireDateIsOneYearAheadClampedToMonthEnd(string $today, string $expected): void
    {
        $validator = new EmployeeValidator(new FixedClock(new DateTimeImmutable($today . ' 09:00:00')));

        self::assertSame($expected, $validator->latestHireDate()->format('Y-m-d'));
    }

    public function testSeveralInvalidFieldsReturnAllErrors(): void
    {
        $input = TestData::validInput(firstName: '', lastName: '', email: 'x', departmentId: null, hireDate: '');

        self::assertSame(
            [
                ValidationError::FirstNameRequired,
                ValidationError::LastNameRequired,
                ValidationError::EmailInvalid,
                ValidationError::DepartmentRequired,
                ValidationError::HireDateRequired,
            ],
            $this->validator->validate($input),
        );
    }
}
