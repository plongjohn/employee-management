<?php

// Works as is after database/setup.sql. The password is a development password for the
// local demo only (see docs/decisions/0008-database-application-user.md).

return [
    'database' => [
        'host' => 'localhost',
        'port' => 3306,
        'name' => 'employee_management',
        'user' => 'employee_app',
        'password' => 'employee_app_dev',
    ],

    // Decides what "today" is for the hire date validation.
    'timezone' => 'Europe/Vienna',

    // debug, info, notice, warning, error, critical, alert or emergency
    'logLevel' => 'info',

    // Shows error details in the browser. Never enable it on a public server.
    'debug' => false,
];
