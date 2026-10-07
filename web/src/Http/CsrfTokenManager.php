<?php

declare(strict_types=1);

namespace EmployeeManagement\Http;

/**
 * Synchronizer token against cross-site request forgery: every form posts the token stored
 * in the session, which a foreign site cannot read.
 */
final readonly class CsrfTokenManager
{
    public const string FIELD_NAME = '_csrf';

    private const string SESSION_KEY = 'csrf_token';

    // 32 random bytes = 256 bits, far beyond guessing.
    private const int TOKEN_BYTES = 32;

    public function __construct(
        private SessionStorage $session,
    ) {
    }

    public function token(): string
    {
        $token = $this->session->get(self::SESSION_KEY);
        if (is_string($token)) {
            return $token;
        }

        $token = bin2hex(random_bytes(self::TOKEN_BYTES));
        $this->session->set(self::SESSION_KEY, $token);

        return $token;
    }

    public function isValid(string $submittedToken): bool
    {
        $token = $this->session->get(self::SESSION_KEY);

        // hash_equals takes the same time for every input, so the token cannot be guessed
        // character by character from response times.
        return is_string($token) && $submittedToken !== '' && hash_equals($token, $submittedToken);
    }
}
