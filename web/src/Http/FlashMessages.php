<?php

declare(strict_types=1);

namespace EmployeeManagement\Http;

/**
 * Messages that survive exactly one redirect, e.g. "Employee was deleted" after Post/Redirect/Get.
 */
final readonly class FlashMessages
{
    private const string SESSION_KEY = 'flash_messages';

    public function __construct(
        private SessionStorage $session,
    ) {
    }

    public function add(FlashType $type, string $textKey): void
    {
        $messages = $this->stored();
        $messages[] = ['type' => $type->value, 'textKey' => $textKey];
        $this->session->set(self::SESSION_KEY, $messages);
    }

    /**
     * Returns the messages and removes them, so they are shown only once.
     *
     * @return list<array{type: string, textKey: string}>
     */
    public function consume(): array
    {
        $messages = $this->stored();
        $this->session->remove(self::SESSION_KEY);

        return $messages;
    }

    /**
     * @return list<array{type: string, textKey: string}>
     */
    private function stored(): array
    {
        $messages = $this->session->get(self::SESSION_KEY);

        /** @var list<array{type: string, textKey: string}> */
        return is_array($messages) ? $messages : [];
    }
}
