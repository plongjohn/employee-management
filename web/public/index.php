<?php

declare(strict_types=1);

use EmployeeManagement\Application;
use EmployeeManagement\Config\AppConfig;
use EmployeeManagement\ContainerFactory;
use EmployeeManagement\Http\Request;

// Front controller: every request that is not a static file ends up here.

$rootDirectory = dirname(__DIR__);
require "{$rootDirectory}/vendor/autoload.php";

try {
    $container = ContainerFactory::create($rootDirectory);
    date_default_timezone_set($container->get(AppConfig::class)->timezone());

    $container->get(Application::class)->handle(Request::fromGlobals())->send();
} catch (Throwable $exception) {
    // Application handles its own errors; this is only reached when it cannot be built at all,
    // e.g. because the log folder is not writable. PHP's error log is the only place left.
    error_log((string) $exception);
    http_response_code(500);
    header('Content-Type: text/plain; charset=UTF-8');
    $texts = require "{$rootDirectory}/lang/de.php";
    echo $texts['ErrorUnexpected'];
}
