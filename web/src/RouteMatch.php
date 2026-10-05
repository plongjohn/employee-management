<?php

declare(strict_types=1);

namespace EmployeeManagement;

final readonly class RouteMatch
{
    /**
     * @param array{class-string, string} $handler
     * @param array<string, string> $parameters
     */
    public function __construct(
        public array $handler,
        public array $parameters,
    ) {
    }
}
