<?php

declare(strict_types=1);

namespace EmployeeManagement\View;

use EmployeeManagement\Services\EmployeeValidator;
use EmployeeManagement\Services\ValidationError;

final readonly class ValidationMessages
{
    public function __construct(
        private Translator $translator,
    ) {
    }

    public function for(ValidationError $error): string
    {
        return $this->translator->translate('Validation' . $error->name, $this->parametersFor($error));
    }

    /**
     * The first message of each field; the validator reports at most one error per field anyway.
     *
     * @param list<ValidationError> $errors
     * @return array<string, string> Field name => message.
     */
    public function byField(array $errors): array
    {
        $messages = [];
        foreach ($errors as $error) {
            $messages[$error->field()] ??= $this->for($error);
        }

        return $messages;
    }

    /**
     * @return array<string, string|int>
     */
    private function parametersFor(ValidationError $error): array
    {
        return match ($error) {
            ValidationError::FirstNameTooLong, ValidationError::LastNameTooLong => [
                'max' => EmployeeValidator::NAME_MAX_LENGTH,
            ],
            ValidationError::EmailTooLong => ['max' => EmployeeValidator::EMAIL_MAX_LENGTH],
            ValidationError::HireDateTooEarly => [
                'date' => EmployeeValidator::earliestHireDate()->format($this->translator->translate('DateFormat')),
            ],
            default => [],
        };
    }
}
