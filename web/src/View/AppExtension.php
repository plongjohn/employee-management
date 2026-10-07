<?php

declare(strict_types=1);

namespace EmployeeManagement\View;

use EmployeeManagement\Http\CsrfTokenManager;
use EmployeeManagement\Http\FlashMessages;
use EmployeeManagement\Models\Employee;
use EmployeeManagement\Models\EmployeeQuery;
use Twig\Extension\AbstractExtension;
use Twig\TwigFilter;
use Twig\TwigFunction;

/**
 * Template helpers: texts, CSRF field, list URLs and the presentation of employees.
 */
final class AppExtension extends AbstractExtension
{
    // Number of badge colours in app.css (.badge-department-0 … -7); further departments reuse them.
    private const int BADGE_COLOR_COUNT = 8;

    public function __construct(
        private readonly Translator $translator,
        private readonly CsrfTokenManager $csrfTokenManager,
        private readonly FlashMessages $flashMessages,
    ) {
    }

    public function getFunctions(): array
    {
        return [
            new TwigFunction('t', $this->translator->translate(...)),
            new TwigFunction('csrf_field', $this->csrfField(...), ['is_safe' => ['html']]),
            new TwigFunction('flash_messages', $this->flashMessages->consume(...)),
            new TwigFunction('list_url', self::listUrl(...)),
        ];
    }

    public function getFilters(): array
    {
        return [
            new TwigFilter('initials', self::initials(...)),
            new TwigFilter('badge_class', self::badgeClass(...)),
        ];
    }

    public static function listUrl(EmployeeQuery $query): string
    {
        $parameters = $query->toParameters();

        return '/employees' . ($parameters === [] ? '' : '?' . http_build_query($parameters));
    }

    public static function initials(Employee $employee): string
    {
        return mb_strtoupper(mb_substr($employee->firstName, 0, 1) . mb_substr($employee->lastName, 0, 1));
    }

    public static function badgeClass(int $departmentId): string
    {
        return 'badge-department-' . ($departmentId % self::BADGE_COLOR_COUNT);
    }

    private function csrfField(): string
    {
        return sprintf(
            '<input type="hidden" name="%s" value="%s">',
            CsrfTokenManager::FIELD_NAME,
            htmlspecialchars($this->csrfTokenManager->token(), ENT_QUOTES),
        );
    }
}
