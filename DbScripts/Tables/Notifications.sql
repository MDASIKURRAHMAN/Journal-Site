use eJournalDb;

--Notifications Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notifications' and xtype='U')
CREATE TABLE "Notifications"(
    "NotificationId" BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    "BlogId" BIGINT NOT NULL,
    "UserId" BIGINT NOT NULL,
    "NotificationText" NVARCHAR(200) NOT NULL,
    "IsChecked" BIT NOT NULL,
    "CreateAt" DATETIME NOT NULL
);
