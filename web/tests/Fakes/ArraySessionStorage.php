<?php

declare(strict_types=1);

namespace EmployeeManagement\Tests\Fakes;

use EmployeeManagement\Http\SessionStorage;

final class ArraySessionStorage implements SessionStorage
{
    /** @var array<string, mixed> */
    private array $values = [];

    public function get(string $key): mixed
    {
        return $this->values[$key] ?? null;
    }

    public function set(string $key, mixed $value): void
    {
        $this->values[$key] = $value;
    }

    public function remove(string $key): void
    {
        unset($this->values[$key]);
    }
}
