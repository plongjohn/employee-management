<?php

// Copy to config.php (git-ignored) and replace CHANGE_ME with the password of employee_app.

return [
    'database' => [
        'host' => 'localhost',
        'port' => 3306,
        'name' => 'employee_management',
        'user' => 'employee_app',
        'password' => 'CHANGE_ME',
    ],

    // Decides what "today" is for the hire date validation.
    'timezone' => 'Europe/Vienna',

    // debug, info, notice, warning, error, critical, alert or emergency
    'logLevel' => 'info',

    // Shows error details in the browser. Never enable it on a public server.
    'debug' => false,
];
