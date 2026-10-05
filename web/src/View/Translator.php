<?php

declare(strict_types=1);

namespace EmployeeManagement\View;

final readonly class Translator
{
    /**
     * @param array<string, string> $texts
     */
    public function __construct(
        private array $texts,
    ) {
    }

    public static function fromFile(string $path): self
    {
        /** @var array<string, string> $texts */
        $texts = require $path;

        return new self($texts);
    }

    /**
     * Replaces placeholders like {count}. An unknown key is returned unchanged, so a missing
     * text shows up on the page instead of breaking it.
     *
     * @param array<string, string|int> $parameters
     */
    public function translate(string $key, array $parameters = []): string
    {
        $text = $this->texts[$key] ?? $key;
        $replacements = [];
        foreach ($parameters as $name => $value) {
            $replacements['{' . $name . '}'] = (string) $value;
        }

        return strtr($text, $replacements);
    }
}
