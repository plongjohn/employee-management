<?php

declare(strict_types=1);

namespace EmployeeManagement\Http;

/**
 * The parts of an HTTP request the application needs. Controllers read from this object
 * instead of the superglobals, which keeps them independent of the PHP runtime.
 */
final readonly class Request
{
    /**
     * @param array<string, mixed> $query
     * @param array<string, mixed> $form
     * @param array<string, string> $routeParameters
     */
    public function __construct(
        public string $method,
        public string $path,
        public array $query = [],
        public array $form = [],
        public array $routeParameters = [],
    ) {
    }

    public static function fromGlobals(): self
    {
        $path = parse_url($_SERVER['REQUEST_URI'] ?? '/', PHP_URL_PATH);

        /** @var array<string, mixed> $query */
        $query = $_GET;
        /** @var array<string, mixed> $form */
        $form = $_POST;

        return new self(
            strtoupper(is_string($_SERVER['REQUEST_METHOD'] ?? null) ? $_SERVER['REQUEST_METHOD'] : 'GET'),
            is_string($path) && $path !== '' ? $path : '/',
            $query,
            $form,
        );
    }

    /**
     * @param array<string, string> $routeParameters
     */
    public function withRouteParameters(array $routeParameters): self
    {
        return new self($this->method, $this->path, $this->query, $this->form, $routeParameters);
    }

    public function isPost(): bool
    {
        return $this->method === 'POST';
    }

    /**
     * Arrays (e.g. ?q[]=x) are treated as missing, so callers always get a string.
     */
    public function formValue(string $name): string
    {
        $value = $this->form[$name] ?? '';

        return is_string($value) ? $value : '';
    }

    public function formInt(string $name): ?int
    {
        $value = filter_var($this->formValue($name), FILTER_VALIDATE_INT);

        return $value === false ? null : $value;
    }

    /**
     * Route parameters are already checked against the route pattern, e.g. {id:\d+}.
     */
    public function routeInt(string $name): int
    {
        return (int) ($this->routeParameters[$name] ?? 0);
    }
}
