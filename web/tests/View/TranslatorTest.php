<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\View;

use EmployeeManagement\View\Translator;
use PHPUnit\Framework\TestCase;

final class TranslatorTest extends TestCase
{
    public function testReplacesNamedPlaceholders(): void
    {
        $translator = new Translator(['PageOf' => 'Seite {page} von {pages}']);

        self::assertSame('Seite 2 von 5', $translator->translate('PageOf', ['page' => 2, 'pages' => 5]));
    }

    public function testUnknownKeyIsReturnedUnchanged(): void
    {
        self::assertSame('Missing', (new Translator([]))->translate('Missing'));
    }
}
