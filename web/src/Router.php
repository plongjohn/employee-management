<?php

declare(strict_types=1);

namespace EmployeeManagement;

/**
 * Maps method and path to a controller action. Path patterns may contain parameters with a
 * regular expression, e.g. "/employees/{id:\d+}/edit".
 */
final class Router
{
    /** @var list<array{method: string, regex: string, handler: array{class-string, string}}> */
    private array $routes = [];

    /**
     * @param array{class-string, string} $handler Controller class and method name.
     */
    public function get(string $pattern, array $handler): void
    {
        $this->add('GET', $pattern, $handler);
    }

    /**
     * @param array{class-string, string} $handler Controller class and method name.
     */
    public function post(string $pattern, array $handler): void
    {
        $this->add('POST', $pattern, $handler);
    }

    public function match(string $method, string $path): ?RouteMatch
    {
        foreach ($this->routes as $route) {
            if ($route['method'] === $method && preg_match($route['regex'], $path, $matches) === 1) {
                $parameters = array_filter($matches, is_string(...), ARRAY_FILTER_USE_KEY);

                return new RouteMatch($route['handler'], $parameters);
            }
        }

        return null;
    }

    /**
     * @param array{class-string, string} $handler
     */
    private function add(string $method, string $pattern, array $handler): void
    {
        $this->routes[] = ['method' => $method, 'regex' => $this->toRegex($pattern), 'handler' => $handler];
    }

    private function toRegex(string $pattern): string
    {
        $regex = preg_replace_callback(
            '/\{(\w+):([^}]+)\}/',
            static fn (array $parameter): string => "(?P<{$parameter[1]}>{$parameter[2]})",
            $pattern,
        );

        return '#^' . $regex . '$#';
    }
}
