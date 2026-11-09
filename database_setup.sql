-- PROG6221 Part 3 - Cybersecurity Awareness Assistant
-- Run this script in MySQL Workbench before starting the application.

CREATE DATABASE IF NOT EXISTS CyberSecurityBotDB;
USE CyberSecurityBotDB;

CREATE TABLE IF NOT EXISTS cyber_tasks
(
    task_id       INT AUTO_INCREMENT PRIMARY KEY,
    title         VARCHAR(150) NOT NULL,
    description   VARCHAR(500) NOT NULL,
    reminder_date DATETIME NULL,
    is_completed  BOOLEAN NOT NULL DEFAULT FALSE,
    created_at    TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Quick check (the result can be empty at first):
SELECT * FROM cyber_tasks;
