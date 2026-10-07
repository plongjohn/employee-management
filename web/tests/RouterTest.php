<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests;

use EmployeeManagement\Controllers\EmployeeController;
use EmployeeManagement\Router;
use PHPUnit\Framework\TestCase;

final class RouterTest extends TestCase
{
    private Router $router;

    protected function setUp(): void
    {
        $this->router = new Router();
        $this->router->get('/employees', [EmployeeController::class, 'index']);
        $this->router->get('/employees/{id:\d+}/edit', [EmployeeController::class, 'edit']);
        $this->router->post('/employees/{id:\d+}', [EmployeeController::class, 'update']);
    }

    public function testMatchesStaticPath(): void
    {
        $match = $this->router->match('GET', '/employees');

        self::assertSame([EmployeeController::class, 'index'], $match?->handler);
        self::assertSame([], $match->parameters);
    }

    public function testMatchesPathWithParameter(): void
    {
        $match = $this->router->match('GET', '/employees/42/edit');

        self::assertSame([EmployeeController::class, 'edit'], $match?->handler);
        self::assertSame(['id' => '42'], $match->parameters);
    }

    public function testParameterMustMatchItsPattern(): void
    {
        self::assertNull($this->router->match('GET', '/employees/abc/edit'));
    }

    public function testMethodMustMatch(): void
    {
        self::assertNull($this->router->match('POST', '/employees/42/edit'));
        self::assertNotNull($this->router->match('POST', '/employees/42'));
    }

    public function testWholePathMustMatch(): void
    {
        self::assertNull($this->router->match('GET', '/employees/42/edit/more'));
        self::assertNull($this->router->match('GET', '/prefix/employees'));
    }
}
