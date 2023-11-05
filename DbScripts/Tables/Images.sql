use eJournalDb;

--Images Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Images' and xtype='U')
CREATE TABLE "Images"(
    "ImageId" BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    "ImageName" NVARCHAR(500) NOT NULL,
    "ImagePath" NVARCHAR(500) NOT NULL,
    "BlogId" BIGINT NULL,
    "CommentId" BIGINT NULL
);