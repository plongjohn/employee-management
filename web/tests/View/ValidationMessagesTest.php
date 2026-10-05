<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\View;

use EmployeeManagement\Services\ValidationError;
use EmployeeManagement\View\Translator;
use EmployeeManagement\View\ValidationMessages;
use PHPUnit\Framework\Attributes\DataProvider;
use PHPUnit\Framework\TestCase;

final class ValidationMessagesTest extends TestCase
{
    private ValidationMessages $messages;

    protected function setUp(): void
    {
        $this->messages = new ValidationMessages(Translator::fromFile(dirname(__DIR__, 2) . '/lang/de.php'));
    }

    /**
     * @return iterable<string, array{ValidationError}>
     */
    public static function allErrors(): iterable
    {
        foreach (ValidationError::cases() as $error) {
            yield $error->name => [$error];
        }
    }

    #[DataProvider('allErrors')]
    public function testEveryErrorHasATextWithoutOpenPlaceholders(ValidationError $error): void
    {
        $text = $this->messages->for($error);

        self::assertNotSame('Validation' . $error->name, $text, 'Text is missing in lang/de.php.');
        self::assertStringNotContainsString('{', $text);
    }

    public function testTextsContainTheLimits(): void
    {
        self::assertStringContainsString('100', $this->messages->for(ValidationError::FirstNameTooLong));
        self::assertStringContainsString('01.01.1950', $this->messages->for(ValidationError::HireDateTooEarly));
    }

    public function testByFieldKeysMessagesByFormField(): void
    {
        $byField = $this->messages->byField([ValidationError::EmailInvalid, ValidationError::HireDateRequired]);

        self::assertSame(['email', 'hireDate'], array_keys($byField));
    }
}
