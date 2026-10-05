<?php

declare(strict_types=1);

namespace EmployeeManagement\Config;

use Monolog\Level;

/**
 * Settings from config/config.php. A missing file or database section does not stop the
 * application from starting: the error is reported on the first database access, when the
 * logger and the error page are already available.
 */
final readonly class AppConfig
{
    private const string DEFAULT_TIMEZONE = 'Europe/Vienna';

    /**
     * @param array<string, mixed> $values
     */
    public function __construct(
        private array $values,
    ) {
    }

    public static function fromFile(string $path): self
    {
        if (!is_file($path)) {
            return new self([]);
        }

        $values = require $path;

        return new self(is_array($values) ? $values : []);
    }

    public function database(): DatabaseConfig
    {
        $database = $this->values['database'] ?? null;
        if (!is_array($database)) {
            throw new ConfigurationException('The database connection is not configured (config/config.php).');
        }

        return new DatabaseConfig(
            host: $this->requireString($database, 'host'),
            port: is_int($database['port'] ?? null) ? $database['port'] : 3306,
            name: $this->requireString($database, 'name'),
            user: $this->requireString($database, 'user'),
            password: $this->requireString($database, 'password'),
        );
    }

    public function timezone(): string
    {
        $timezone = $this->values['timezone'] ?? null;

        return is_string($timezone) && in_array($timezone, timezone_identifiers_list(), true)
            ? $timezone
            : self::DEFAULT_TIMEZONE;
    }

    // An unknown level falls back to info instead of failing: the logger is needed to report
    // every other configuration error.
    public function logLevel(): Level
    {
        $level = $this->values['logLevel'] ?? null;
        foreach (Level::cases() as $known) {
            if (is_string($level) && strcasecmp($known->name, $level) === 0) {
                return $known;
            }
        }

        return Level::Info;
    }

    public function debug(): bool
    {
        return ($this->values['debug'] ?? false) === true;
    }

    /**
     * @param array<mixed> $section
     */
    private function requireString(array $section, string $key): string
    {
        $value = $section[$key] ?? null;
        if (!is_string($value) || $value === '') {
            throw new ConfigurationException("The database setting '{$key}' is missing (config/config.php).");
        }

        return $value;
    }
}
