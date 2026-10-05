<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\Http;

use EmployeeManagement\Http\FlashMessages;
use EmployeeManagement\Http\FlashType;
use EmployeeManagement\Tests\Fakes\ArraySessionStorage;
use PHPUnit\Framework\TestCase;

final class FlashMessagesTest extends TestCase
{
    public function testConsumeReturnsMessagesInOrderOnlyOnce(): void
    {
        $messages = new FlashMessages(new ArraySessionStorage());
        $messages->add(FlashType::Success, 'StatusDeleted');
        $messages->add(FlashType::Warning, 'EmployeeNotFound');

        self::assertSame(
            [
                ['type' => 'success', 'textKey' => 'StatusDeleted'],
                ['type' => 'warning', 'textKey' => 'EmployeeNotFound'],
            ],
            $messages->consume(),
        );
        self::assertSame([], $messages->consume());
    }
}
