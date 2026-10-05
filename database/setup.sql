-- Employee Management – database setup with sample data.
-- Rerunnable: drops and recreates both tables, so existing data is lost.

-- The file is UTF-8; without this, Windows clients may read umlauts in the sample data
-- with their console code page and store garbled names.
SET NAMES utf8mb4;

CREATE DATABASE IF NOT EXISTS employee_management
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_uca1400_ai_ci;

USE employee_management;

-- employees references departments, so it has to go first.
DROP TABLE IF EXISTS employees;
DROP TABLE IF EXISTS departments;

CREATE TABLE departments (
  id   INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(100) NOT NULL,
  CONSTRAINT uq_departments_name UNIQUE (name),
  CONSTRAINT chk_departments_name_not_blank CHECK (TRIM(name) <> '')
) ENGINE = InnoDB;

CREATE TABLE employees (
  id            INT AUTO_INCREMENT PRIMARY KEY,
  first_name    VARCHAR(100) NOT NULL,
  last_name     VARCHAR(100) NOT NULL,
  email         VARCHAR(255) NOT NULL,
  department_id INT NOT NULL,
  hire_date     DATE NOT NULL,
  -- Optimistic concurrency: the apps increment it on every update and only write
  -- when it still matches the loaded value (see docs/decisions/0003).
  version       INT UNSIGNED NOT NULL DEFAULT 1,
  created_at    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT uq_employees_email UNIQUE (email),
  CONSTRAINT fk_employees_department FOREIGN KEY (department_id)
    REFERENCES departments (id) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT chk_employees_first_name_not_blank CHECK (TRIM(first_name) <> ''),
  CONSTRAINT chk_employees_last_name_not_blank CHECK (TRIM(last_name) <> ''),
  CONSTRAINT chk_employees_email_not_blank CHECK (TRIM(email) <> ''),
  -- Indexes serve prefix search (LIKE 'abc%') and the default sort by name.
  -- department_name also covers the foreign key and the department filter.
  INDEX idx_employees_last_first (last_name, first_name),
  INDEX idx_employees_first_name (first_name),
  INDEX idx_employees_department_name (department_id, last_name, first_name)
) ENGINE = InnoDB;

-- Fixed IDs keep the employee rows below readable.
-- Finanzen intentionally has no employees, to show the empty state of the department filter.
INSERT INTO departments (id, name) VALUES
  (1, 'Produktion'),
  (2, 'Qualitätssicherung'),
  (3, 'Logistik'),
  (4, 'Einkauf'),
  (5, 'Entwicklung'),
  (6, 'IT'),
  (7, 'Personal'),
  (8, 'Finanzen');

INSERT INTO employees (first_name, last_name, email, department_id, hire_date) VALUES
  ('Anna',        'Müller',                   'anna.mueller@example.com',                        1, '2012-04-01'),
  ('Lukas',       'Müller',                   'lukas.mueller@example.com',                       3, '2018-11-05'),
  ('Thomas',      'Weiß',                     'thomas.weiss@example.com',                        1, '2015-09-15'),
  ('Sean',        'O''Connor',                'sean.oconnor@example.com',                        5, '2019-03-01'),
  ('Katharina',   'Meier-Huber',              'katharina.meier-huber@example.com',               2, '2016-07-01'),
  ('Maximilian',  'von Hohenberg-Schönfeld',  'maximilian.vonhohenberg-schoenfeld@example.com',  4, '2021-02-01'),
  ('Elif',        'Yılmaz',                   'elif.yilmaz@example.com',                         6, '2023-09-01'),
  ('Sabine',      'Schneider',                'sabine.schneider@example.com',                    7, '2010-01-04'),
  ('Michael',     'Fischer',                  'michael.fischer@example.com',                     1, '2011-06-01'),
  ('Julia',       'Wagner',                   'julia.wagner@example.com',                        6, '2020-10-01'),
  ('Stefan',      'Becker',                   'stefan.becker@example.com',                       5, '2014-08-18'),
  ('Laura',       'Hoffmann',                 'laura.hoffmann@example.com',                      2, '2022-04-01'),
  ('Daniel',      'Schäfer',                  'daniel.schaefer@example.com',                     3, '2013-02-11'),
  ('Markus',      'Koch',                     'markus.koch@example.com',                         1, '2017-05-02'),
  ('Christina',   'Bauer',                    'christina.bauer@example.com',                     4, '2019-12-01'),
  ('Andreas',     'Richter',                  'andreas.richter@example.com',                     5, '2024-01-15'),
  ('Sophie',      'Klein',                    'sophie.klein@example.com',                        7, '2025-03-01'),
  ('Jan',         'Wolf',                     'jan.wolf@example.com',                            3, '2016-10-17'),
  ('Petra',       'Schröder',                 'petra.schroeder@example.com',                     2, '2010-09-01'),
  ('Felix',       'Neumann',                  'felix.neumann@example.com',                       6, '2026-08-03');
