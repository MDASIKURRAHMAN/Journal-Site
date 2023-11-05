use eJournalDb;

--Users Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' and xtype='U')
CREATE TABLE "Users"(
    "UserId" BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    "UserEmail" NVARCHAR(100) NOT NULL,
    "FirstName" NVARCHAR(100) NULL,
    "LastName" NVARCHAR(100) NULL,
    "Gender" NVARCHAR(10) NULL,
    "DateOfBirth" DATETIME NULL,
	"Department" NVARCHAR(100) NULL,
    "Designation" NVARCHAR(100) NULL,
    "Phone" NVARCHAR(20) NULL,
    "NickName" NVARCHAR(100) NULL,
    "Bio" NVARCHAR(4000) NULL,
    "ImageId" BIGINT NULL,
    "IsActive" BIT NOT NULL,
    "CreatedAt" DATETIME NOT NULL,
    "UpdatedAt" DATETIME NULL
);