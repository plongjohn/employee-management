<?php

declare(strict_types=1);

namespace EmployeeManagement\Http;

/**
 * Values match the Bootstrap colour names used for the message.
 */
enum FlashType: string
{
    case Success = 'success';
    case Warning = 'warning';
}
