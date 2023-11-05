use eJournalDb;

--Blogs Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Blogs' and xtype='U')
CREATE TABLE "Blogs"(
    "BlogId" BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    "BlogTitle" NVARCHAR(500) NOT NULL,
    "BlogText" NVARCHAR(max) NOT NULL,
    "UserId" BIGINT NOT NULL,
    "CreatedAt" DATETIME NOT NULL,
    "UpdatedAt" DATETIME NULL
);