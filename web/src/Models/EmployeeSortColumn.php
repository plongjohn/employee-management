<?php

declare(strict_types=1);

namespace EmployeeManagement\Models;

enum EmployeeSortColumn: string
{
    case Name = 'name';
    case Email = 'email';
    case Department = 'department';
    case HireDate = 'hireDate';
}
