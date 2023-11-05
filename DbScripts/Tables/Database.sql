IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'eJournalDb')
CREATE DATABASE eJournalDb;
GO