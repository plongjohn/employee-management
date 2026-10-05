<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\Http;

use EmployeeManagement\Http\CsrfTokenManager;
use EmployeeManagement\Tests\Fakes\ArraySessionStorage;
use PHPUnit\Framework\TestCase;

final class CsrfTokenManagerTest extends TestCase
{
    private CsrfTokenManager $manager;

    protected function setUp(): void
    {
        $this->manager = new CsrfTokenManager(new ArraySessionStorage());
    }

    public function testTokenStaysTheSameWithinASession(): void
    {
        self::assertSame($this->manager->token(), $this->manager->token());
    }

    public function testTokenIsLongAndRandom(): void
    {
        $otherSession = new CsrfTokenManager(new ArraySessionStorage());

        self::assertSame(64, strlen($this->manager->token()));
        self::assertNotSame($this->manager->token(), $otherSession->token());
    }

    public function testIssuedTokenIsValid(): void
    {
        self::assertTrue($this->manager->isValid($this->manager->token()));
    }

    public function testOtherTokenIsInvalid(): void
    {
        $this->manager->token();

        self::assertFalse($this->manager->isValid(str_repeat('0', 64)));
    }

    public function testEmptyTokenIsInvalidEvenWithoutSessionToken(): void
    {
        self::assertFalse($this->manager->isValid(''));
    }
}
