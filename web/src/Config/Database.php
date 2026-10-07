<?php

declare(strict_types=1);

namespace EmployeeManagement\Config;

use PDO;

final class Database
{
    public static function connect(DatabaseConfig $config): PDO
    {
        return new PDO($config->dsn(), $config->user, $config->password, [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
            // Real prepared statements: the server receives SQL and values separately.
            PDO::ATTR_EMULATE_PREPARES => false,
        ]);
    }
}
