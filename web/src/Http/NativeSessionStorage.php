<?php

declare(strict_types=1);

namespace EmployeeManagement\Http;

/**
 * PHP's own session, started on first use, so requests that need no session (e.g. assets
 * or error pages) do not create one.
 */
final class NativeSessionStorage implements SessionStorage
{
    private const string COOKIE_NAME = 'employee_management_session';

    public function get(string $key): mixed
    {
        $this->start();

        return $_SESSION[$key] ?? null;
    }

    public function set(string $key, mixed $value): void
    {
        $this->start();
        $_SESSION[$key] = $value;
    }

    public function remove(string $key): void
    {
        $this->start();
        unset($_SESSION[$key]);
    }

    private function start(): void
    {
        if (session_status() === PHP_SESSION_ACTIVE) {
            return;
        }

        session_name(self::COOKIE_NAME);
        session_start([
            'cookie_httponly' => true,
            'cookie_samesite' => 'Lax',
            'cookie_secure' => ($_SERVER['HTTPS'] ?? 'off') !== 'off',
            'use_strict_mode' => true,
        ]);
    }
}
