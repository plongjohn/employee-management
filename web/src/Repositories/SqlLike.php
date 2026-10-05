<?php

declare(strict_types=1);

namespace EmployeeManagement\Repositories;

final class SqlLike
{
    /**
     * Builds a LIKE pattern that matches values starting with the term.
     * Wildcards typed by the user are escaped, so "50%" matches literally.
     * A prefix pattern (no leading wildcard) lets MariaDB use the column index.
     */
    public static function prefix(string $term): string
    {
        // Backslash is MariaDB's default LIKE escape character, so it has to be escaped first.
        return strtr($term, ['\\' => '\\\\', '%' => '\\%', '_' => '\\_']) . '%';
    }
}
