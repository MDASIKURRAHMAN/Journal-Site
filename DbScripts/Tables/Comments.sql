use eJournalDb;

--Comments Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Comments' and xtype='U')
CREATE TABLE "Comments"(
    "CommentId" BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    "UserId" BIGINT NOT NULL,
    "CommentText" NVARCHAR(3000) NOT NULL,
    "ParentCommentId" BIGINT NULL,
    "UpdatedAt" DATETIME NULL,
    "CreatedAt" DATETIME NOT NULL,
    "BlogId" BIGINT NULL
);
