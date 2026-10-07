<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\Models;

use EmployeeManagement\Models\EmployeeInput;
use PHPUnit\Framework\TestCase;

final class EmployeeInputTest extends TestCase
{
    public function testFromFormReadsAllFields(): void
    {
        $input = EmployeeInput::fromForm([
            'firstName' => 'Anna',
            'lastName' => 'Müller',
            'email' => 'anna.mueller@example.com',
            'departmentId' => '3',
            'hireDate' => '2012-04-01',
        ]);

        self::assertEquals(new EmployeeInput('Anna', 'Müller', 'anna.mueller@example.com', 3, '2012-04-01'), $input);
    }

    public function testFromFormTreatsMissingOrMalformedValuesAsEmpty(): void
    {
        $input = EmployeeInput::fromForm(['firstName' => ['array'], 'departmentId' => 'abc']);

        self::assertEquals(new EmployeeInput('', '', '', null, ''), $input);
    }

    public function testTrimmedRemovesUnicodeWhitespace(): void
    {
        $input = new EmployeeInput(" Anna\u{00A0}", "\tMüller", ' a@b.c ', 1, ' 2012-04-01 ');

        self::assertEquals(new EmployeeInput('Anna', 'Müller', 'a@b.c', 1, '2012-04-01'), $input->trimmed());
    }

    public function testParsedHireDateReturnsDateAtMidnight(): void
    {
        $input = new EmployeeInput('', '', '', null, '2012-04-01');

        self::assertSame('2012-04-01 00:00:00', $input->parsedHireDate()?->format('Y-m-d H:i:s'));
    }

    public function testParsedHireDateRejectsImpossibleDate(): void
    {
        self::assertNull((new EmployeeInput('', '', '', null, '2026-02-30'))->parsedHireDate());
    }

    public function testToFormIsTheOppositeOfFromForm(): void
    {
        $input = new EmployeeInput('Anna', 'Müller', 'anna.mueller@example.com', 3, '2012-04-01');

        self::assertEquals($input, EmployeeInput::fromForm($input->toForm()));
    }
}
