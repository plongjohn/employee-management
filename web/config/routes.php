<?php

declare(strict_types=1);

use EmployeeManagement\Controllers\EmployeeController;
use EmployeeManagement\Router;

return static function (Router $router): void {
    $router->get('/', [EmployeeController::class, 'home']);
    $router->get('/employees', [EmployeeController::class, 'index']);
    $router->get('/employees/new', [EmployeeController::class, 'create']);
    $router->post('/employees', [EmployeeController::class, 'store']);
    $router->get('/employees/{id:\d+}/edit', [EmployeeController::class, 'edit']);
    $router->post('/employees/{id:\d+}', [EmployeeController::class, 'update']);
    $router->post('/employees/{id:\d+}/delete', [EmployeeController::class, 'delete']);
};
