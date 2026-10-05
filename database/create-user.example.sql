-- Template for the restricted application user.
-- Copy to create-user.sql (git-ignored), replace CHANGE_ME and run it as root
-- after setup.sql. Use the same password in the desktop and web configuration.

CREATE USER IF NOT EXISTS 'employee_app'@'localhost' IDENTIFIED BY 'CHANGE_ME';

-- Departments are a fixed list maintained by setup.sql, so the apps only read them.
GRANT SELECT, INSERT, UPDATE, DELETE ON employee_management.employees TO 'employee_app'@'localhost';
GRANT SELECT ON employee_management.departments TO 'employee_app'@'localhost';
