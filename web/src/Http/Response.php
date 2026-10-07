<?php

declare(strict_types=1);

namespace EmployeeManagement\Http;

final readonly class Response
{
    /**
     * @param array<string, string> $headers
     */
    public function __construct(
        public int $statusCode,
        public string $body = '',
        public array $headers = [],
    ) {
    }

    public static function html(string $body, int $statusCode = 200): self
    {
        return new self($statusCode, $body, ['Content-Type' => 'text/html; charset=UTF-8']);
    }

    /**
     * 303 See Other makes the browser follow with GET, so reloading the next page never
     * submits a form a second time (Post/Redirect/Get).
     */
    public static function redirect(string $location): self
    {
        return new self(303, '', ['Location' => $location]);
    }

    public function send(): void
    {
        http_response_code($this->statusCode);
        foreach ($this->headers as $name => $value) {
            header("{$name}: {$value}");
        }

        echo $this->body;
    }
}
